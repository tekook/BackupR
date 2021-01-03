using System.Net;

namespace Tekook.BackupR.Lib.Ftp
{
    /// <summary>
    /// Configuration for an <see cref="FtpProvider"/>.
    /// </summary>
    public interface IFtpConfig
    {
        /// <summary>
        /// Hostname to connect to.
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// Password to auth with.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Root path to connect to.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Port to use.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Username to auth with.
        /// </summary>
        public string Username { get; set; }
    }
}