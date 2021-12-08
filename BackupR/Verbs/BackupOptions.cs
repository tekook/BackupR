using CommandLine;
using System;
using System.IO;
using Tekook.BackupR.Lib.Contracts;
using Tekook.VerbR.Contracts;

namespace Tekook.BackupR.Verbs
{
    [Verb("backup", HelpText = "Starts the backup process.")]
    internal class BackupOptions : ICanValidateOnly, IOptions
    {
        private string config;

        [Option('c', "config", Required = true)]
        public string Config
        {
            get => config; set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException(nameof(Config));
                }
                if (!File.Exists(value))
                {
                    throw new ArgumentException("Config file does not exist!", nameof(Config));
                }
                config = value;
            }
        }

        [Option("validation-only", Default = false)]
        public bool ValidationOnly { get; set; }
    }
}