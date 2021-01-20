using System;
using System.IO;
using System.Threading.Tasks;

namespace Tekook.BackupR.Lib.Contracts
{
    /// <summary>
    /// Contract for a provider
    /// </summary>
    public interface IProvider
    {
        /// <summary>
        /// Deletes an item from this provider.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the item is not derrived from this provider.</exception>
        /// <param name="item">The Item to delete.</param>
        /// <returns></returns>
        Task Delete(IItem item);

        /// <summary>
        /// Reads the root container of the provider and all its children.
        /// </summary>
        /// <returns>The root container with all child containers and items.</returns>
        Task<IContainer> GetRoot();

        /// <summary>
        /// Uploads a file to the specific target container
        /// </summary>
        /// <param name="file">File to Upload.</param>
        /// <param name="target">Target Container,</param>
        /// <param name="name">Optional name to use on upload.</param>
        /// <exception cref="InvalidOperationException">Thrown when the container is not derrived from this provider.</exception>
        /// <returns></returns>
        Task Upload(FileInfo file, IContainer target, string name = null);
    }
}