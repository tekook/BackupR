using CommandLine;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tekook.BackupR.Verbs.Cleanup;

namespace Tekook.BackupR
{
    internal class Program
    {
        private static void HandleParseError(IEnumerable<Error> obj)
        {
            obj.Output();
            if (obj.IsHelp() || obj.IsVersion())
            {
                Environment.Exit(0);
            }
            else
            {
                Environment.Exit(1);
            }
        }

        private static async Task Main(string[] args)
        {
            Parser.Default.ParseArguments<CleanupOptions>(args)
                .WithParsed(o => (new CleanupVerb(o)).Invoke())
                .WithNotParsed(HandleParseError);
        }
    }
}