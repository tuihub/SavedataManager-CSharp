using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Text.Unicode;
using TuiHub.SavedataManagerLibrary.Properties;

namespace TuiHub.SavedataManagerLibrary.Models.V1
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
        public static string JsonSchemaStr { get => JsonSchemaRes.V1Schema; }
        public Platform Platform { get; set; }
        public List<Entry>? Entries { get; set; }
    }
    public enum Platform
    {
        Windows
    }
    public class Entry
    {
        public long Id { get; set; } = -1;
        public PathMode PathMode { get; set; }
        public string Path { get; set; } = string.Empty;

        public string GetRealPath()
        {
            // not remove last folder in path
            //var tempPath = Path;
            //if (tempPath.EndsWith(System.IO.Path.DirectorySeparatorChar) ||
            //    tempPath.EndsWith(System.IO.Path.AltDirectorySeparatorChar))
            //    tempPath = tempPath.Remove(tempPath.Length - 1);
            if (PathMode == PathMode.Document)
                return System.IO.Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    Path);
            else if (PathMode == PathMode.Profile)
                return System.IO.Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                    Path);
            else return Path;
        }

        public EntryFSType GetFSType()
        {
            if (Path.EndsWith(System.IO.Path.DirectorySeparatorChar) ||
                Path.EndsWith(System.IO.Path.AltDirectorySeparatorChar))
                return EntryFSType.Folder;
            else return EntryFSType.File;
        }

        public override string ToString()
        {
            return $"Config Entry {{ Id = {Id}, PathMode = {PathMode} Path = {Path} }}";
        }
    }
    public enum PathMode
    {
        Absolute,
        Game,
        Document,
        Profile
    }
    public enum EntryFSType
    {
        File,
        Folder
    }
}
