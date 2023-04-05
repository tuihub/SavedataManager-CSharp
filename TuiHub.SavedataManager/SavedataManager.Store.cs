using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TuiHub.SavedataManager.Models;

namespace TuiHub.SavedataManager
{
	public partial class SavedataManager
	{
		public MemoryStream Store(string archivePath, string gameDirPath)
		{
			Log.Info("RunStore", "Starting store");
			Log.Debug("RunStore", $"Setting CurrentDirectory to {gameDirPath}");
			Directory.SetCurrentDirectory(gameDirPath);
			string configPath = Path.Combine(Environment.CurrentDirectory, s_savedataConfigFileName);
			Log.Debug("RunStore", $"configPath = {configPath}");
			string configStr = File.ReadAllText(configPath, Encoding.UTF8);
			Log.Debug("RunStore", $"configStr = {configStr}");
			Log.Debug("RunStore", "Starting config deserialization");
			var config = JsonSerializer.Deserialize<Config>(configStr, s_jsonSerializerOptions);
			if (config == null)
			{
				Log.Error("RunStore", "config is null");
				return;
			}
			Log.Debug("RunStore", "Config deserialization finished");

			// create ZipArchive using MemoryStream
			var memoryStream = new MemoryStream();
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
			Log.Info("RunStore", "Returning memoryStream");
			return memoryStream;
		}
	}
}
