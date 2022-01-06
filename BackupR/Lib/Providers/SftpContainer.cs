using Tekook.BackupR.Lib.Contracts;

namespace Tekook.BackupR.Lib.Providers
{
    /// <summary>
    /// Implementation of an SftpContainer
    /// </summary>
    public class SftpContainer : Container<SftpProvider, SftpContainer, SftpItem>, IContainer
    {
        /// <summary>
        /// Creates a new <see cref="SftpContainer"/>
        /// </summary>
        /// <param name="provider">Provider this container belongs to.</param>
        /// <param name="path">Path this container is set on.</param>
        public SftpContainer(SftpProvider provider, string path) : base(provider, path)
        {
        }
    }
}