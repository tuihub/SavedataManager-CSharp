using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Models
{
    public class Config
    {
        [JsonPropertyName("$schema")]
        public string Schema { get; set; } = "https://tuihub.github.io/protos/schemas/savedata/v2.1.json";
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
