using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TuiHub.SavedataManagerLibrary.Models.V2_1;
using TuiHub.SavedataManagerLibrary.Services.V2_1.Utils;
using TuiHub.SavedataManagerLibrary.Utils;

namespace TuiHub.SavedataManagerLibrary.Services.V2_1
{
    public partial class Service
    {
        public bool CheckFSLastWriteTimeNewer(object configObj, ZipArchive zipArchive, string gameDir)
        {
            _logger?.LogInformation("Checking last write time");
            Config config = (configObj as Config)!;
            if (config.Entries == null || !config.Entries.Any())
                throw new ArgumentException("config.Entries is null or empty");
            var zipArchiveLastWriteTime = zipArchive.GetEntriesMaxLastWriteTime(_savedataConfigFileName);
            _logger?.LogDebug($"zipArchiveLastWriteTime = {zipArchiveLastWriteTime}");
            var fsLastWriteTime = GetFSLastWriteTime(config, gameDir);
            _logger?.LogDebug($"fsLastWriteTime = {fsLastWriteTime}");
            return fsLastWriteTime > zipArchiveLastWriteTime;
        }

        private DateTime GetFSLastWriteTime(Config config, string gameDir)
        {
            var lastWriteTime = DateTime.MinValue;
            foreach (var entry in config.Entries!)
            {
                var entryBaseDir = entry.GetRealBaseDir(gameDir);
                if (entry.FilePatterns == null || !entry.FilePatterns.Any())
                {
                    _logger?.LogWarning($"entry.FilePatterns is null or empty, entryBaseDir = {entryBaseDir}");
                    continue;
                }
                List<string> files = FSUtil.GetFSFilesFromEntry(config, entry, entryBaseDir);
                DateTime entryLastWriteTime;
                if (!files.Any())
                {
                    _logger?.LogWarning($"files is null or empty, entryBaseDir = {entryBaseDir}");
                    entryLastWriteTime = DateTime.MinValue;
                }
                else
                {
                    entryLastWriteTime = files.Select(e => File.GetLastWriteTime(e)).Max();
                    _logger?.LogDebug($"entryLastWriteTime = {entryLastWriteTime}, entryBaseDir = {entryBaseDir}");
                }
                lastWriteTime = entryLastWriteTime > lastWriteTime ? entryLastWriteTime : lastWriteTime;
            }
            return lastWriteTime;
        }
    }
}
