using System.Collections.Generic;

namespace Tekook.BackupR.Lib.Config
{
    public interface IBackupConfig
    {
        IEnumerable<ICommandBackup> Commands { get; }
        IEnumerable<IFolderBackup> Folders { get; }
        IEnumerable<IMysqlBackup> MysqlBackups { get; }
    }
}