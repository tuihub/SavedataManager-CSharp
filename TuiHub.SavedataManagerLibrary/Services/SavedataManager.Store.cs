using Microsoft.Extensions.Logging;
using System.IO.Compression;
using System.Text.Json;
using TuiHub.SavedataManagerLibrary.Models.V1;
using TuiHub.SavedataManagerLibrary.Utils;

namespace TuiHub.SavedataManagerLibrary
{
    public partial class SavedataManager
    {
        public void Store(string gameDir, Stream stream)
        {
            _logger?.LogInformation("Starting store");
            string configPath = Path.Combine(gameDir, s_savedataConfigFileName);
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
            var config = configStr.GetConfigObj();
            _logger?.LogDebug("Config deserialization finished");

            var service = configStr.GetIService(s_savedataConfigFileName, _logger);
            service.Store(config, stream, gameDir, configPath);
        }
    }
}
