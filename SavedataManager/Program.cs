using CommandLine;
using SavedataManager;
using SavedataManager.Models;
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
        static void Main(string[] args)
        {
            try
            {
                Log.Debug("Main", "Starting program");
                Parser.Default.ParseArguments<StoreOptions, RestoreOptions>(args)
                    .WithParsed<StoreOptions>(RunStore)
                    .WithParsed<RestoreOptions>(RunRestore)
                    .WithNotParsed(HandleParseError);
            }
            catch (Exception e)
            {
                Log.Error("Main", $"{e.Message}");
                Log.Debug("Main", $"{e.StackTrace}");
            }
        }

        private static void HandleParseError(IEnumerable<Error> errs)
        {
            if (errs.IsVersion())
            {
                Log.Debug("HandleParseError", "Running version");
                return;
            }
            if (errs.IsHelp())
            {
                Log.Debug("HandleParseError", "Running version");
                return;
            }
        }

        private static void RunStore(StoreOptions opts)
        {
            Log.Info("RunStore", "Starting store");
            string workDir = opts.DirPath;
            Log.Debug("RunStore", $"workDir = {workDir}");
            Directory.SetCurrentDirectory(workDir);
            string configPath = Path.Combine(workDir, Global.SavedataConfigFileName);
            Log.Debug("RunStore", $"configPath = {configPath}");
            string configStr = File.ReadAllText(configPath, Encoding.UTF8);
            Log.Debug("RunStore", $"configStr = {configStr}");
            Log.Debug("RunStore", "Starting config deserialization");
            var config = JsonSerializer.Deserialize<SavedataManagerConfig>(configStr);
            if (config == null)
            {
                Log.Error("RunStore", "config is null");
                return;
            }
            Log.Debug("RunStore", "Config deserialization finished");

            // create ZipArchive using MemoryStream
            using MemoryStream memoryStream = new MemoryStream();
            // leaveOpen must be true
            using ZipArchive zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Update, true);

            // add entries to zipArchive
            foreach (var entry in config.Entries)
            {
                Log.Debug("RunStore/AddEntriesToZipActhive", entry.ToString());
                zipArchive.CreateEntryFromAny(entry.Path, entry.Id.ToString());
            }

            // add config.json
            Log.Debug("RunStore", "Adding SavedataConfigFile to zipArchive");
            zipArchive.CreateEntryFromAny(configPath);

            // must dispose
            zipArchive.Dispose();
            long curTimeMs = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            string zipFileName = "Savedata_" + curTimeMs.ToString() + ".zip";
            Log.Info("RunStore", $"Savedata filename: {zipFileName}");
            string zipFilePath = Path.Combine(Global.SavedataArchiveFolderPath, zipFileName);
            Log.Debug("RunStore", $"Savedata filepath = {zipFilePath}");
            FileHelper.Write(memoryStream, zipFilePath);
        }

        private static void RunRestore(RestoreOptions opts)
        {
            Log.Info("RunRestore", "Starting restore");
        }
    }
}