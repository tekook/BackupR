using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tekook.BackupR.Lib.Config
{
    public interface IMysqlBackupOptions
    {
        public string Database { get; }
        public bool AddLocks { get; }
        public bool ColumnStatistics { get; }
        public bool Events { get; }
        public bool FlushPrivileges { get; }
        public bool Routines { get; }
        public bool SkipLockTables { get; }
        public bool Triggers { get; }
    }
}
