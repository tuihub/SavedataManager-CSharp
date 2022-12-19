using CommandLine;
using SavedataManager;
using SavedataManager.Utils;
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
            if (errs.IsVersion())
            {
                Log.Debug("HandleParseError", "Running version");
                return;
            }
            if (errs.IsHelp())
            {
                Log.Debug("HandleParseError", "Running version");
                return;
            }
        }

        private static void RunStore(StoreOptions opts)
        {
            Log.Debug("RunStore", "Starting store");
        }

        private static void RunRestore(RestoreOptions opts)
        {
            Log.Debug("RunRestore", "Starting restore");
        }
    }
}