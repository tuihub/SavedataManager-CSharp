using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TuiHub.SavedataManagerLibrary.Models;
using TuiHub.SavedataManagerLibrary.Utils;

namespace TuiHub.SavedataManagerLibrary
{
    public partial class SavedataManager
    {
        public bool CheckFSLastWriteTimeNewer(string archivePath)
        {
            string savedataArchivePath = archivePath;
            _log.Debug($"savedataArchivePath = {savedataArchivePath}");
            using var zipArchive = ZipFile.Open(savedataArchivePath, ZipArchiveMode.Read);
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

            return InnerCheckFSLastWriteTimeNewer(config, zipArchive);
        }

        private bool InnerCheckFSLastWriteTimeNewer(Config config, ZipArchive zipArchive)
        {
            // compare LastWriteTime
            var zipArchiveEntriesMaxLastWriteTime = zipArchive.GetEntriesMaxLastWriteTime(s_savedataConfigFileName);
            _log.Debug($"zipArchiveEntriesMaxLastWriteTime = {zipArchiveEntriesMaxLastWriteTime}");
            DateTime? fsMaxLastWriteTime = null;

            if (config.Entries == null)
                _log.Warn("config.Entries is null");
            else
                foreach (var entry in config.Entries)
                {
                    _log.Debug($"{entry.ToString()}");
                    if (entry.GetFSType() == EntryFSType.File)
                    {
                        _log.Debug($"Checking file: {entry.GetRealPath()}");
                        var curFileLastWriteTime = File.GetLastWriteTime(entry.GetRealPath());
                        _log.Debug($"curFileLastWriteTime = {curFileLastWriteTime}");
                        _log.Debug($"fsMaxLastWriteTime = {fsMaxLastWriteTime}");
                        if (fsMaxLastWriteTime == null || curFileLastWriteTime > fsMaxLastWriteTime)
                        {
                            _log.Debug($"Updating fsMaxLastWriteTime = {curFileLastWriteTime}");
                            fsMaxLastWriteTime = curFileLastWriteTime;
                        }
                    }
                    else if (entry.GetFSType() == EntryFSType.Folder)
                    {
                        if (Directory.Exists(entry.GetRealPath()) == false)
                        {
                            _log.Debug($"Dir {entry.GetRealPath()} not exists, skip");
                            continue;
                        }
                        var files = Directory.GetFiles(entry.GetRealPath(), "*", SearchOption.AllDirectories);
                        foreach (var file in files)
                        {
                            _log.Debug($"Checking file: {file}");
                            var curFileLastWriteTime = File.GetLastWriteTime(file);
                            _log.Debug($"curFileLastWriteTime = {curFileLastWriteTime}");
                            _log.Debug($"fsMaxLastWriteTime = {fsMaxLastWriteTime}");
                            if (fsMaxLastWriteTime == null || curFileLastWriteTime > fsMaxLastWriteTime)
                            {
                                _log.Debug($"Updating fsMaxLastWriteTime = {curFileLastWriteTime}");
                                fsMaxLastWriteTime = curFileLastWriteTime;
                            }
                        }
                    }
                }
            bool ret;
            // current savedata is newer
            if (fsMaxLastWriteTime != null && fsMaxLastWriteTime > zipArchiveEntriesMaxLastWriteTime)
            {
                _log.Warn("Current App savedata is newer than the one to restore");
                ret = true;
            }
            else
            {
                _log.Debug("Current App savedata is not newer than the one to restore");
                ret = false;
            }
            return ret;
        }
    }
}
