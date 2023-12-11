using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TuiHub.SavedataManagerLibrary.Contracts;

namespace TuiHub.SavedataManagerLibrary.Services.V1
{
    public partial class Service : IService
    {
        private readonly ILogger? _logger;
        private readonly string s_savedataConfigFileName;

        public Service(ILogger? logger, string savedataConfigFileName)
        {
            _logger = logger;
            s_savedataConfigFileName = savedataConfigFileName;
        }
    }
}
