using Config.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tekook.BackupR.Lib;
using Tekook.BackupR.Lib.Configs;
using Tekook.BackupR.Lib.Contracts;
using Tekook.VerbR.Resolvers;

namespace Tekook.BackupR.Verbs.Cleanup
{
    internal class CleanupVerb : VerbR.Verb<CleanupOptions, IConfig>
    {
        public CleanupVerb(CleanupOptions options) : base(options)
        {
            this.Resolver = new ConfigNetResolver<IConfig, CleanupOptions>((builder) => builder.UseJsonFile(options.Config));
        }

        public override async Task<int> InvokeAsync()
        {
            IProvider provider = Lib.Resolver.ResolveProvider(this.Config, this.Options);
            IContainer root = await provider.GetRoot();

            long max = 1024 * 1024 * 1024;
            foreach (IContainer sub in root.Containers)
            {
                Console.WriteLine($"Checking {sub.Name} with Size: {ByteSizeLib.ByteSize.FromBytes(sub.Size)}");
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
                        Console.WriteLine($"Deleting {item.Path} -> {ByteSizeLib.ByteSize.FromBytes(item.Size)}");
                        await item.Delete();
                    }
                }
            }
            return 0;
        }
    }
}