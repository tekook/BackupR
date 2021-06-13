using System;
using Tekook.BackupR.Lib.Contracts;

namespace Tekook.BackupR.Lib.Exceptions
{
    internal class BackupException : Exception
    {
        public IBackupTask BackupTask { get; }

        public BackupException(IBackupTask backupTask)
        {
            this.BackupTask = backupTask ?? throw new ArgumentNullException(nameof(backupTask));
        }

        public BackupException(IBackupTask backupTask, string message) : base(message)
        {
            this.BackupTask = backupTask ?? throw new ArgumentNullException(nameof(backupTask));
        }

        public BackupException(IBackupTask backupTask, string message, Exception innerException) : base(message, innerException)
        {
            this.BackupTask = backupTask ?? throw new ArgumentNullException(nameof(backupTask));
        }
    }
}