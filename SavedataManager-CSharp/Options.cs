using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace SavedataManager_CSharp
{
    interface IStoreOptions
    {
        [Option('s', "store", Default = false, SetName ="store")]
        bool DoStore { get; set; }
    }

    interface IRestoreOptions
    {
        [Option('r', "restore", Default = false, SetName = "restore")]
        bool DoRestore { get; set; }
    }

    internal class Options : IStoreOptions, IRestoreOptions
    {
        // store options
        public bool DoStore { get; set; }

        // restore options
        public bool DoRestore { get; set; }

        [Option('d', "dir", Required = true, HelpText = "")]
        public string DirPath { get; set; } = null!;
    }
}
