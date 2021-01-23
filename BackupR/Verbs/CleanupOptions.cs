using CommandLine;
using Tekook.BackupR.Lib.Contracts;
using Tekook.VerbR.Contracts;

namespace Tekook.BackupR.Verbs
{
    [Verb("cleanup", true)]
    internal class CleanupOptions : ICanValidateOnly, IOptions
    {
        [Option('c', "config", Required = true)]
        public string Config { get; set; }

        [Option("validation-only", Default = false)]
        public bool ValidationOnly { get; set; }
    }
}