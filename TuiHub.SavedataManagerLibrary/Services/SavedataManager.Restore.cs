using log4net;
using TuiHub.SavedataManagerLibrary.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TuiHub.SavedataManagerLibrary.Models;

namespace TuiHub.SavedataManagerLibrary
{
    public partial class SavedataManager
    {
        public bool Restore(string archivePath, string gameDirPath, bool forceOverwrite = false)
        {
            _log.Info("Starting restore");

            string savedataArchivePath = archivePath;
            _log.Debug($"savedataArchivePath = {savedataArchivePath}");
            using var zipArchive = ZipFile.Open(savedataArchivePath, ZipArchiveMode.Read);
            string workDir = gameDirPath;
            _log.Debug($"workDir = {workDir}");
            _log.Debug($"Setting CurrentDirectory to {workDir}");
            Directory.SetCurrentDirectory(workDir);
            var deletedFolder = new HashSet<string>();

            var configEntry = zipArchive.GetEntry(s_savedataConfigFileName);
            if (configEntry == null)
            {
                _log.Error("configEntry is null");
                return false;
            }
            using var configEntryStreamReader = new StreamReader(configEntry.Open(), Encoding.UTF8);
            string configStr = configEntryStreamReader.ReadToEnd();
            _log.Debug($"configStr = {configStr}");
            var config = JsonSerializer.Deserialize<Config>(configStr, s_jsonSerializerOptions);
            _log.Debug("Starting config deserialization");
            if (config == null)
            {
                _log.Error("config is null");
                return false;
            }
            _log.Debug("Config deserialization finished");

            _log.Debug("Checking fs and archive last write time");
            bool fsLastWriteTimeNewer = CheckFSLastWriteTimeNewer(config, zipArchive);
            if (fsLastWriteTimeNewer == true)
            {
                _log.Debug($"forceOverwrite = {forceOverwrite}");
                if (forceOverwrite == false)
                {
                    _log.Info("fs savedata is newer, not overwriting");
                    return false;
                }
                else _log.Warn("fs savedata is newer, overwriting");
            }

            _log.Debug("Extracting entries from zipArchive");
            // extract entries from zipArchive
            if (config.Entries == null)
                _log.Warn("config.Entries is null");
            else
                foreach (var entry in config.Entries)
                {
                    _log.Debug($"{entry.ToString()}");
                    var extractPath = Path.GetDirectoryName(entry.GetRealPath());
                    if (extractPath == null)
                    {
                        _log.Error("extractPath is null");
                        return false;
                    }
                    _log.Debug($"entry.GetRealPath() = {entry.GetRealPath()}");
                    _log.Debug($"extractPath = {extractPath}");
                    var zipArchiveBaseDirName = entry.Id.ToString();
                    var zipArchiveEntriesFiltered = from zipArchiveEntry in zipArchive.Entries
                                                    let entryDirName = Path.GetDirectoryName(zipArchiveEntry.FullName)
                                                    where entryDirName.StartsWith(zipArchiveBaseDirName)
                                                    where String.IsNullOrEmpty(zipArchiveEntry.Name) == false
                                                    select zipArchiveEntry;
                    if (entry.GetFSType() == EntryFSType.Folder)
                    {
                        if (zipArchiveEntriesFiltered.Count() == 0)
                        {
                            _log.Warn($"Empty dir in entry: {entry.ToString()}");
                        }
                        else
                        {
                            var e = zipArchiveEntriesFiltered.First();
                            var folder = FolderHelper.GetRootFolder(e.FullName.Substring(zipArchiveBaseDirName.Length + 1));
                            if (folder == null)
                            {
                                _log.Error("zipArchiveBaseDirName.GetRootFolder() is null");
                                return false;
                            }
                            var path = Path.Combine(extractPath, folder);
                            if (deletedFolder.Contains(path))
                            {
                                _log.Debug($"Dir {path} have been deleted");
                            }
                            else
                            {
                                deletedFolder.Add(path);
                                if (Directory.Exists(path) == false)
                                {
                                    _log.Debug($"Dir {path} not exists");
                                }
                                else
                                {
                                    _log.Warn($"Deleting dir: {path}");
                                    Directory.Delete(path, true);
                                    Directory.CreateDirectory(path);
                                }
                            }
                        }
                    }
                    // from https://stackoverflow.com/questions/36350199/c-sharp-extract-specific-directory-from-zip-preserving-folder-structure
                    foreach (var zipArchiveEntry in zipArchiveEntriesFiltered)
                    {
                        var name = zipArchiveEntry.FullName.Substring(zipArchiveBaseDirName.Length + 1);
                        _log.Debug($"zipArchiveEntry.name = {name}");
                        string path = Path.Combine(extractPath, name);
                        _log.Debug($"zipArchiveEntry.path = {path}");
                        var pathDirName = Path.GetDirectoryName(path);
                        _log.Debug($"pathDirName = {pathDirName}");
                        if (pathDirName == null)
                        {
                            _log.Error("pathDirName is null");
                            return false;
                        }
                        if (Directory.Exists(pathDirName) == false)
                        {
                            _log.Debug($"Creating dir: {pathDirName}");
                            Directory.CreateDirectory(pathDirName);
                        }
                        zipArchiveEntry.ExtractToFile(path, true);
                    }
                }

            // extract config
            var extractConfigPath = Path.Combine(Environment.CurrentDirectory, s_savedataConfigFileName);
            _log.Debug($"extractConfigPath = {extractConfigPath}");
            _log.Debug("Extracting SavedataConfigFile from zipArchive");
            configEntry.ExtractToFile(extractConfigPath, true);
            _log.Info("Restore complete");
            return true;
        }
    }
}
