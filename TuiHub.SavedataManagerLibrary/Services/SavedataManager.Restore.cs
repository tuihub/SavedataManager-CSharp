using Microsoft.Extensions.Logging;
using System.IO.Compression;
using System.Text.Json;
using TuiHub.SavedataManagerLibrary.Models.V1;
using TuiHub.SavedataManagerLibrary.Utils;

namespace TuiHub.SavedataManagerLibrary
{
    public partial class SavedataManager
    {
        public void Restore(Stream archiveStream, string gameDir, bool forceOverwrite = false)
        {
            _logger?.LogInformation("Starting restore");

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
            var config = configStr.GetConfigObj();
            _logger?.LogDebug("Config deserialization finished");

            var service = configStr.GetIService(s_savedataConfigFileName, _logger);
            var restoreResult = service.Restore(config, zipArchive, gameDir, forceOverwrite);
            if (restoreResult == true)
            {
                // extract config
                var extractConfigPath = Path.Combine(gameDir, s_savedataConfigFileName);
                _logger?.LogDebug($"extractConfigPath = {extractConfigPath}");
                _logger?.LogDebug("Extracting SavedataConfigFile from zipArchive");
                configEntry.ExtractToFile(extractConfigPath, true);
                _logger?.LogInformation("Restore complete");
            }
        }
    }
}
