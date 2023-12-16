using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuiHub.SavedataManagerLibrary.Models.V1;
using TuiHub.SavedataManagerLibrary.Utils;

namespace TuiHub.SavedataManagerLibrary.Services.V1
{
    public partial class Service
    {
        public bool CheckFSLastWriteTimeNewer(object configObj, ZipArchive zipArchive, string gameDir)
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
                // compare LastWriteTime
                var zipArchiveEntriesMaxLastWriteTime = zipArchive.GetEntriesMaxLastWriteTime(_savedataConfigFileName);
                _logger?.LogDebug($"zipArchiveEntriesMaxLastWriteTime = {zipArchiveEntriesMaxLastWriteTime}");
                DateTime? fsMaxLastWriteTime = null;

                if (config.Entries == null)
                    _logger?.LogWarning("config.Entries is null");
                else
                    foreach (var entry in config.Entries)
                    {
                        _logger?.LogDebug($"{entry.ToString()}");
                        if (entry.GetFSType() == EntryFSType.File)
                        {
                            _logger?.LogDebug($"Checking file: {entry.GetRealPath()}");
                            var curFileLastWriteTime = File.GetLastWriteTime(entry.GetRealPath());
                            _logger?.LogDebug($"curFileLastWriteTime = {curFileLastWriteTime}");
                            _logger?.LogDebug($"fsMaxLastWriteTime = {fsMaxLastWriteTime}");
                            if (fsMaxLastWriteTime == null || curFileLastWriteTime > fsMaxLastWriteTime)
                            {
                                _logger?.LogDebug($"Updating fsMaxLastWriteTime = {curFileLastWriteTime}");
                                fsMaxLastWriteTime = curFileLastWriteTime;
                            }
                        }
                        else if (entry.GetFSType() == EntryFSType.Folder)
                        {
                            if (Directory.Exists(entry.GetRealPath()) == false)
                            {
                                _logger?.LogDebug($"Dir {entry.GetRealPath()} not exists, skip");
                                continue;
                            }
                            var files = Directory.GetFiles(entry.GetRealPath(), "*", SearchOption.AllDirectories);
                            foreach (var file in files)
                            {
                                _logger?.LogDebug($"Checking file: {file}");
                                var curFileLastWriteTime = File.GetLastWriteTime(file);
                                _logger?.LogDebug($"curFileLastWriteTime = {curFileLastWriteTime}");
                                _logger?.LogDebug($"fsMaxLastWriteTime = {fsMaxLastWriteTime}");
                                if (fsMaxLastWriteTime == null || curFileLastWriteTime > fsMaxLastWriteTime)
                                {
                                    _logger?.LogDebug($"Updating fsMaxLastWriteTime = {curFileLastWriteTime}");
                                    fsMaxLastWriteTime = curFileLastWriteTime;
                                }
                            }
                        }
                    }
                bool ret;
                // current savedata is newer
                if (fsMaxLastWriteTime != null && fsMaxLastWriteTime > zipArchiveEntriesMaxLastWriteTime)
                {
                    _logger?.LogWarning("Current App savedata is newer than the one to restore");
                    ret = true;
                }
                else
                {
                    _logger?.LogDebug("Current App savedata is not newer than the one to restore");
                    ret = false;
                }
                return ret;
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
