using CommandLine;
using log4net;
using SavedataManager.Utils;
using System.IO;

namespace SavedataManager
{
    class Program
    {
        // https://www.c-sharpcorner.com/article/log4net-and-net-core/
        private readonly ILog _log = LogManager.GetLogger(typeof(Program));
        private readonly TuiHub.SavedataManagerLibrary.SavedataManager _manager = new (LogManager.GetLogger(typeof(TuiHub.SavedataManagerLibrary.SavedataManager)));
        static void Main(string[] args)
        {
            var program = new Program();
            program.Run(args);
        }
        public void Run(string[] args)
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
                _log.Fatal($"{e.Message}", e);
                //_log.Debug($"{e.StackTrace}");
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

        private void RunStore(StoreOptions opts)
        {
            var appName = opts.AppName;
            var memoryStream = _manager.Store(opts.DirPath);
            if (memoryStream == null)
            {
                _log.Error("Store failed");
                return;
            }
            string zipFileName = GenerateStoreZipFileName(appName);
            _log.Info($"Savedata filename: {zipFileName}");
            string zipFilePath = Path.Combine(Global.SavedataArchiveFolderPath, zipFileName);
            _log.Debug($"Savedata filepath = {zipFilePath}");
            FileHelper.Write(memoryStream, zipFilePath);
            _log.Info("Store complete");
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
            var archivePath = opts.FilePath;
            var gameDirPath = opts.DirPath;
            var forceOverwrite = opts.Overwrite;
            // current savedata is newer
            if (forceOverwrite == false)
            {
                if (_manager.CheckFSLastWriteTimeNewer(archivePath) == true)
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
                else
                    _log.Debug("Current App savedata is not newer than the one to restore, overwrite");
            }
            else
                _log.Debug($"forceOverwrite is {forceOverwrite}, overwriting");
            var result = _manager.Restore(archivePath, gameDirPath, true);
            if (result == false)
            {
                _log.Error("Restore failed");
                return;
            }
        }
    }
}