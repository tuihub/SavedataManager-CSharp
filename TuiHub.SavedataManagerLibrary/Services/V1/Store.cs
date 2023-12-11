using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
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
        public void Store(object configObj, Stream stream, string configPath)
        {
            Config config = (configObj as Config)!;
            // leaveOpen must be true
            using ZipArchive zipArchive = new ZipArchive(stream, ZipArchiveMode.Update, true);

            // add entries to zipArchive
            if (config.Entries == null)
                _logger?.LogWarning("config.Entries is null");
            else
                foreach (Entry entry in config.Entries)
                {
                    _logger?.LogDebug($"AddEntriesToZipArchive entry = {entry.ToString()}");
                    _logger?.LogDebug($"AddEntriesToZipArchive entry.GetRealPath() = {entry.GetRealPath()}");
                    zipArchive.CreateEntryFromAny(entry.GetRealPath(), entry.Id.ToString());
                }

            // add savedata config file
            _logger?.LogDebug("Adding SaveDataConfigFile to zipArchive");
            zipArchive.CreateEntryFromAny(configPath);

            // must dispose
            zipArchive.Dispose();
        }
    }
}
