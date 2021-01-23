using SharpCompress.Archives;
using SharpCompress.Archives.Tar;
using SharpCompress.Common;
using System;
using System.IO;
using System.Threading.Tasks;
using Tekook.BackupR.Lib.Config;

namespace Tekook.BackupR.Lib.Backups
{
    internal class FolderBackup : Backup
    {
        protected IFolderBackup Settings { get; set; }

        public FolderBackup(IFolderBackup settings)
        {
            this.Settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public override async Task<FileInfo> CreateBackup()
        {
            await Task.Run(() =>
            {
                string tempFile = Path.GetTempFileName();
                using (var archive = TarArchive.Create())
                {
                    archive.AddAllFromDirectory(this.Settings.Path);
                    archive.SaveTo(tempFile, CompressionType.GZip);
                }
                this.BackupFile = new FileInfo(tempFile);
            });
            return this.BackupFile;
        }
    }
}