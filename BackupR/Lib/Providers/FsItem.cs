using System;
using System.Threading.Tasks;
using Tekook.BackupR.Lib.Contracts;

namespace Tekook.BackupR.Lib.Providers
{
    /// <summary>
    /// Implementation for Filesystem.
    /// </summary>
    public class FsItem : IItem
    {
        /// <summary>
        /// The <see cref="FsContainer"/> this <see cref="FsItem"/> belongs to.
        /// </summary>
        public FsContainer Container { get; }

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
        /// Creates a new <see cref="FsItem"/>.
        /// </summary>
        /// <param name="container"><see cref="FsContainer"/> this item belongs to.</param>
        /// <exception cref="ArgumentNullException">Thrown if container is null</exception>
        public FsItem(FsContainer container)
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