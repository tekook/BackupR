using System.IO;
using System.Threading.Tasks;

namespace Tekook.BackupR.Lib.Backups
{
    internal abstract class Backup
    {
        /// <summary>
        /// Creates the backup.
        /// </summary>
        /// <returns>Path of the created backup file.</returns>
        public abstract Task<FileInfo> CreateBackup();
    }
}