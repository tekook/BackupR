using System;
using System.Collections.Generic;
using System.Linq;
using Tekook.BackupR.Lib.Contracts;

namespace Tekook.BackupR.Lib.Ftp
{
    public class FtpContainer : IContainer
    {
        public List<FtpContainer> Containers { get; } = new List<FtpContainer>();

        IEnumerable<IContainer> IContainer.Containers => this.Containers;

        public List<FtpItem> Items { get; } = new List<FtpItem>();

        IEnumerable<IItem> IContainer.Items => this.Items;

        public string Name { get; }
        public string Path { get; }
        public FTPProvider Provider { get; }

        IProvider IContainer.Provider => this.Provider;

        public long Size => this.Items.Sum(x => x.Size);

        public FtpContainer(FTPProvider provider, string path)
        {
            this.Provider = provider ?? throw new ArgumentNullException(nameof(provider));
            this.Path = path ?? throw new ArgumentNullException(nameof(path));
            this.Name = System.IO.Path.GetFileName(path);
        }
    }
}