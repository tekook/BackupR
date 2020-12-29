using System.Net;

namespace Tekook.BackupR.Lib.Ftp
{
    public class FtpConfig
    {
        public string Host { get; set; }
        public string Password { get; set; }
        public string Path { get; set; }
        public string Username { get; set; }

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