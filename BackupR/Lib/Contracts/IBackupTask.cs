using System.IO;
using System.Threading.Tasks;

namespace Tekook.BackupR.Lib.Contracts
{
    public interface IBackupTask
    {
        /// <summary>
        /// The Backup file.
        /// Null if is has not been created yet.
        /// </summary>
        FileInfo BackupFile { get; }

        /// <summary>
        /// Creates the backup.
        /// </summary>
        /// <returns>Path of the created backup file.</returns>
        Task<FileInfo> CreateBackup();

        /// <summary>
        /// Removes the created backup from local storage.
        /// </summary>
        void RemoveBackup();
    }
}