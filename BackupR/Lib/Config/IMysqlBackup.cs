namespace Tekook.BackupR.Lib.Config
{
    public interface IMysqlBackup : IBackup
    {
        public string MysqlDumpPath { get; }
        public bool AddLocks { get; }
        public bool FlushPrivilegies { get; }
        public bool Routines { get; }
        public bool Events { get; }
        public bool Triggers { get; }
        public string DbName { get; }
    }
}