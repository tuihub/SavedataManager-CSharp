using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SavedataManager
{
    public static class Global
    {
        public static readonly string ExecutionPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
        public static readonly string ExecutionFolderPath = Path.GetDirectoryName(ExecutionPath);
        public static readonly string SavedataArchiveFolderName = "archives";
        public static readonly string SavedataArchiveFolderPath = ExecutionFolderPath + Path.AltDirectorySeparatorChar + SavedataArchiveFolderName;
        public static readonly string SavedataConfig = "savedata_manager_config.json";
    }
}
