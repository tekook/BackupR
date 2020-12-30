using System.Net;

namespace Tekook.BackupR.Lib.Ftp
{
    /// <summary>
    /// Configuration for an <see cref="FtpProvider"/>.
    /// </summary>
    public class FtpConfig
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

        /// <summary>
        /// Genereates the NetworkCredential for this configuration.
        /// </summary>
        /// <returns>Null if <see cref="Username"/> or <see cref="Password"/> is null.</returns>
        public NetworkCredential GetNetworkCredential()
        {
            if (this.Username == null || this.Password == null)
            {
                return null;
            }
            return new NetworkCredential(this.Username, this.Password);
        }
    }
}