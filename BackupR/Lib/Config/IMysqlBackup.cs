namespace Tekook.BackupR.Lib.Config
{
    public interface IMysqlBackup : IBackup
    {
        public string MysqlDumpPath { get; }
    }
}