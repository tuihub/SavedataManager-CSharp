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
                var jsonSchemaId = configStr.GetConfigSchemaId();
                object? config = jsonSchemaId switch
                {
                    "https://github.com/tuihub/protos/schemas/savedata/v1" =>
                        JsonSerializer.Deserialize<Models.V1.Config>(configStr, s_jsonSerializerOptions),
                    "https://github.com/tuihub/protos/schemas/savedata/v2.1.json" =>
                        null,
                    _ => throw new Exception($"{jsonSchemaId} not supported")
                };
                if (config == null)
                {
                    _logger?.LogError("config is null");
                    throw new Exception("config is null");
                }
                _logger?.LogDebug("Config deserialization finished");

                _logger?.LogDebug($"Restoring CurrentDirectory to {originWorkDir}");
                Directory.SetCurrentDirectory(originWorkDir);

                IService service = jsonSchemaId switch
                {
                    "https://github.com/tuihub/protos/schemas/savedata/v1" =>
                         new Services.V1.Service(_logger, s_savedataConfigFileName),
                    "https://github.com/tuihub/protos/schemas/savedata/v2.1.json" =>
                        null,
                    _ => throw new Exception($"{jsonSchemaId} not supported")
                };
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
