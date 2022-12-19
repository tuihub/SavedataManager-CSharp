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

        public static LogLevel LogLevel = LogLevel.INFO;
    }
    public enum LogLevel
    {
        NONE = 0,
        ERROR = 1,
        WARN = 2,
        INFO = 3,
        DEBUG = 4,
    }
}
