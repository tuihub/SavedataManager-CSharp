using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.Json;
using System.Threading.Tasks;
using TuiHub.SavedataManagerLibrary.Contracts;
using Microsoft.Extensions.Logging;
using Json.Schema;

namespace TuiHub.SavedataManagerLibrary.Utils
{
    public static class JsonSchemaUtil
    {
        private static string GetConfigSchemaId(this string configStr)
        {
            var jsonNode = JsonSerializer.Deserialize<JsonNode>(configStr);
            if (jsonNode == null)
                throw new Exception("Json schema deserialization to JsonNode failed");
            var schemaId = jsonNode["$schema"]?.ToString();
            if (schemaId == null)
                throw new Exception("$schema not found");
            return schemaId;
        }

        public static object GetConfigObj(this string configStr)
        {
            var jsonSchemaId = configStr.GetConfigSchemaId();
            object? config = jsonSchemaId switch
            {
                "https://tuihub.github.io/protos/schemas/savedata/v1.json" =>
                    JsonSerializer.Deserialize<Models.V1.Config>(configStr, Models.V1.Config.JsonSerializerOptions),
                "https://tuihub.github.io/protos/schemas/savedata/v2.1.json" =>
                    JsonSerializer.Deserialize<Models.V2_1.Config>(configStr, Models.V2_1.Config.JsonSerializerOptions),
                _ => throw new Exception($"{jsonSchemaId} not supported")
            };
            if (config == null)
                throw new Exception("Json schema deserialization to object failed");
            return config!;
        }

        public static JsonSchema GetJsonSchema(this string configStr)
        {
            var jsonSchemaId = configStr.GetConfigSchemaId();
            var jsonSchemaStr = jsonSchemaId switch
            {
                "https://tuihub.github.io/protos/schemas/savedata/v1.json" => Models.V1.Config.JsonSchemaStr,
                "https://tuihub.github.io/protos/schemas/savedata/v2.1.json" => Models.V2_1.Config.JsonSchemaStr,
                _ => throw new Exception($"jsonSchemaId {jsonSchemaId} not supported"),
            };
            var jsonSerializerOptions = jsonSchemaId switch
            {
                "https://tuihub.github.io/protos/schemas/savedata/v1.json" => Models.V1.Config.JsonSerializerOptions,
                "https://tuihub.github.io/protos/schemas/savedata/v2.1.json" => Models.V2_1.Config.JsonSerializerOptions,
                _ => throw new Exception($"jsonSchemaId {jsonSchemaId} not supported"),
            };
            var jsonSchema = JsonSchema.FromText(jsonSchemaStr, jsonSerializerOptions);
            return jsonSchema;
        }

        public static IService GetIService(this object configObj, string savedataConfigFileName, ILogger? logger = null)
        {
            IService service = configObj switch
            {
                Models.V1.Config => new Services.V1.Service(savedataConfigFileName, logger),
                Models.V2_1.Config => new Services.V2_1.Service(savedataConfigFileName, logger),
                _ => throw new Exception("configObj is not a valid SavedataConfig"),
            };
            return service;
        }
    }
}
