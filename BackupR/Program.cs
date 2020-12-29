using FluentFTP;
using System;
using System.Threading.Tasks;
using Tekook.BackupR.Lib.Contracts;
using Tekook.BackupR.Lib.Ftp;

namespace Tekook.BackupR
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            FtpConfig config = new FtpConfig()
            {
                Host = "ftpserver",
                Path = "/"
            };
            FTPProvider provider = new FTPProvider(config);
            IContainer root = await provider.Read();
            foreach(IContainer sub in root.Containers)
            {
                Console.WriteLine(sub);
            }
        }
    }
}