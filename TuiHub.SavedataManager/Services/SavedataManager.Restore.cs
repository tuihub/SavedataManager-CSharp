using log4net;
using Microsoft.Extensions.Logging;
using SavedataManager.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TuiHub.SavedataManager.Models;

namespace TuiHub.SavedataManager
{
    public partial class SavedataManager
    {
        public bool Restore(string archivePath, string gameDirPath, bool forceOverwrite = false)
        {
            _logger.LogInformation("Starting restore");

            string savedataArchivePath = archivePath;
            _logger.LogDebug("savedataArchivePath = {SavedataArchivePath}", savedataArchivePath);
            using var zipArchive = ZipFile.Open(savedataArchivePath, ZipArchiveMode.Read);
            string workDir = gameDirPath;
            _logger.LogDebug("workDir = {WorkDir}", workDir);
            _logger.LogDebug("Setting CurrentDirectory to {WorkDir}", workDir);
            Directory.SetCurrentDirectory(workDir);
            var deletedFolder = new HashSet<string>();

            var configEntry = zipArchive.GetEntry(s_savedataConfigFileName);
            if (configEntry == null)
            {
                _logger.LogError("configEntry is null");
                return false;
            }
            using var configEntryStreamReader = new StreamReader(configEntry.Open(), Encoding.UTF8);
            string configStr = configEntryStreamReader.ReadToEnd();
            _logger.LogDebug("configStr = {ConfigStr}", configStr);
            var config = JsonSerializer.Deserialize<Config>(configStr, s_jsonSerializerOptions);
            _logger.LogDebug("Starting config deserialization");
            if (config == null)
            {
                _logger.LogError("config is null");
                return false;
            }
            _logger.LogDebug("Config deserialization finished");

            _logger.LogDebug("Checking fs and archive last write time");
            bool fsLastWriteTimeNewer = CheckFSLastWriteTimeNewer(config, zipArchive);
            if (fsLastWriteTimeNewer == true)
            {
                _logger.LogDebug("forceOverwrite = {ForceOverwrite}", forceOverwrite);
                if (forceOverwrite == false)
                {
                    _logger.LogInformation("fs savedata is newer, not overwriting");
                    return false;
                }
                else _logger.LogWarning("fs savedata is newer, overwriting");
            }

            _logger.LogDebug("Extracting entries from zipArchive");
            // extract entries from zipArchive
            if (config.Entries == null)
                _logger.LogWarning("config.Entries is null");
            else
                foreach (var entry in config.Entries)
                {
                    _logger.LogDebug("{Entry}", entry.ToString());
                    var extractPath = Path.GetDirectoryName(entry.GetRealPath());
                    if (extractPath == null)
                    {
                        _logger.LogError("extractPath is null");
                        return false;
                    }
                    _logger.LogDebug("entry.GetRealPath() = {EntryRealPath}", entry.GetRealPath());
                    _logger.LogDebug("extractPath = {ExtractPath}", extractPath);
                    var zipArchiveBaseDirName = entry.Id.ToString();
                    var zipArchiveEntriesFiltered = from zipArchiveEntry in zipArchive.Entries
                                                    where Path.GetDirectoryName(zipArchiveEntry.FullName).StartsWith(zipArchiveBaseDirName)
                                                    where String.IsNullOrEmpty(zipArchiveEntry.Name) == false
                                                    select zipArchiveEntry;
                    if (entry.GetFSType() == EntryFSType.Folder)
                    {
                        if (zipArchiveEntriesFiltered.Count() == 0)
                        {
                            _logger.LogWarning("Empty dir in entry: {Entry}", entry.ToString());
                        }
                        else
                        {
                            var e = zipArchiveEntriesFiltered.First();
                            var folder = FolderHelper.GetRootFolder(e.FullName.Substring(zipArchiveBaseDirName.Length + 1));
                            var path = Path.Combine(extractPath, folder);
                            if (deletedFolder.Contains(path))
                            {
                                _logger.LogDebug("Dir {Path} have been deleted", path);
                            }
                            else
                            {
                                deletedFolder.Add(path);
                                if (Directory.Exists(path) == false)
                                {
                                    _logger.LogDebug("Dir {Path} not exists", path);
                                }
                                else
                                {
                                    _logger.LogWarning("Deleting dir: {Path}", path);
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
                        _logger.LogDebug("zipArchiveEntry.name = {Name}", name);
                        string path = Path.Combine(extractPath, name);
                        _logger.LogDebug("zipArchiveEntry.path = {Path}", path);
                        if (Directory.Exists(Path.GetDirectoryName(path)) == false)
                        {
                            _logger.LogDebug("Creating dir: {PathDirectoryName}", Path.GetDirectoryName(path));
                            Directory.CreateDirectory(Path.GetDirectoryName(path));
                        }
                        zipArchiveEntry.ExtractToFile(path, true);
                    }
                }

            // extract config
            var extractConfigPath = Path.Combine(Environment.CurrentDirectory, s_savedataConfigFileName);
            _logger.LogDebug("extractConfigPath = {ExtractConfigPath}", extractConfigPath);
            _logger.LogDebug("Extracting SavedataConfigFile from zipArchive");
            configEntry.ExtractToFile(extractConfigPath, true);
            _logger.LogInformation("Restore complete");
            return true;
        }
    }
}
