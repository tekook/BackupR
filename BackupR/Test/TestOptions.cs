using CommandLine;
using Tekook.VerbR.Contracts;

namespace Tekook.BackupR.Test
{
    [Verb("test", isDefault: true)]
    public class TestOptions : ICanValidateOnly
    {
        [Option('c', "config", Required = true)]
        public string Config { get; set; }

        [Option("validation-only", Default = false)]
        public bool ValidationOnly { get; set; }
    }
}