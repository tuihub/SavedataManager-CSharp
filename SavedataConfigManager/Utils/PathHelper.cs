using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SavedataConfigManager.Utils
{
    internal class PathHelper
    {
        public static void Convert(string gamePath, ref string path, out PathMode pathMode)
        {
            var myDocumentPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var userProfilePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            if (path.StartsWith(gamePath) == true)
            {
                pathMode = PathMode.Game;
                path = path.Replace(gamePath, ".");
            }
            else if (path.StartsWith(myDocumentPath) == true)
            {
                pathMode = PathMode.Document;
                path = path.Replace(myDocumentPath, ".");
            }
            else if (path.StartsWith(userProfilePath) == true)
            {
                pathMode = PathMode.Profile;
                path = path.Replace(userProfilePath, ".");
            }
            else
            {
                pathMode = PathMode.Absolute;
            }
        }
    }
}
