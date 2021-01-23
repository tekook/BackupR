using System.IO;
using System.Threading.Tasks;
using Tekook.BackupR.Lib.Contracts;

namespace Tekook.BackupR.Lib.Backups
{
    internal abstract class Backup : IBackupTask
    {
        /// <inheritdoc/>
        public FileInfo BackupFile { get; protected set; }

        /// <inheritdoc/>
        public abstract Task<FileInfo> CreateBackup();

        /// <inheritdoc/>
        public virtual void RemoveBackup()
        {
            this.BackupFile?.Delete();
            this.BackupFile = null;
        }
    }
}