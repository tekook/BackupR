using Tekook.BackupR.Lib.Contracts;

namespace Tekook.BackupR.Lib.Filesystem
{
    /// <summary>
    /// Implementation of an FsContainer
    /// </summary>
    public class FsContainer : Container<FsProvider, FsContainer, FsItem>, IContainer
    {
        /// <summary>
        /// Creates a new <see cref="FsContainer"/>
        /// </summary>
        /// <param name="provider">Provider this container belongs to.</param>
        /// <param name="path">Path this container is set on.</param>
        public FsContainer(FsProvider provider, string path) : base(provider, path)
        {
        }
    }
}