using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace TuiHub.SavedataManager
{
    public partial class SavedataManager
    {
        private static readonly string s_savedataConfigFileName = "tuihub_savedata_config.json";
        private static readonly JsonSerializerOptions s_jsonSerializerOptions = new()
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
            Converters =
            {
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
            }
        };
        private readonly ILogger _logger;
        public SavedataManager(ILogger logger)
        {
            _logger = logger;
        }
    }
}