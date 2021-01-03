using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tekook.BackupR.Lib;
using Tekook.BackupR.Lib.Contracts;
using Tekook.BackupR.Lib.Ftp;
using Config.Net;

namespace Tekook.BackupR
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var builder = new ConfigurationBuilder<IFtpConfig>();
            builder.UseJsonFile("./Examples/ftp-config.json");
            IFtpConfig config = builder.Build();
            FtpProvider provider = new FtpProvider(config);
            IContainer root = await provider.Read();
            long max = 1024 * 1024 * 1024;
            foreach (IContainer sub in root.Containers)
            {
                Console.WriteLine($"Checking {sub.Name} with Size: {sub.Size.ToReadableBytes()}");
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
                        Console.WriteLine($"Deleting {item.Path} -> {item.Size.ToReadableBytes()}");
                        await item.Delete();
                    }
                }
            }
        }
    }
}