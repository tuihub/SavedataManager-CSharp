﻿using TuiHub.SavedataManagerLibrary.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TuiHub.SavedataManagerLibrary.Models;

namespace TuiHub.SavedataManagerLibrary
{
    public partial class SavedataManager
    {
        public MemoryStream? Store(string gameDirPath)
        {
            _log.Info("Starting store");
            _log.Debug($"Setting CurrentDirectory to {gameDirPath}");
            Directory.SetCurrentDirectory(gameDirPath);
            string configPath = Path.Combine(Environment.CurrentDirectory, s_savedataConfigFileName);
            _log.Debug($"configPath = {configPath}");
            string configStr = File.ReadAllText(configPath, s_UTF8WithoutBom);
            _log.Debug($"configStr = {configStr}");
            _log.Debug("Starting config validation");
            var validation = Validate(configStr);
            if (validation == false)
            {
                _log.Error("Savedata config validation failed");
                throw new Exception("Savedata config validation failed");
            }
            _log.Debug("Validation finished");
            _log.Debug("Starting config deserialization");
            var config = JsonSerializer.Deserialize<Config>(configStr, s_jsonSerializerOptions);
            if (config == null)
            {
                _log.Error("config is null");
                throw new Exception("config is null");
            }
            _log.Debug("Config deserialization finished");

            // create ZipArchive using MemoryStream
            var memoryStream = new MemoryStream();
            // leaveOpen must be true
            using ZipArchive zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Update, true);

            // add entries to zipArchive
            if (config.Entries == null)
                _log.Warn("config.Entries is null");
            else
                foreach (Entry entry in config.Entries)
                {
                    _log.Debug($"AddEntriesToZipArchive entry = {entry.ToString()}");
                    _log.Debug($"AddEntriesToZipArchive entry.GetRealPath() = {entry.GetRealPath()}");
                    zipArchive.CreateEntryFromAny(entry.GetRealPath(), entry.Id.ToString());
                }

            // add config.json
            _log.Debug("Adding SaveDataConfigFile to zipArchive");
            zipArchive.CreateEntryFromAny(configPath);

            // must dispose
            zipArchive.Dispose();
            _log.Info("Returning memoryStream");
            return memoryStream;
        }
    }
}
