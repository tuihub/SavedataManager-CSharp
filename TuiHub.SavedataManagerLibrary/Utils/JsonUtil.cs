using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.Json;
using System.Threading.Tasks;

namespace TuiHub.SavedataManagerLibrary.Utils
{
    public static class JsonUtil
    {
        public static string GetConfigSchemaId(this string configStr)
        {
            var jsonNode = JsonSerializer.Deserialize<JsonNode>(configStr);
            if (jsonNode == null)
                throw new Exception("Json schema deserialization failed");
            var schemaId = jsonNode["$id"]?.ToString();
            if (schemaId == null)
                throw new Exception("Not a vaild SavedataConfig");
            return schemaId;
        }
    }
}
