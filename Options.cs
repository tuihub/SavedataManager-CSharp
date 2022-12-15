using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace SavedataManager
{
    [Verb("store", isDefault: true, aliases: new string[] { "s" })]
    class StoreOptions
    {
        [Option('d', "dir", Default = ".", HelpText = "App base folder path to store savedata.")]
        public string DirPath { get; set; } = null!;
    }

    [Verb("restore", aliases: new string[] { "rs" })]
    class RestoreOptions
    {
        [Option('f', "file", Required = true, HelpText = "Savedata file path to restore.")]
        public string FilePath { get; set; } = null!;
        [Option('d', "dir", Default = ".", HelpText = "App base folder path to restore savedata.")]
        public string DirPath { get; set; } = null!;
    }
}
