using Microsoft.Extensions.Logging;
using System.IO.Compression;
using System.Text.Json;
using TuiHub.SavedataManagerLibrary.Contracts;
using TuiHub.SavedataManagerLibrary.Utils;

namespace TuiHub.SavedataManagerLibrary
{
    public partial class SavedataManager
    {
        public bool CheckFSLastWriteTimeNewer(Stream archiveStream, string gameDirPath)
        {
            string originWorkDir = Directory.GetCurrentDirectory();
            _logger?.LogDebug($"originWorkDir = {originWorkDir}");

            try
            {
                string workDir = gameDirPath;
                _logger?.LogDebug($"workDir = {workDir}");
                _logger?.LogDebug($"Setting CurrentDirectory to {workDir}");
                Directory.SetCurrentDirectory(workDir);
                using var zipArchive = new ZipArchive(archiveStream, ZipArchiveMode.Read);
                var configEntry = zipArchive.GetEntry(s_savedataConfigFileName);
                if (configEntry == null)
                {
                    _logger?.LogError("configEntry is null");
                    throw new Exception("configEntry is null");
                }
                using var configEntryStreamReader = new StreamReader(configEntry.Open(), s_UTF8WithoutBom);
                string configStr = configEntryStreamReader.ReadToEnd();
                _logger?.LogDebug($"configStr = {configStr}");
                _logger?.LogDebug("Starting config validation");
                var validation = Validate(configStr);
                if (validation == false)
                {
                    _logger?.LogError("Savedata config validation failed");
                    throw new Exception("Savedata config validation failed");
                }
                _logger?.LogDebug("Validation finished");
                _logger?.LogDebug("Starting config deserialization");
                var config = configStr.GetConfigObj(s_jsonSerializerOptions);
                _logger?.LogDebug("Config deserialization finished");

                _logger?.LogDebug($"Restoring CurrentDirectory to {originWorkDir}");
                Directory.SetCurrentDirectory(originWorkDir);

                var service = configStr.GetIService(s_savedataConfigFileName, _logger);
                return service.CheckFSLastWriteTimeNewer(config, zipArchive);
            }
            catch
            {
                throw;
            }
            finally
            {
                // ensure restore working directory
                _logger?.LogDebug($"Restoring CurrentDirectory to {originWorkDir}");
                Directory.SetCurrentDirectory(originWorkDir);
            }
        }
    }
}
