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
            Log.Debug("RunStore", $"Setting CurrentDirectory to {workDir}");
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
                Log.Debug("RunStore/AddEntriesToZipArchive", entry.ToString());
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
            Log.Info("RunStore", "Store complete");
        }

        private static void RunRestore(RestoreOptions opts)
        {
            Log.Info("RunRestore", "Starting restore");

            string savedataArchivePath = opts.FilePath;
            Log.Debug("RunRestore", $"savedataArchivePath = {savedataArchivePath}");
            using var zipArchive = ZipFile.Open(savedataArchivePath, ZipArchiveMode.Read);

            string workDir = opts.DirPath;
            Log.Debug("RunRestore", $"workDir = {workDir}");
            Log.Debug("RunRestore", $"Setting CurrentDirectory to {workDir}");
            Directory.SetCurrentDirectory(workDir);

            var configEntry = zipArchive.GetEntry(Global.SavedataConfigFileName);
            using var configEntryStreamReader = new StreamReader(configEntry.Open(), Encoding.UTF8);
            string configStr = configEntryStreamReader.ReadToEnd();
            Log.Debug("RunRestore", $"configStr = {configStr}");
            var config = JsonSerializer.Deserialize<SavedataManagerConfig>(configStr);
            Log.Debug("RunRestore", "Starting config deserialization");
            if (config == null)
            {
                Log.Error("RunStore", "config is null");
                return;
            }
            Log.Debug("RunRestore", "Config deserialization finished");

            // extract entries from zipArchive
            foreach (var entry in config.Entries)
            {
                var extractPath = Path.GetDirectoryName(entry.Path);
                Log.Debug("RunRestore/ExtractFromZipArchive", $"extractPath = {extractPath}");
                var zipArchiveBaseDirName = entry.Id.ToString();
                var zipArchiveEntriesFiltered = from zipArchiveEntry in zipArchive.Entries
                                                where Path.GetDirectoryName(zipArchiveEntry.FullName).StartsWith(zipArchiveBaseDirName)
                                                where String.IsNullOrEmpty(zipArchiveEntry.Name) == false
                                                select zipArchiveEntry;
                // from https://stackoverflow.com/questions/36350199/c-sharp-extract-specific-directory-from-zip-preserving-folder-structure
                foreach (var zipArchiveEntry in zipArchiveEntriesFiltered)
                {
                    var name = zipArchiveEntry.FullName.Substring(zipArchiveBaseDirName.Length + 1);
                    Log.Debug("RunRestore/ExtractFromZipArchive", $"zipArchiveEntry.name = {name}");
                    string path = Path.Combine(extractPath, name);
                    Log.Debug("RunRestore/ExtractFromZipArchive", $"zipArchiveEntry.path = {path}");
                    if (Directory.Exists(path) == false)
                    {
                        Log.Debug("RunRestore/ExtractFromZipArchive", $"Creating dir: {Path.GetDirectoryName(path)}");
                        Directory.CreateDirectory(Path.GetDirectoryName(path));
                    }
                    zipArchiveEntry.ExtractToFile(path, true);
                }
            }

            // extract config
            var extractConfigPath = Path.Combine(workDir, Global.SavedataConfigFileName);
            Log.Debug("RunRestore", $"extractConfigPath = {extractConfigPath}");
            Log.Debug("RunRestore", "Extracting SavedataConfigFile from zipArchive");
            zipArchive.GetEntry(Global.SavedataConfigFileName).ExtractToFile(extractConfigPath, true);
            Log.Info("RunRestore", "Restore complete");
        }
    }
}