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

        public string GetRealPath()
        {
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
