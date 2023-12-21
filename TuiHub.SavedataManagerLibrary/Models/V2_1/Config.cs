using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Text.Unicode;
using TuiHub.SavedataManagerLibrary.Properties;

namespace TuiHub.SavedataManagerLibrary.Models.V2_1
{
    public class Config
    {
        public static JsonSerializerOptions JsonSerializerOptions { get => new()
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
            Converters =
            {
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
            },
            // not converting to ASCII char
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
        }; }
        public static string JsonSchemaStr {  get => Resources.JsonSchemaV2_1Str; }
        public Platform Platform { get; set; }
        public List<Entry>? Entries { get; set; }
        public bool CaseSensitive { get; set; }
    }
    public enum Platform
    {
        Windows
    }

    public class Entry
    {
        public long Id { get; set; }
        public BaseDirMode BaseDirMode { get; set; }
        public string BaseDir { get; set; } = string.Empty;
        public List<FilePattern>? FilePatterns { get; set; }
        public bool ClearBaseDirBeforeRestore { get; set; }
        public string GetRealBaseDir(string? gameDir = null)
        {
            return BaseDirMode switch
            {
                BaseDirMode.GameRoot => Path.Combine(gameDir ?? string.Empty, BaseDir),
                BaseDirMode.UserDocument => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), BaseDir),
                BaseDirMode.UserProfile => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), BaseDir),
                BaseDirMode.Absolute => BaseDir,
                _ => throw new ArgumentException("Invalid BaseDirMode")
            };
        }
    }

    public enum BaseDirMode
    {
        GameRoot,
        UserDocument,
        UserProfile,
        Absolute
    }

    public class FilePattern
    {
        public FilePatternType Type { get; set; }
        public string Pattern { get; set; } = string.Empty;
    }

    public enum FilePatternType
    {
        Include,
        Exclude
    }
}
