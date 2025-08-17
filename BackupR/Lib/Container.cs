using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Tekook.BackupR.Lib.Contracts;

namespace Tekook.BackupR.Lib
{
    public abstract class Container<TProvider, TContainer, TItem> : IContainer where TItem : IItem where TContainer : IContainer where TProvider : IProvider
    {
        /// <inheritdoc/>
        IEnumerable<IContainer> IContainer.Containers => (IEnumerable<IContainer>)this.Containers;

        /// <summary>
        /// List of direct child containers of this container.
        /// </summary>
        public List<TContainer> Containers { get; set; } = [];

        /// <inheritdoc/>
        IEnumerable<IItem> IContainer.Items => (IEnumerable<IItem>)this.Items;

        /// <summary>
        /// Directy child items this container has.
        /// </summary>
        public List<TItem> Items { get; set; } = [];

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
        public string ReadableSize => ByteSizeLib.ByteSize.FromBytes(this.Size).ToString();

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

        /// <inheritdoc/>
        public IContainer GetContainer(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException(nameof(path));
            }
            path = path.TrimStart(this.Provider.Seperator);
            List<string> paths = path.Split(this.Provider.Seperator).ToList();
            IContainer container = this.Containers.FirstOrDefault(x => x.Name == paths[0]);
            if (container != null && paths.Count > 1)
            {
                paths.RemoveAt(0);
                return container.GetContainer(string.Join(this.Provider.Seperator, paths));
            }
            return container;
        }

        public override string ToString()
        {
            return $"{this.GetType().Name}:{this.Path} ({this.ReadableSize})";
        }

        /// <inheritdoc/>
        public async Task Upload(FileInfo file, string name = null)
        {
            await this.Provider.Upload(file, this, name);
        }
    }
}