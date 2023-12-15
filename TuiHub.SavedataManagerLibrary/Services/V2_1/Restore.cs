using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuiHub.SavedataManagerLibrary.Services.V2_1
{
    public partial class Service
    {
        public bool Restore(object configObj, ZipArchive zipArchive, string gameDir, bool forceOverwrite = false)
        {
            throw new NotImplementedException();
        }
    }
}
