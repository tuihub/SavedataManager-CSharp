using Json.Schema;
using Microsoft.Extensions.Logging;
using System.Text.Json.Nodes;
using TuiHub.SavedataManagerLibrary.Properties;

namespace TuiHub.SavedataManagerLibrary
{
    public partial class SavedataManager<T>
    {
        public bool Validate(string configStr)
        {
            _logger?.LogDebug("Starting validation");
            var jsonSchemaStr = Resources.JsonSchemaStr;
            _logger?.LogDebug($"jsonSchemaStr = {jsonSchemaStr}");
            _logger?.LogDebug($"configStr = {configStr}");
            var jsonNode = JsonNode.Parse(configStr);
            var jsonSchema = JsonSchema.FromText(jsonSchemaStr, s_jsonSerializerOptions);
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
