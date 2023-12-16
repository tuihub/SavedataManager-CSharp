using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TuiHub.SavedataManagerLibrary.Models.V2_1;

namespace TuiHub.SavedataManagerLibrary.Services.V2_1.Utils
{
    public static class FSUtil
    {
        public static List<string> GetFSFilesFromEntry(Config config, Entry entry, string entryBaseDir)
        {
            var files = new List<string>();
            foreach (var filePattern in entry.FilePatterns!.Where(e => e.Type == FilePatternType.Include))
                files.AddRange(Directory.EnumerateFiles(entryBaseDir, filePattern.Pattern, new EnumerationOptions
                {
                    RecurseSubdirectories = true,
                    MatchCasing = config.IsCaseSensitve ? MatchCasing.CaseSensitive : MatchCasing.CaseInsensitive
                }));
            foreach (var filePattern in entry.FilePatterns!.Where(e => e.Type == FilePatternType.Exclude))
                files.RemoveAll(e => Regex.IsMatch(e, $"^{filePattern.Pattern}$",
                    config.IsCaseSensitve ? RegexOptions.None : RegexOptions.IgnoreCase));
            return files;
        }
    }
}
