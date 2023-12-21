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
        public static void Convert(string gamePath, ref string path, out BaseDirMode baseDirMode)
        {
            var myDocumentPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var userProfilePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            if (path.StartsWith(gamePath) == true)
            {
                baseDirMode = BaseDirMode.GameRoot;
                path = path.Replace(gamePath, ".");
            }
            else if (path.StartsWith(myDocumentPath) == true)
            {
                baseDirMode = BaseDirMode.UserDocument;
                path = path.Replace(myDocumentPath, ".");
            }
            else if (path.StartsWith(userProfilePath) == true)
            {
                baseDirMode = BaseDirMode.UserProfile;
                path = path.Replace(userProfilePath, ".");
            }
            else
            {
                baseDirMode = BaseDirMode.Absolute;
            }
        }
    }
}
