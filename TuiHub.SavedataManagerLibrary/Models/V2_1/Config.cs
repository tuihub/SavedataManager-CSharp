namespace TuiHub.SavedataManagerLibrary.Models.V2_1
{
    public class Config
    {
        public Platform Platform { get; set; }
        public List<Entry>? Entries { get; set; }
        public bool IsCaseSensitve
        {
            get
            {
                return Platform switch
                {
                    Platform.Windows => false,
                    _ => throw new ArgumentException("Invalid Platform")
                };
            }
        }
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
        public string GetRealBaseDir(string gameDir)
        {
            return BaseDirMode switch
            {
                BaseDirMode.GameRoot => Path.Combine(gameDir, BaseDir),
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
