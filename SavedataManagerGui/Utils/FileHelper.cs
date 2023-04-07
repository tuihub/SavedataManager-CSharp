using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SavedataManagerGui.Utils
{
    public static class FileHelper
    {
        public static void Write(MemoryStream memoryStream, string path)
        {
            using var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read, 65536);
            memoryStream.Seek(0, SeekOrigin.Begin);
            // force flush after seek
            memoryStream.Flush();
            memoryStream.CopyTo(fileStream);
            fileStream.Close();
            memoryStream.Close();
        }
    }
}
