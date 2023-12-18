using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuiHub.SavedataManagerLibrary.Models.V2_1;
using TuiHub.SavedataManagerLibrary.Services.V2_1.Utils;

namespace TuiHub.SavedataManagerLibrary.Services.V2_1
{
    public partial class Service
    {
        public bool Restore(object configObj, ZipArchive zipArchive, string gameDir, bool forceOverwrite = false)
        {
            Config config = (configObj as Config)!;
            if (config.Entries == null || !config.Entries.Any())
                throw new ArgumentException("config.Entries is null or empty");
            // check fs savedate last write time if forceOverwrite == false
            if (forceOverwrite == false)
            {
                bool fsLastWriteTimeNewer = CheckFSLastWriteTimeNewer(config, zipArchive, gameDir);
                if (fsLastWriteTimeNewer == true)
                {
                    _logger?.LogInformation("fs savedata is newer, not overwriting");
                    return false;
                }
                _logger?.LogInformation("fs savedata is not newer, overwriting");
            }
            // delete folders where entry ClearBaseDirBeforeRestore == true
            foreach (var entry in config.Entries.Where(e => e.ClearBaseDirBeforeRestore))
            {
                var dirPath = entry.GetRealBaseDir(gameDir);
                _logger?.LogDebug($"Clearing dir: {dirPath}");
                Directory.Delete(dirPath, true);
                Directory.CreateDirectory(dirPath);
            }
            // delete files exlcuding patterns FilePatternType.Exclude
            foreach (var entry in config.Entries.Where(e => !e.ClearBaseDirBeforeRestore))
            {
                var entryBaseDir = entry.GetRealBaseDir(gameDir);
                var files = FSUtil.GetFSFilesFromEntry(config, entry, entryBaseDir);
                foreach (var file in files)
                {
                    _logger?.LogDebug($"Deleting file: {file}");
                    File.Delete(file);
                }
            }
            // restore
            foreach (var entry in config.Entries)
            {
                var entryId = entry.Id.ToString();
                var zipArchiveEntries = zipArchive.Entries
                                                  .Where(e => !string.IsNullOrEmpty(e.Name))
                                                  .Where(e => e.FullName.StartsWith(entryId));
                if (!zipArchiveEntries.Any())
                {
                    _logger?.LogWarning($"zipArchiveEntries is empty, entryId = {entryId}");
                    continue;
                }
                var baseDir = entry.GetRealBaseDir(gameDir);
                foreach (var e in zipArchiveEntries)
                {
                    var relativePath = e.FullName.Substring(entryId.Length + 1);
                    var extractPath = Path.Combine(baseDir, relativePath);
                    if (!Directory.Exists(Path.GetDirectoryName(extractPath)))
                    {
                        _logger?.LogDebug($"Creating dir: {Path.GetDirectoryName(extractPath)}");
                        Directory.CreateDirectory(Path.GetDirectoryName(extractPath)!);
                    }
                    if (File.Exists(extractPath))
                    {
                        _logger?.LogWarning($"{extractPath} exists.");
                    }
                    else
                    {
                        _logger?.LogDebug($"Extracting zipArchiveEntry {e.FullName} to {extractPath}");
                        e.ExtractToFile(extractPath, false);
                    }
                }
            }
            return true;
        }
    }
}
