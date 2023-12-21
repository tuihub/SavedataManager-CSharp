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
            _logger?.LogInformation("Starting validation");
            var jsonNode = JsonNode.Parse(configStr);
            var jsonSchema = configStr.GetJsonSchema();
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
