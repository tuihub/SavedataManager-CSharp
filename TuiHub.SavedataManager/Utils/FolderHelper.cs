using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SavedataManager.Utils
{
    public static class FolderHelper
    {
        // from https://stackoverflow.com/questions/7911448/c-get-first-directory-name-of-a-relative-path
        public static string GetRootFolder(string path)
        {
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
