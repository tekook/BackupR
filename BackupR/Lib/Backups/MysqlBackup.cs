using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tekook.BackupR.Lib.Config;

namespace Tekook.BackupR.Lib.Backups
{
    internal class MysqlBackup : Backup
    {
        protected IMysqlBackup Settings { get; set; }
        public MysqlBackup(IMysqlBackup settings)
        {
            this.Settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }
        public override Task<FileInfo> CreateBackup()
        {
            throw new NotImplementedException();
        }
    }
}
