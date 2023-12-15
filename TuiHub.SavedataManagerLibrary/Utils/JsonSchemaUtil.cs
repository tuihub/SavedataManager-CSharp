﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.Json;
using System.Threading.Tasks;
using TuiHub.SavedataManagerLibrary.Contracts;
using Microsoft.Extensions.Logging;

namespace TuiHub.SavedataManagerLibrary.Utils
{
    public static class JsonSchemaUtil
    {
        public static string GetConfigSchemaId(this string configStr)
        {
            var jsonNode = JsonSerializer.Deserialize<JsonNode>(configStr);
            if (jsonNode == null)
                throw new Exception("Json schema deserialization to JsonNode failed");
            var schemaId = jsonNode["$id"]?.ToString();
            if (schemaId == null)
                throw new Exception("Not a vaild SavedataConfig");
            return schemaId;
        }

        public static object GetConfigObj(this string configStr, JsonSerializerOptions? jsonSerializerOptions = null)
        {
            var jsonSchemaId = configStr.GetConfigSchemaId();
            object? config = jsonSchemaId switch
            {
                "https://github.com/tuihub/protos/schemas/savedata/v1" =>
                    JsonSerializer.Deserialize<Models.V1.Config>(configStr, jsonSerializerOptions),
                "https://github.com/tuihub/protos/schemas/savedata/v2.1.json" =>
                    JsonSerializer.Deserialize<Models.V2_1.Config>(configStr, jsonSerializerOptions),
                _ => throw new Exception($"{jsonSchemaId} not supported")
            };
            if (config == null)
                throw new Exception("Json schema deserialization to object failed");
            return config!;
        }

        public static IService GetIService(this string configStr, string savedataConfigFileName, ILogger? logger = null)
        {
            var jsonSchemaId = configStr.GetConfigSchemaId();
            IService service = jsonSchemaId switch
            {
                "https://github.com/tuihub/protos/schemas/savedata/v1" =>
                     new Services.V1.Service(logger, savedataConfigFileName),
                "https://github.com/tuihub/protos/schemas/savedata/v2.1.json" =>
                    new Services.V2_1.Service(logger, savedataConfigFileName),
                _ => throw new Exception($"{jsonSchemaId} not supported")
            };
            return service;
        }
    }
}