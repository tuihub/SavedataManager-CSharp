using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SavedataManager
{
    public static class Global
    {
        public static readonly LogLevel LogLevel = Enum.Parse<LogLevel>(ConfigurationManager.AppSettings["logLevel"] ?? "INFO");

        public static readonly string ExecutionPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
        public static readonly string ExecutionFolderPath = Path.GetDirectoryName(ExecutionPath);
        public static readonly string SavedataArchiveFolderName = ConfigurationManager.AppSettings["savedataArchiveFolderName"] ?? "archives";
        public static readonly string SavedataArchiveFolderPath = ConfigurationManager.AppSettings["savedataArchiveFolderPath"] ?? Path.Combine(ExecutionFolderPath, SavedataArchiveFolderName);
        public static readonly string SavedataConfigFileName = ConfigurationManager.AppSettings["savedataConfigFileName"] ?? "savedata_manager_config.json";
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
