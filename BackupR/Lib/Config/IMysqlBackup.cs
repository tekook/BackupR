using System.Collections.Generic;

namespace Tekook.BackupR.Lib.Config
{
    public interface IMysqlBackup : IBackup
    {
        public bool AddLocks { get; }
        public IEnumerable<string> Databases { get; set; }
        public bool Events { get; }
        public bool FlushPrivilegies { get; }
        public string MysqlDumpPath { get; }
        public bool Routines { get; }
        public bool Triggers { get; }
    }
}