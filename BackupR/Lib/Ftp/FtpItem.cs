using System;
using System.Threading.Tasks;
using Tekook.BackupR.Lib.Contracts;

namespace Tekook.BackupR.Lib.Ftp
{
    /// <summary>
    /// Implementation for Ftp.
    /// </summary>
    public class FtpItem : IItem
    {
        /// <summary>
        /// The <see cref="FtpContainer"/> this <see cref="FtpItem"/> belongs to.
        /// </summary>
        public FtpContainer Container { get; }

        /// <inheritdoc/>
        IContainer IItem.Container => this.Container;

        /// <inheritdoc/>
        public DateTime Date { get; set; }

        /// <inheritdoc/>
        public bool Deleted { get; protected set; }

        /// <inheritdoc/>
        public string Name { get; set; }

        /// <inheritdoc/>
        public string Path { get; set; }

        /// <inheritdoc/>
        public long Size { get; set; }

        /// <summary>
        /// Creates a new <see cref="FtpItem"/>.
        /// </summary>
        /// <param name="container"><see cref="FtpContainer"/> this item belongs to.</param>
        /// <exception cref="ArgumentNullException">Thrown if container is null</exception>
        public FtpItem(FtpContainer container)
        {
            this.Container = container ?? throw new ArgumentNullException(nameof(container));
        }

        /// <inheritdoc/>
        public async Task Delete()
        {
            await this.Container.Provider.Delete(this);
            this.Deleted = true;
        }

        public override string ToString()
        {
            return $"{this.GetType().Name}:{this.Name} ({ByteSizeLib.ByteSize.FromBytes(this.Size)})";
        }
    }
}