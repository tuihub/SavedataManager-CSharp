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
        public bool CheckFSLastWriteTimeNewer(object configObj, ZipArchive zipArchive);
        public bool Restore(object configObj, ZipArchive zipArchive, bool forceOverwrite = false);
    }
}
