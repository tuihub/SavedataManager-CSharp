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
using Microsoft.Json.Schema.Validation;
using Microsoft.Json.Schema;
using TuiHub.SavedataManagerLibrary.Properties;

namespace TuiHub.SavedataManagerLibrary
{
    public partial class SavedataManager
    {
        public bool Validate(string configStr)
        {
            _log.Info("Starting validation");
            var jsonSchemaStr = Resources.JsonSchemaStr;
            _log.Debug($"jsonSchemaStr = {jsonSchemaStr}");
            var jsonSchema = JsonSerializer.Deserialize<JsonSchema>(jsonSchemaStr);
            var validator = new Validator(jsonSchema);
            _log.Debug($"configStr = {configStr}");
            _log.Debug("Starting validation");
            var errors = validator.Validate(configStr, s_savedataConfigFileName);
            _log.Debug("Validation finished");
            var ret = true;
            if (errors.Any())
            {
                foreach (var error in errors) _log.Debug($"{error}");
                ret = false;
            }
            _log.Info("Returning validation result");
            return ret;
        }
    }
}
