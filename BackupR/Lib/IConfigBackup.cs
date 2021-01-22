using System.Collections.Generic;
using Tekook.BackupR.Lib.Backups;

namespace Tekook.BackupR.Lib
{
    public interface IConfigBackup
    {
        IEnumerable<IFolderBackup> Folders { get; }
    }
}