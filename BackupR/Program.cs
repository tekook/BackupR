using CommandLine;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tekook.BackupR.Lib.StateManagement;
using Tekook.BackupR.Verbs;

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

#pragma warning disable CS1998 // Bei der asynchronen Methode fehlen "await"-Operatoren. Die Methode wird synchron ausgeführt.

        private static async Task Main(string[] args)
#pragma warning restore CS1998 // Bei der asynchronen Methode fehlen "await"-Operatoren. Die Methode wird synchron ausgeführt.
        {
            Parser.Default.ParseArguments<CleanupOptions, BackupOptions>(args)
                .WithParsed<CleanupOptions>(o => new CleanupVerb(o).Invoke())
                .WithParsed<BackupOptions>(o => new BackupVerb(o).Invoke())
                .WithNotParsed(HandleParseError);
            if (StateManager.Instance != null)
            {
                StateManager.Save();
            }
        }
    }
}