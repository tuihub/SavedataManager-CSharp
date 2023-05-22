using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;

namespace TuiHub.SavedataManagerLibrary
{
    public partial class SavedataManager<T>
    {
        private static readonly Encoding s_UTF8WithoutBom = new UTF8Encoding(false);
        private static readonly string s_savedataConfigFileName = "tuihub_savedata_config.json";
        private static readonly JsonSerializerOptions s_jsonSerializerOptions = new()
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
            Converters =
            {
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
            },
            // not converting to ASCII char
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
        };
        private readonly ILogger<T>? _logger;
        public SavedataManager(ILogger<T>? logger)
        {
            _logger = logger;
        }
    }
}