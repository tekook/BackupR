namespace Tekook.BackupR.Lib.Config
{
    public interface ICommandBackup : IBackup
    {
        public string Command { get; }
        public string OutputPath { get; }
        public string WorkingDirectory { get; }
    }
}