using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SavedataManager.Utils
{
    // from https://stackoverflow.com/questions/15133626/creating-directories-in-a-ziparchive-c-sharp-net-4-5
    public static class ZipArchiveHelper
    {
        public static void CreateEntryFromAny(this ZipArchive archive, string sourceName, string entryName = "")
        {
            var fileName = Path.GetFileName(sourceName);
            if (File.GetAttributes(sourceName).HasFlag(FileAttributes.Directory))
            {
                archive.CreateEntryFromDirectory(sourceName, Path.Combine(entryName, fileName));
            }
            else
            {
                archive.CreateEntryFromFile(sourceName, Path.Combine(entryName, fileName));
            }
        }

        public static void CreateEntryFromDirectory(this ZipArchive archive, string sourceDirName, string entryName = "")
        {
            var files = Directory.EnumerateFileSystemEntries(sourceDirName);
            foreach (var file in files)
            {
                archive.CreateEntryFromAny(file, entryName);
            }
        }

        public static DateTime GetEntriesMaxLastWriteTime(this ZipArchive archive)
        {
            var lastWriteTime = archive.Entries.First().LastWriteTime;
            foreach (var entry in archive.Entries)
            {
                if (entry.FullName == Global.SavedataConfigFileName)
                    continue;
                if (entry.LastWriteTime > lastWriteTime)
                    lastWriteTime = entry.LastWriteTime;
            }
            return DateTimeOffsetHelper.ConvertFromDateTimeOffset(lastWriteTime);
        }
    }
}
