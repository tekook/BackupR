using System.Collections.Generic;

namespace Tekook.BackupR.Lib.Config
{
    public interface IConfigBackup
    {
        IEnumerable<IFolderBackup> Folders { get; }
    }
}