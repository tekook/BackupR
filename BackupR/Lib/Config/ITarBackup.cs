using System.Collections.Generic;

namespace Tekook.BackupR.Lib.Config
{
    public interface ITarBackup : IBackup
    {
        public bool Compress { get; }
        public IEnumerable<string> Excludes { get; }
        public string Path { get; }
        public string AdditionalOptions { get; }
    }
}