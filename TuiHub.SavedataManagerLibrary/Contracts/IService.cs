using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuiHub.SavedataManagerLibrary.Contracts
{
    public interface IService
    {
        public bool CheckFSLastWriteTimeNewer(object configObj, ZipArchive zipArchive, string gameDir);
        public bool Restore(object configObj, ZipArchive zipArchive, string gameDir, bool forceOverwrite = false);
        public void Store(object configObj, Stream stream, string gameDir, string configPath);
    }
}
