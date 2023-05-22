using Microsoft.Extensions.Logging;
using System.IO.Compression;
using System.Text.Json;
using TuiHub.SavedataManagerLibrary.Models;
using TuiHub.SavedataManagerLibrary.Utils;

namespace TuiHub.SavedataManagerLibrary
{
    public partial class SavedataManager<T>
    {
        public MemoryStream Store(string gameDirPath)
        {
            _logger?.LogInformation("Starting store");
            _logger?.LogDebug($"Setting CurrentDirectory to {gameDirPath}");
            Directory.SetCurrentDirectory(gameDirPath);
            string configPath = Path.Combine(Environment.CurrentDirectory, s_savedataConfigFileName);
            _logger?.LogDebug($"configPath = {configPath}");
            string configStr = File.ReadAllText(configPath, s_UTF8WithoutBom);
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

            // create ZipArchive using MemoryStream
            var memoryStream = new MemoryStream();
            // leaveOpen must be true
            using ZipArchive zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Update, true);

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

            // add config.json
            _logger?.LogDebug("Adding SaveDataConfigFile to zipArchive");
            zipArchive.CreateEntryFromAny(configPath);

            // must dispose
            zipArchive.Dispose();
            _logger?.LogInformation("Returning memoryStream");
            return memoryStream;
        }
    }
}
