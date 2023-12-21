using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuiHub.SavedataManagerLibrary.Models.V2_1;
using TuiHub.SavedataManagerLibrary.Services.V2_1.Utils;
using TuiHub.SavedataManagerLibrary.Utils;

namespace TuiHub.SavedataManagerLibrary.Services.V2_1
{
    public partial class Service
    {
        public void Store(object configObj, Stream stream, string configPath, string gameDir)
        {
            Config config = (configObj as Config)!;
            using ZipArchive zipArchive = new(stream, ZipArchiveMode.Update, true);

            _logger?.LogDebug($"Adding file = {configPath}");
            zipArchive.CreateEntryFromAny(configPath);
            if (config.Entries == null || !config.Entries.Any())
                _logger?.LogWarning("config.Entries is null or empty");
            else
                foreach (var entry in config.Entries)
                {
                    _logger?.LogDebug($"Processing entry = {entry.Id.ToString()}");
                    var entryBaseDir = entry.GetRealBaseDir(gameDir);
                    var files = FSUtil.GetFSFilesFromEntry(config, entry, entryBaseDir);
                    foreach (var file in files)
                    {
                        _logger?.LogDebug($"Adding file = {file}");
                        zipArchive.CreateEntryFromFile(file, Path.Combine(entry.Id.ToString(), Path.GetRelativePath(entryBaseDir, file)));
                    }
                }

            zipArchive.Dispose();
        }
    }
}
