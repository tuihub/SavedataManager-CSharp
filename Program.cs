using CommandLine;
using SavedataManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SavedataManager
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<StoreOptions, RestoreOptions>(args)
                .WithParsed<StoreOptions>(RunStore)
                .WithParsed<RestoreOptions>(RunRestore)
                .WithNotParsed(HandleParseError);
        }

        private static void HandleParseError(IEnumerable<Error> errs)
        {
            if (errs.IsVersion()) return;
            if (errs.IsHelp()) return;
        }

        private static void RunStore(StoreOptions opts)
        {
            Console.WriteLine("run store");
        }

        private static void RunRestore(RestoreOptions obj)
        {
            Console.WriteLine("run restore");
        }
    }
}