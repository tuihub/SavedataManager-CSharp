using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TuiHub.SavedataManagerLibrary.Models
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
        public long Id { get; set; } = -1;
        public PathMode PathMode { get; set; }
        public string Path { get; set; } = String.Empty;

        public string GetRealPath()
        {
            var tempPath = Path;
            if (Path.EndsWith(System.IO.Path.DirectorySeparatorChar) ||
                Path.EndsWith(System.IO.Path.AltDirectorySeparatorChar))
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
