using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TuiHub.SavedataManager.Models
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
        public string Id { get; set; } = String.Empty;
        public PathMode PathMode { get; set; }
        public string Path { get; set; } = String.Empty;
    }
    public enum PathMode
    {
        Absolute,
        Game,
        Document,
        Profile
    }
}
