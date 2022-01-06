using System.Collections.Generic;

namespace Tekook.BackupR.Lib.Config
{
    public interface IMysqlBackup : IBackup
    {
        public bool AddLocks { get; }
        public bool ColumnStatistics { get; }
        public IEnumerable<string> Databases { get; }
        public bool Events { get; }
        public IEnumerable<string> Excludes { get; }
        public bool FetchDatabases { get; }
        public bool FlushPrivileges { get; }
        public string Host { get; }
        public string MysqlDumpPath { get; }
        public string Password { get; }
        public bool PasswordViaEnvironment { get; }
        public bool Routines { get; }
        public bool Triggers { get; }
        public string Username { get; }
    }
}