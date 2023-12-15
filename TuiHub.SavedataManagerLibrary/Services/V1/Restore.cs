using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuiHub.SavedataManagerLibrary.Models.V1;

namespace TuiHub.SavedataManagerLibrary.Services.V1
{
    public partial class Service
    {
        public bool Restore(object configObj, ZipArchive zipArchive, string gameDir, bool forceOverwrite = false)
        {
            string originWorkDir = Directory.GetCurrentDirectory();
            _logger?.LogDebug($"originWorkDir = {originWorkDir}");
            try
            {
                Config config = (configObj as Config)!;
                // set working directory
                _logger?.LogDebug($"workDir = {gameDir}");
                _logger?.LogDebug($"Setting CurrentDirectory to {gameDir}");
                Directory.SetCurrentDirectory(gameDir);
                var deletedFolder = new HashSet<string>();
                _logger?.LogDebug("Checking fs and archive last write time");
                bool fsLastWriteTimeNewer = CheckFSLastWriteTimeNewer(config, zipArchive, gameDir);
                if (fsLastWriteTimeNewer == true)
                {
                    _logger?.LogDebug($"forceOverwrite = {forceOverwrite}");
                    if (forceOverwrite == false)
                    {
                        _logger?.LogInformation("fs savedata is newer, not overwriting");
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
                return true;
            }
            catch
            {
                throw;
            }
            finally
            {
                // restore working directory
                _logger?.LogDebug($"Restoring CurrentDirectory to {originWorkDir}");
                Directory.SetCurrentDirectory(originWorkDir);
            }
        }
    }
}
