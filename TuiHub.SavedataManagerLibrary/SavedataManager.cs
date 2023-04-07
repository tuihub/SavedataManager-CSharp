using log4net;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;

namespace TuiHub.SavedataManagerLibrary
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
            },
            // not converting to ASCII char
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
        };
        private readonly ILog _log;
        public SavedataManager(ILog log)
        {
            _log = log;
        }
    }
}