using System;
using System.Threading.Tasks;

namespace Tekook.BackupR.Lib.Contracts
{
    /// <summary>
    /// Contract for an item.
    /// </summary>
    public interface IItem
    {
        /// <summary>
        /// Container this item belongs to.
        /// </summary>
        IContainer Container { get; }

        /// <summary>
        /// Date this item was last modified or created.
        /// </summary>
        DateTime Date { get; }

        /// <summary>
        /// Determinates if this item has been deleted or not.
        /// </summary>
        bool Deleted { get; }

        /// <summary>
        /// Name of the item.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Abs. path of the item.
        /// </summary>
        string Path { get; }

        /// <summary>
        /// Size of the item in bytes.
        /// </summary>
        long Size { get; }

        /// <summary>
        /// Deletes the item from the container.
        /// </summary>
        /// <returns></returns>
        Task Delete();
    }
}