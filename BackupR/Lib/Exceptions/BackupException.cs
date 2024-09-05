using System;
using Tekook.BackupR.Lib.Contracts;

namespace Tekook.BackupR.Lib.Exceptions
{
    internal class BackupException : BaseException
    {
        public IBackupTask BackupTask { get; }

        public BackupException(IBackupTask backupTask) : base()
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