using Microsoft.Extensions.Logging;
using System.IO.Compression;
using System.Text.Json;
using TuiHub.SavedataManagerLibrary.Models;

namespace TuiHub.SavedataManagerLibrary
{
    public partial class SavedataManager
    {
        public bool Restore(Stream archiveStream, string gameDirPath, bool forceOverwrite = false)
        {
            _logger?.LogInformation("Starting restore");

            string originWorkDir = Directory.GetCurrentDirectory();
            _logger?.LogDebug($"originWorkDir = {originWorkDir}");
            try
            {
                using var zipArchive = new ZipArchive(archiveStream, ZipArchiveMode.Read);
                string workDir = gameDirPath;
                _logger?.LogDebug($"workDir = {workDir}");
                _logger?.LogDebug($"Setting CurrentDirectory to {workDir}");
                Directory.SetCurrentDirectory(workDir);
                var deletedFolder = new HashSet<string>();

                var configEntry = zipArchive.GetEntry(s_savedataConfigFileName);
                if (configEntry == null)
                {
                    _logger?.LogError("configEntry is null");
                    throw new Exception("configEntry is null");
                }
                using var configEntryStreamReader = new StreamReader(configEntry.Open(), s_UTF8WithoutBom);
                string configStr = configEntryStreamReader.ReadToEnd();
                _logger?.LogDebug($"configStr = {configStr}");
                _logger?.LogDebug("Starting config validation");
                var validation = Validate(configStr);
                if (validation == false)
                {
                    _logger?.LogError("Savedata config validation failed");
                    throw new Exception("Savedata config validation failed");
                }
                _logger?.LogDebug("Validation finished");
                _logger?.LogDebug("Starting config deserialization");
                var config = JsonSerializer.Deserialize<Config>(configStr, s_jsonSerializerOptions);
                if (config == null)
                {
                    _logger?.LogError("config is null");
                    throw new Exception("config is null");
                }
                _logger?.LogDebug("Config deserialization finished");

                _logger?.LogDebug("Checking fs and archive last write time");
                bool fsLastWriteTimeNewer = InnerCheckFSLastWriteTimeNewer(config, zipArchive);
                if (fsLastWriteTimeNewer == true)
                {
                    _logger?.LogDebug($"forceOverwrite = {forceOverwrite}");
                    if (forceOverwrite == false)
                    {
                        _logger?.LogInformation("fs savedata is newer, not overwriting");

                        // restore working directory
                        _logger?.LogDebug($"Restoring CurrentDirectory to {originWorkDir}");
                        Directory.SetCurrentDirectory(originWorkDir);

                        return false;
                    }
                    else _logger?.LogWarning("fs savedata is newer, overwriting");
                }

                _logger?.LogDebug("Extracting entries from zipArchive");
                // extract entries from zipArchive
                if (config.Entries == null)
                    _logger?.LogWarning("config.Entries is null");
                else
                    foreach (var entry in config.Entries)
                    {
                        _logger?.LogDebug($"{entry.ToString()}");
                        var extractPath = Path.GetDirectoryName(entry.GetRealPath());
                        if (extractPath == null)
                        {
                            _logger?.LogError("extractPath is null");
                            throw new Exception("extractPath is null");
                        }
                        _logger?.LogDebug($"entry.GetRealPath() = {entry.GetRealPath()}");
                        _logger?.LogDebug($"extractPath = {extractPath}");
                        var zipArchiveBaseDirName = entry.Id.ToString();
                        var zipArchiveEntriesFiltered = from zipArchiveEntry in zipArchive.Entries
                                                        let entryDirName = Path.GetDirectoryName(zipArchiveEntry.FullName)
                                                        where entryDirName.StartsWith(zipArchiveBaseDirName)
                                                        where String.IsNullOrEmpty(zipArchiveEntry.Name) == false
                                                        select zipArchiveEntry;
                        if (entry.GetFSType() == EntryFSType.Folder)
                        {
                            if (zipArchiveEntriesFiltered.Any() == false)
                            {
                                _logger?.LogWarning($"Empty dir in entry: {entry.ToString()}");
                            }
                            else
                            {
                                var e = zipArchiveEntriesFiltered.First();
                                // not use folder path in zipArchive, use the one in config entry
                                //var folder = FolderHelper.GetRootFolder(e.FullName.Substring(zipArchiveBaseDirName.Length + 1));
                                //if (folder == null)
                                //{
                                //    _logger?.LogError("zipArchiveBaseDirName.GetRootFolder() is null");
                                //    return false;
                                //}
                                var path = extractPath;
                                if (deletedFolder.Contains(path))
                                {
                                    _logger?.LogDebug($"Dir {path} have been deleted");
                                }
                                else
                                {
                                    deletedFolder.Add(path);
                                    if (Directory.Exists(path) == false)
                                    {
                                        _logger?.LogDebug($"Dir {path} not exists");
                                    }
                                    else
                                    {
                                        _logger?.LogWarning($"Deleting dir: {path}");
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
                            _logger?.LogDebug($"zipArchiveEntry.name = {name}");
                            string path = Path.Combine(extractPath, name);
                            _logger?.LogDebug($"zipArchiveEntry.path = {path}");
                            var pathDirName = Path.GetDirectoryName(path);
                            _logger?.LogDebug($"pathDirName = {pathDirName}");
                            if (pathDirName == null)
                            {
                                _logger?.LogError("pathDirName is null");
                                throw new Exception("pathDirName is null");
                            }
                            if (Directory.Exists(pathDirName) == false)
                            {
                                _logger?.LogDebug($"Creating dir: {pathDirName}");
                                Directory.CreateDirectory(pathDirName);
                            }
                            zipArchiveEntry.ExtractToFile(path, true);
                        }
                    }

                // extract config
                var extractConfigPath = Path.Combine(Environment.CurrentDirectory, s_savedataConfigFileName);
                _logger?.LogDebug($"extractConfigPath = {extractConfigPath}");
                _logger?.LogDebug("Extracting SavedataConfigFile from zipArchive");
                configEntry.ExtractToFile(extractConfigPath, true);
                _logger?.LogInformation("Restore complete");

                _logger?.LogDebug($"Restoring CurrentDirectory to {originWorkDir}");
                Directory.SetCurrentDirectory(originWorkDir);

                return true;
            }
            catch
            {
                throw;
            }
            finally
            {
                // ensure restore working directory
                _logger?.LogDebug($"Restoring CurrentDirectory to {originWorkDir}");
                Directory.SetCurrentDirectory(originWorkDir);
            }
        }
    }
}
