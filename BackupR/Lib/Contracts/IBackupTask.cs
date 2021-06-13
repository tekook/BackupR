using System.IO;
using System.Threading.Tasks;
using Tekook.BackupR.Lib.Config;

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
        /// Cleans all temporary files of the backup task.
        /// </summary>
        void CleanupTask();

        /// <summary>
        /// Creates the backup.
        /// </summary>
        /// <returns>Path of the created backup file.</returns>
        Task<FileInfo> CreateBackup();

        /// <summary>
        /// Replaces all variables for the upload name. (Duh... fix this doc stupid).
        /// </summary>
        /// <param name="settings">Settings to take the <see cref="IBackup.UploadName"/> from.</param>
        /// <returns>Name to use for Upload or null if <see cref="IBackup.UploadName"/> is null.</returns>
        string GetUploadName(IBackup settings);

        /// <summary>
        /// Removes the created backup from local storage.
        /// </summary>
        void RemoveBackup();
    }
}