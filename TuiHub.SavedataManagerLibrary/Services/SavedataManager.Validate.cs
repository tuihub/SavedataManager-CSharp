using TuiHub.SavedataManagerLibrary.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TuiHub.SavedataManagerLibrary.Models;
using TuiHub.SavedataManagerLibrary.Properties;
using System.Text.Json.Nodes;
using Json.Schema;

namespace TuiHub.SavedataManagerLibrary
{
    public partial class SavedataManager
    {
        public bool Validate(string configStr)
        {
            _log.Info("Starting validation");
            var jsonSchemaStr = Resources.JsonSchemaStr;
            _log.Debug($"jsonSchemaStr = {jsonSchemaStr}");
            _log.Debug($"configStr = {configStr}");
            var jsonNode = JsonNode.Parse(configStr);
            var jsonSchema = JsonSchema.FromText(jsonSchemaStr, s_jsonSerializerOptions);
            _log.Debug("Starting validation");
            var results = jsonSchema.Evaluate(jsonNode);
            _log.Debug("Validation finished");
            var ret = true;
            if (results.IsValid == false)
            {
                if (results.Errors != null)
                    foreach (var error in results.Errors)
                        _log.Debug($"{error}");
                ret = false;
            }
            _log.Debug("Returning validation result");
            return ret;
        }
    }
}
