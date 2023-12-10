using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TuiHub.SavedataManagerLibrary.Services.V1
{
    public partial class Service
    {
        private readonly ILogger? _logger;
        private readonly string s_savedataConfigFileName;
        private readonly JsonSerializerOptions s_jsonSerializerOptions;

        public Service(ILogger? logger, string savedataConfigFileName, JsonSerializerOptions jsonSerializerOptions)
        {
            _logger = logger;
            s_savedataConfigFileName = savedataConfigFileName;
            s_jsonSerializerOptions = jsonSerializerOptions;
        }
    }
}
