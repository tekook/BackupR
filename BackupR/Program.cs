using System;
using System.Collections.Generic;
using System.Linq;
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
                Host = "ftp.dev.local",
                Path = "/",
                Username = "docker-dev",
                Password = "docker-dev"
            };
            FTPProvider provider = new FTPProvider(config);
            IContainer root = await provider.Read();
            long max = 1024 * 1024 * 1024;
            foreach (IContainer sub in root.Containers)
            {
                if (sub.Name == "home")
                {
                    if (sub.Size > max)
                    {
                        var files = sub.Items.OrderBy(x => x.Date).ToList();
                        var toDelete = new List<IItem>();
                        while (files.Sum(x => x.Size) > max && files.Count > 0)
                        {
                            toDelete.Add(files[0]);
                            files.RemoveAt(0);
                        }
                        foreach (var item in toDelete)
                        {
                            Console.WriteLine($"Deleting {item.FullName}");
                            await item.Delete();
                        }
                    }
                }
            }
        }
    }
}