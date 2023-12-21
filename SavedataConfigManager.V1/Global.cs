using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using Models;
using System.Text.Encodings.Web;
using System.Text.Unicode;

namespace SavedataConfigManager
{
    internal static class Global
    {
        public static readonly Encoding UTF8WithoutBom = new UTF8Encoding(false);
        public static readonly string SavedataConfigFileName = "tuihub_savedata_config.json";
        public static readonly JsonSerializerOptions JsonSerializerOptions = new()
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
        public static string? CurrentOpenedConfigFilePath;
    }
}
