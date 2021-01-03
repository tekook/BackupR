using CommandLine;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tekook.BackupR.Test;

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
            Parser.Default.ParseArguments<TestOptions>(args)
                .WithParsed<TestOptions>(o => (new TestVerb(o)).Invoke())
                .WithNotParsed(HandleParseError);
        }
    }
}