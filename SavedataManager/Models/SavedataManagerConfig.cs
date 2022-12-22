using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SavedataManager.Models
{
    public class SavedataManagerConfig
    {
        public List<Entry> Entries { get; set; } = null!;
    }
    public class Entry
    {
        public int Id { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public EntryType Type { get; set; }
        public string Path { get; set; } = null!;
        public string OriginalName { get; set; } = null!;

        public string GetRealPath()
        {
            string realPath = Path;
            if (Path.Contains("{USER_DOCUMENT}"))
                realPath = realPath.Replace("{USER_DOCUMENT}", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
            if (Path.Contains("{USER_PROFILE}"))
                realPath = realPath.Replace("{USER_PROFILE}", Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
            if (Path.Contains("{USER_SAVED_GAMES}"))
                realPath = realPath.Replace("{USER_SAVED_GAMES}", System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Saved Games"));
            return realPath;
        }
        public override string ToString()
        {
            return $"SavedataManagerConfig Entry {{ Id = {Id}, Type = {Type}, Path = {Path}, OriginalName = {OriginalName} }}";
        }
    }
    public enum EntryType
    {
        File,
        Folder
    }
}
