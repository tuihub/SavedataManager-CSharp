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
            string appName = opts.AppName;
            Log.Debug("RunStore", $"appName = {appName}");
            string workDir = opts.DirPath;
            Log.Debug("RunStore", $"workDir = {workDir}");
            Log.Debug("RunStore", $"Setting CurrentDirectory to {workDir}");
            Directory.SetCurrentDirectory(workDir);
            string configPath = Path.Combine(Environment.CurrentDirectory, Global.SavedataConfigFileName);
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
                Log.Debug("RunStore/AddEntriesToZipArchive", $"entry.GetRealPath() = {entry.GetRealPath()}");
                zipArchive.CreateEntryFromAny(entry.GetRealPath(), entry.Id.ToString());
            }

            // add config.json
            Log.Debug("RunStore", "Adding SavedataConfigFile to zipArchive");
            zipArchive.CreateEntryFromAny(configPath);

            // must dispose
            zipArchive.Dispose();
            long curTimeMs = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            string zipFileName = "Savedata_" + (appName.Length > 0 ? (appName + "_") : "") + curTimeMs.ToString() + ".zip";
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
            var deleteFolderContent = opts.Delete;
            Log.Debug("RunRestore", $"deleteFolderContent = {deleteFolderContent}");
            var deletedFolder = new HashSet<string>();

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

            // compare LastWriteTime
            var zipArchiveEntriesMaxLastWriteTime = zipArchive.GetEntriesMaxLastWriteTime();
            DateTime? fsMaxLastWriteTime = null;
            foreach (var entry in config.Entries)
            {
                Log.Debug("RunRestore/LastWriteTime", $"{entry}");
                if (entry.Type == EntryType.File)
                {
                    Log.Debug("RunRestore/LastWriteTime", $"Checking file: {entry.GetRealPath()}");
                    var curFileLastWriteTime = File.GetLastWriteTime(entry.GetRealPath());
                    Log.Debug("RunRestore/LastWriteTime", $"curFileLastWriteTime = {curFileLastWriteTime}");
                    Log.Debug("RunRestore/LastWriteTime", $"fsMaxLastWriteTime = {fsMaxLastWriteTime}");
                    if (fsMaxLastWriteTime == null || curFileLastWriteTime > fsMaxLastWriteTime)
                    {
                        Log.Debug("RunRestore/LastWriteTime", $"Updating fsMaxLastWriteTime = {curFileLastWriteTime}");
                        fsMaxLastWriteTime = curFileLastWriteTime;
                    }
                }
                else if (entry.Type == EntryType.Folder)
                {
                    var files = Directory.GetFiles(entry.GetRealPath(), "*", SearchOption.AllDirectories);
                    foreach (var file in files)
                    {
                        Log.Debug("RunRestore/LastWriteTime", $"Checking file: {file}");
                        var curFileLastWriteTime = File.GetLastWriteTime(file);
                        Log.Debug("RunRestore/LastWriteTime", $"curFileLastWriteTime = {curFileLastWriteTime}");
                        Log.Debug("RunRestore/LastWriteTime", $"fsMaxLastWriteTime = {fsMaxLastWriteTime}");
                        if (fsMaxLastWriteTime == null || curFileLastWriteTime > fsMaxLastWriteTime)
                        {
                            Log.Debug("RunRestore/LastWriteTime", $"Updating fsMaxLastWriteTime = {curFileLastWriteTime}");
                            fsMaxLastWriteTime = curFileLastWriteTime;
                        }
                    }
                }
            }
            // current savedata is newer
            if (fsMaxLastWriteTime != null && fsMaxLastWriteTime > zipArchiveEntriesMaxLastWriteTime)
            {
                Console.WriteLine("Current App savedata is newer than the one to restore, overwrite(Y/N): ");
                var overWrite = UserInput.ReadLineYN();
                if (overWrite == false)
                {
                    Log.Warn("RunRestore/LastWriteTime", "User abort, exiting");
                    Environment.Exit(0);
                }
                Log.Warn("RunRestore/LastWriteTime", "User approved, force overwrite app savedata");
            }
            Log.Debug("RunRestore/LastWriteTime", "Current App savedata is not newer than the one to restore, overwrite");

            // extract entries from zipArchive
            foreach (var entry in config.Entries)
            {
                var extractPath = Path.GetDirectoryName(entry.GetRealPath());
                Log.Debug("RunRestore/ExtractFromZipArchive", $"entry.GetRealPath() = {entry.GetRealPath()}");
                Log.Debug("RunRestore/ExtractFromZipArchive", $"extractPath = {extractPath}");
                var zipArchiveBaseDirName = entry.Id.ToString();
                var zipArchiveEntriesFiltered = from zipArchiveEntry in zipArchive.Entries
                                                where Path.GetDirectoryName(zipArchiveEntry.FullName).StartsWith(zipArchiveBaseDirName)
                                                where String.IsNullOrEmpty(zipArchiveEntry.Name) == false
                                                select zipArchiveEntry;
                if (deleteFolderContent && entry.Type == EntryType.Folder)
                {
                    if (zipArchiveEntriesFiltered.Count() == 0)
                    {
                        Log.Warn("RunRestore/ExtractFromZipArchive.DeleteFolderContent", $"Empty dir in entry: {entry}");
                    }
                    else
                    {
                        var e = zipArchiveEntriesFiltered.First();
                        var folder = FolderHelper.GetRootFolder(e.FullName.Substring(zipArchiveBaseDirName.Length + 1));
                        var path = Path.Combine(extractPath, folder);
                        if (deletedFolder.Contains(path))
                        {
                            Log.Debug("RunRestore/ExtractFromZipArchive.DeleteFolderContent", $"Dir {path} have been deleted");
                        }
                        else
                        {
                            deletedFolder.Add(path);
                            if (Directory.Exists(path) == false)
                            {
                                Log.Debug("RunRestore/ExtractFromZipArchive.DeleteFolderContent", $"Dir {path} not exists");
                            }
                            else
                            {
                                Log.Warn("RunRestore/ExtractFromZipArchive.DeleteFolderContent", $"Deleting dir: {path}");
                                Directory.Delete(path, true);
                                Directory.CreateDirectory(path);
                            }
                        }
                    }
                }
                // from https://stackoverflow.com/questions/36350199/c-sharp-extract-specific-directory-from-zip-preserving-folder-structure
                foreach (var zipArchiveEntry in zipArchiveEntriesFiltered)
                {
                    var name = zipArchiveEntry.FullName.Substring(zipArchiveBaseDirName.Length + 1);
                    Log.Debug("RunRestore/ExtractFromZipArchive", $"zipArchiveEntry.name = {name}");
                    string path = Path.Combine(extractPath, name);
                    Log.Debug("RunRestore/ExtractFromZipArchive", $"zipArchiveEntry.path = {path}");
                    if (Directory.Exists(Path.GetDirectoryName(path)) == false)
                    {
                        Log.Debug("RunRestore/ExtractFromZipArchive", $"Creating dir: {Path.GetDirectoryName(path)}");
                        Directory.CreateDirectory(Path.GetDirectoryName(path));
                    }
                    zipArchiveEntry.ExtractToFile(path, true);
                }
            }

            // extract config
            var extractConfigPath = Path.Combine(Environment.CurrentDirectory, Global.SavedataConfigFileName);
            Log.Debug("RunRestore", $"extractConfigPath = {extractConfigPath}");
            Log.Debug("RunRestore", "Extracting SavedataConfigFile from zipArchive");
            zipArchive.GetEntry(Global.SavedataConfigFileName).ExtractToFile(extractConfigPath, true);
            Log.Info("RunRestore", "Restore complete");
        }
    }
}