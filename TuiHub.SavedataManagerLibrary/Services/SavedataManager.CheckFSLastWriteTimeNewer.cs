using Microsoft.Extensions.Logging;
using System.IO.Compression;
using System.Text.Json;
using TuiHub.SavedataManagerLibrary.Models;
using TuiHub.SavedataManagerLibrary.Utils;

namespace TuiHub.SavedataManagerLibrary
{
    public partial class SavedataManager
    {
        public bool CheckFSLastWriteTimeNewer(Stream archiveStream, string gameDirPath)
        {
            string workDir = gameDirPath;
            _logger?.LogDebug($"workDir = {workDir}");
            _logger?.LogDebug($"Setting CurrentDirectory to {workDir}");
            Directory.SetCurrentDirectory(workDir);
            using var zipArchive = new ZipArchive(archiveStream, ZipArchiveMode.Read);
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

            return InnerCheckFSLastWriteTimeNewer(config, zipArchive);
        }

        private bool InnerCheckFSLastWriteTimeNewer(Config config, ZipArchive zipArchive)
        {
            // compare LastWriteTime
            var zipArchiveEntriesMaxLastWriteTime = zipArchive.GetEntriesMaxLastWriteTime(s_savedataConfigFileName);
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
    }
}
