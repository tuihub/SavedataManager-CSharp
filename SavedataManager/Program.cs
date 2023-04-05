using CommandLine;
using log4net;
using log4net.Core;
using SavedataManager;
using SavedataManager.Utils;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SavedataManager
{
    class Program
    {
        private readonly ILog _log = LogManager.GetLogger(typeof(Program));
        void Main(string[] args)
        {
            try
            {
                _log.Debug("Starting program");
                Parser.Default.ParseArguments<StoreOptions, RestoreOptions>(args)
                    .WithParsed<StoreOptions>(RunStore)
                    .WithParsed<RestoreOptions>(RunRestore)
                    .WithNotParsed(HandleParseError);
            }
            catch (Exception e)
            {
                _log.Error($"{e.Message}");
                _log.Debug($"{e.StackTrace}");
            }
        }

        private void HandleParseError(IEnumerable<Error> errs)
        {
            if (errs.IsVersion())
            {
                _log.Debug("Running version");
                return;
            }
            if (errs.IsHelp())
            {
                _log.Debug("Running help");
                return;
            }
        }

        private static void RunStore(StoreOptions opts)
        {
            
        }

        private static string GenerateStoreZipFileName(string appName)
        {
            string fileName = Global.SavedataArchiveName;
            if (fileName.Contains("{NAME}"))
                fileName = fileName.Replace("{NAME}", appName);
            if (fileName.Contains("{TIME}"))
                fileName = fileName.Replace("{TIME}", DateTime.Now.ToString(Global.SavedataArchiveNameTimeFormat));
            return fileName;
        }

        private void RunRestore(RestoreOptions opts)
        {
            // current savedata is newer
            if (fsMaxLastWriteTime != null && fsMaxLastWriteTime > zipArchiveEntriesMaxLastWriteTime)
            {
                Console.Write("Current App savedata is newer than the one to restore, overwrite(Y/N): ");
                var overWrite = UserInput.ReadLineYN();
                if (overWrite == false)
                {
                    _log.Warn("User abort, exiting");
                    Environment.Exit(0);
                }
                _log.Warn("User approved, force overwrite app savedata");
            }
            _log.Debug("Current App savedata is not newer than the one to restore, overwrite");
        }
    }
}