using System;
using System.Collections.Generic;
using System.Linq;
using Tekook.BackupR.Lib.Contracts;

namespace Tekook.BackupR.Lib.Ftp
{
    /// <summary>
    /// Implementation of an FtpContainer
    /// </summary>
    public class FtpContainer : IContainer
    {
        /// <summary>
        /// Child Containers this Container holds.
        /// </summary>
        public List<FtpContainer> Containers { get; } = new List<FtpContainer>();

        /// <inheritdoc/>
        IEnumerable<IContainer> IContainer.Containers => this.Containers;

        /// <summary>
        /// Items this container holds.
        /// </summary>
        public List<FtpItem> Items { get; } = new List<FtpItem>();

        /// <inheritdoc/>
        IEnumerable<IItem> IContainer.Items => this.Items;

        /// <inheritdoc/>
        public string Name { get; }

        /// <inheritdoc/>
        public string Path { get; }

        /// <summary>
        /// Provider this container belongs to.
        /// </summary>
        public FtpProvider Provider { get; }

        /// <inheritdoc/>
        IProvider IContainer.Provider => this.Provider;

        /// <inheritdoc/>
        public long Size => this.Items.Sum(x => x.Size) + this.Containers.Sum(x => x.Size);

        /// <summary>
        /// Creates a new <see cref="FtpContainer"/>
        /// </summary>
        /// <param name="provider">Provider this container belongs to.</param>
        /// <param name="path">Path this container is set on.</param>
        public FtpContainer(FtpProvider provider, string path)
        {
            this.Provider = provider ?? throw new ArgumentNullException(nameof(provider));
            this.Path = path ?? throw new ArgumentNullException(nameof(path));
            this.Name = System.IO.Path.GetFileName(path);
        }
    }
}