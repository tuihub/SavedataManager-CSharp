using Json.Schema;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using TuiHub.SavedataManagerLibrary.Properties;
using TuiHub.SavedataManagerLibrary.Utils;

namespace TuiHub.SavedataManagerLibrary
{
    public partial class SavedataManager
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
        private readonly ILogger? _logger;
        public SavedataManager(ILogger? logger)
        {
            _logger = logger;
        }
    }
}