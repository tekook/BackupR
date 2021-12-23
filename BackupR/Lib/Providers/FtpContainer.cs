using Tekook.BackupR.Lib.Contracts;

namespace Tekook.BackupR.Lib.Providers
{
    /// <summary>
    /// Implementation of an FtpContainer
    /// </summary>
    public class FtpContainer : Container<FtpProvider, FtpContainer, FtpItem>, IContainer
    {
        /// <summary>
        /// Creates a new <see cref="FtpContainer"/>
        /// </summary>
        /// <param name="provider">Provider this container belongs to.</param>
        /// <param name="path">Path this container is set on.</param>
        public FtpContainer(FtpProvider provider, string path) : base(provider, path)
        {
        }
    }
}