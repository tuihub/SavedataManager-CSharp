using Json.Schema;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Nodes;
using TuiHub.SavedataManagerLibrary.Properties;
using TuiHub.SavedataManagerLibrary.Utils;

namespace TuiHub.SavedataManagerLibrary
{
    public partial class SavedataManager
    {
        public bool Validate(string configStr)
        {
            _logger?.LogDebug("Starting validation");
            var jsonSchemaId = configStr.GetConfigSchemaId();
            var jsonSchemaStr = jsonSchemaId switch
            {
                "https://github.com/tuihub/protos/schemas/savedata/v1" => Resources.JsonSchemaV1Str,
                "https://github.com/tuihub/protos/schemas/savedata/v2.1.json" => Resources.JsonSchemaV2_1Str,
                _ => throw new Exception($"{jsonSchemaId} not supported"),
            };
            var jsonSerializerOptions = jsonSchemaId switch
            {
                "https://github.com/tuihub/protos/schemas/savedata/v1" => Models.V1.Config.JsonSerializerOptions,
                "https://github.com/tuihub/protos/schemas/savedata/v2.1.json" => Models.V2_1.Config.JsonSerializerOptions,
                _ => throw new Exception($"{jsonSchemaId} not supported"),
            };
            _logger?.LogDebug($"jsonSchemaStr = {jsonSchemaStr}");
            _logger?.LogDebug($"configStr = {configStr}");
            var jsonNode = JsonNode.Parse(configStr);
            var jsonSchema = JsonSchema.FromText(jsonSchemaStr, jsonSerializerOptions);
            _logger?.LogDebug("Starting validation");
            var results = jsonSchema.Evaluate(jsonNode);
            _logger?.LogDebug("Validation finished");
            var ret = true;
            if (results.IsValid == false)
            {
                if (results.Errors != null)
                    foreach (var error in results.Errors)
                        _logger?.LogDebug($"{error}");
                ret = false;
            }
            _logger?.LogDebug("Returning validation result");
            return ret;
        }
    }
}
