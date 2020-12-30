using System;
using System.Collections.Generic;
using System.Linq;
using Tekook.BackupR.Lib.Contracts;

namespace Tekook.BackupR.Lib
{
    public abstract class Container<TProvider, TContainer, TItem> : IContainer where TItem : IItem where TContainer : IContainer where TProvider : IProvider
    {
        /// <inheritdoc/>
        IEnumerable<IContainer> IContainer.Containers => (IEnumerable<IContainer>)this.Containers;

        /// <summary>
        /// Child containers of this container.
        /// </summary>
        public List<TContainer> Containers { get; set; } = new List<TContainer>();

        /// <inheritdoc/>
        IEnumerable<IItem> IContainer.Items => (IEnumerable<IItem>)this.Items;

        /// <summary>
        /// Directy child items this container has.
        /// </summary>
        public List<TItem> Items { get; set; } = new List<TItem>();

        /// <inheritdoc/>
        public string Name { get; }

        /// <inheritdoc/>
        public string Path { get; }

        /// <summary>
        /// Provider this container belongs to.
        /// </summary>
        public TProvider Provider { get; }

        /// <inheritdoc/>
        IProvider IContainer.Provider => this.Provider;

        /// <summary>
        /// Size in a readable string.
        /// </summary>
        public string ReadableSize => this.Size.ToReadableBytes();

        /// <inheritdoc/>
        public long Size => this.Items.Sum(x => x.Size) + this.Containers.Sum(x => x.Size);

        /// <summary>
        /// Creates a new Container with the given provider.
        /// </summary>
        /// <param name="provider">Provider to use.</param>
        /// <param name="path">Path this container sits on.</param>
        public Container(TProvider provider, string path)
        {
            this.Provider = provider;
            this.Path = path ?? throw new ArgumentNullException(nameof(path));
            this.Name = System.IO.Path.GetFileName(path);
        }

        public override string ToString()
        {
            return $"{this.GetType().Name}:{this.Path} ({this.ReadableSize})";
        }
    }
}