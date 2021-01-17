using System.Collections.Generic;

namespace Tekook.BackupR.Lib.Contracts
{
    /// <summary>
    /// Contract for a container.
    /// </summary>
    public interface IContainer
    {
        /// <summary>
        /// List of all containers underlying to this one.
        /// </summary>
        IEnumerable<IContainer> AllContainers { get; }

        /// <summary>
        /// List of direct child containers.
        /// </summary>
        IEnumerable<IContainer> Containers { get; }

        /// <summary>
        /// Direct child items.
        /// </summary>
        IEnumerable<IItem> Items { get; }

        /// <summary>
        /// Name of the container.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Abs. path of the container.
        /// </summary>
        string Path { get; }

        /// <summary>
        /// Provider this container was created by.
        /// </summary>
        IProvider Provider { get; }

        /// <summary>
        /// Size in bytes.
        /// </summary>
        long Size { get; }
    }
}