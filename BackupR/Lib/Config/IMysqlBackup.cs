using System.Collections.Generic;

namespace Tekook.BackupR.Lib.Config
{
    public interface IMysqlBackup : IBackup, IMysqlBackupOptions
    {
        public bool FetchDatabases { get; }
        public bool PasswordViaEnvironment { get; }
        public IEnumerable<IMysqlBackupOptions> Options { get; }
        public IEnumerable<string> Databases { get; }
        public IEnumerable<string> Excludes { get; }
        public int Port { get; }
        public string Host { get; }
        public string MysqlDumpPath { get; }
        public string Password { get; }
        public string Username { get; }
    }
}