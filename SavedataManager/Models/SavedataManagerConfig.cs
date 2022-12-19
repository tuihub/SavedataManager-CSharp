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
