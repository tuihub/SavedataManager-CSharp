﻿namespace TuiHub.SavedataManagerLibrary.Utils
{
    public static class FolderHelper
    {
        // from https://stackoverflow.com/questions/7911448/c-get-first-directory-name-of-a-relative-path
        public static string? GetRootFolder(string? path)
        {
            if (path == null)
                return null;
            if (path.Contains(System.IO.Path.DirectorySeparatorChar) == false &&
                path.Contains(System.IO.Path.AltDirectorySeparatorChar) == false)
                return ".";
            var root = Path.GetPathRoot(path);
            while (true)
            {
                var temp = Path.GetDirectoryName(path);
                if (temp != null && temp.Equals(root))
                    break;
                path = temp;
            }
            return path;
        }
    }
}
