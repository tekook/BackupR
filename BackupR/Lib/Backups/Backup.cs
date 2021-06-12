using System;
using System.IO;
using System.Threading.Tasks;
using Tekook.BackupR.Lib.Config;
using Tekook.BackupR.Lib.Contracts;

namespace Tekook.BackupR.Lib.Backups
{
    internal abstract class Backup : IBackupTask
    {
        /// <inheritdoc/>
        public FileInfo BackupFile { get; protected set; }

        /// <inheritdoc/>
        public abstract Task<FileInfo> CreateBackup();

        // <inheritdoc />
        public virtual string GetUploadName(IBackup settings)
        {
            return settings.UploadName?.Replace("$date", DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"), StringComparison.CurrentCultureIgnoreCase);
        }

        /// <inheritdoc/>
        public virtual void RemoveBackup()
        {
            this.BackupFile?.Delete();
            this.BackupFile = null;
        }
    }
}