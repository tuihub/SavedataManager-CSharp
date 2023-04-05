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
        public bool CheckFSLastWriteTimeNewer(Config config, ZipArchive zipArchive)
        {
            // compare LastWriteTime
            var zipArchiveEntriesMaxLastWriteTime = zipArchive.GetEntriesMaxLastWriteTime(s_savedataConfigFileName);
            _logger.LogDebug("zipArchiveEntriesMaxLastWriteTime = {ZipArchiveEntriesMaxLastWriteTime}", zipArchiveEntriesMaxLastWriteTime);
            DateTime? fsMaxLastWriteTime = null;

            if (config.Entries == null)
                _logger.LogWarning("config.Entries is null");
            else
                foreach (var entry in config.Entries)
                {
                    _logger.LogDebug("Entry {Entry}", entry.ToString());
                    if (entry.GetFSType() == EntryFSType.File)
                    {
                        _logger.LogDebug("Checking file: {EntryRealPath}", entry.GetRealPath());
                        var curFileLastWriteTime = File.GetLastWriteTime(entry.GetRealPath());
                        _logger.LogDebug("curFileLastWriteTime = {CurFileLastWriteTime}", curFileLastWriteTime);
                        _logger.LogDebug("fsMaxLastWriteTime = {fsMaxLastWriteTime}", fsMaxLastWriteTime);
                        if (fsMaxLastWriteTime == null || curFileLastWriteTime > fsMaxLastWriteTime)
                        {
                            _logger.LogDebug("Updating fsMaxLastWriteTime = {CurFileLastWriteTime}", curFileLastWriteTime);
                            fsMaxLastWriteTime = curFileLastWriteTime;
                        }
                    }
                    else if (entry.GetFSType() == EntryFSType.Folder)
                    {
                        if (Directory.Exists(entry.GetRealPath()) == false)
                        {
                            _logger.LogDebug("dir {EntryRealPath} not exists, skip", entry.GetRealPath());
                            continue;
                        }
                        var files = Directory.GetFiles(entry.GetRealPath(), "*", SearchOption.AllDirectories);
                        foreach (var file in files)
                        {
                            _logger.LogDebug("Checking file: {File}", file);
                            var curFileLastWriteTime = File.GetLastWriteTime(file);
                            _logger.LogDebug("curFileLastWriteTime = {CurFileLastWriteTime}", curFileLastWriteTime);
                            _logger.LogDebug("fsMaxLastWriteTime = {fsMaxLastWriteTime}", fsMaxLastWriteTime);
                            if (fsMaxLastWriteTime == null || curFileLastWriteTime > fsMaxLastWriteTime)
                            {
                                _logger.LogDebug("Updating fsMaxLastWriteTime = {CurFileLastWriteTime}", curFileLastWriteTime);
                                fsMaxLastWriteTime = curFileLastWriteTime;
                            }
                        }
                    }
                }
            bool ret;
            // current savedata is newer
            if (fsMaxLastWriteTime != null && fsMaxLastWriteTime > zipArchiveEntriesMaxLastWriteTime)
            {
                _logger.LogWarning("Current App savedata is newer than the one to restore");
                ret = true;
            }
            else
            {
                _logger.LogDebug("Current App savedata is not newer than the one to restore");
                ret = false;
            }
            return ret;
        }
    }
}
