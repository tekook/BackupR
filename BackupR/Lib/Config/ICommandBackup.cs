namespace Tekook.BackupR.Lib.Config
{
    public interface ICommandBackup : IBackup
    {
        public string Arguments { get; }
        public string Command { get; }
        public string WorkingDirectory { get; }
    }
}