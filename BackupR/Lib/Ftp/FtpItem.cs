﻿using System;
using System.Threading.Tasks;
using Tekook.BackupR.Lib.Contracts;

namespace Tekook.BackupR.Lib.Ftp
{
    public class FtpItem : IItem
    {
        public FtpContainer Container { get; }

        IContainer IItem.Container => this.Container;

        public DateTime Date { get; set; }
        public string FullName { get; set; }

        public string Name { get; set; }

        public long Size { get; set; }

        public FtpItem(FtpContainer container)
        {
            this.Container = container ?? throw new ArgumentNullException(nameof(container));
        }

        public async Task Delete()
        {
            await this.Container.Provider.Delete(this);
        }
    }
}