using System.Collections.Generic;

namespace Tekook.BackupR.Lib.Config
{
    public interface IBackupConfig
    {
        IEnumerable<IFolderBackup> Folders { get; }
    }
}