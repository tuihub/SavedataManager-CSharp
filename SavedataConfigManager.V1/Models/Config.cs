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
        public string Schema { get; set; } = "https://tuihub.github.io/protos/schemas/savedata/v1.json";
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
            var tempPath = Path;
            if (Path.EndsWith('/') || Path.EndsWith('\\'))
                tempPath = tempPath.Remove(tempPath.Length - 1);
            if (PathMode == PathMode.Document)
                return System.IO.Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    tempPath);
            else if (PathMode == PathMode.Profile)
                return System.IO.Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                    tempPath);
            else return tempPath;
        }

        public EntryFSType GetFSType()
        {
            if (Path.EndsWith('/') || Path.EndsWith('\\'))
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
