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
        public MemoryStream? Store(string gameDirPath)
        {
            _logger.LogInformation("Starting store");
            _logger.LogDebug("Setting CurrentDirectory to {GameDirPath}", gameDirPath);
            Directory.SetCurrentDirectory(gameDirPath);
            string configPath = Path.Combine(Environment.CurrentDirectory, s_savedataConfigFileName);
            _logger.LogDebug("configPath = {ConfigPath}", configPath);
            string configStr = File.ReadAllText(configPath, Encoding.UTF8);
            _logger.LogDebug("configStr = {ConfigStr}", configStr);
            _logger.LogDebug("Starting config deserialization");
            var config = JsonSerializer.Deserialize<Config>(configStr, s_jsonSerializerOptions);
            if (config == null)
            {
                _logger.LogError("config is null");
                return null;
            }
            _logger.LogDebug("Config deserialization finished");

            // create ZipArchive using MemoryStream
            var memoryStream = new MemoryStream();
            // leaveOpen must be true
            using ZipArchive zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Update, true);

            // add entries to zipArchive
            if (config.Entries == null)
                _logger.LogWarning("config.Entries is null");
            else
                foreach (Entry entry in config.Entries)
                {
                    _logger.LogDebug("AddEntriesToZipArchive entry = {Entry}", entry.ToString());
                    _logger.LogDebug("AddEntriesToZipArchive entry.GetRealPath() = {EntryRealPath}", entry.GetRealPath());
                    zipArchive.CreateEntryFromAny(entry.GetRealPath(), entry.Id.ToString());
                }

            // add config.json
            _logger.LogDebug("Adding SaveDataConfigFile to zipArchive");
            zipArchive.CreateEntryFromAny(configPath);

            // must dispose
            zipArchive.Dispose();
            _logger.LogInformation("Returning memoryStream");
            return memoryStream;
        }
    }
}
