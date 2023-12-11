namespace TuiHub.SavedataManagerLibrary.Models.V2_1
{
    public class Config
    {
        public Platform Platform { get; set; }
        public List<Entry>? Entries { get; set; }
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
