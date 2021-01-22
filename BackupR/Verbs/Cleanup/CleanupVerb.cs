using ByteSizeLib;
using Config.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tekook.BackupR.Lib.Config;
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

            foreach (IContainerConfig configContainer in this.Config.Containers)
            {
                IContainer container = root.AllContainers.Where(x => x.Path == root.Path + configContainer.Path).FirstOrDefault();
                if (container == null)
                {
                    Console.WriteLine($"Container not found for {configContainer.Path}");
                    continue;
                }
                if (configContainer.MaxSize != null)
                {
                    await HandleMaxSize(configContainer, container);
                }
                if (configContainer.MaxFiles != null)
                {
                    await HandleMaxFiles(configContainer, container);
                }
            }
            return 0;
        }

        private async Task HandleMaxFiles(IContainerConfig configContainer, IContainer container)
        {
            var files = container.Items.Where(x => !x.Deleted).OrderBy(x => x.Date).ToList();
            Console.WriteLine($"Handling {configContainer.Path} with MaxFiles: {configContainer.MaxFiles}. Current: {files.Count}");
            var toDelete = new List<IItem>();
            while (files.Count > configContainer.MinFiles && files.Count > configContainer.MaxFiles)
            {
                toDelete.Add(files[0]);
                files.RemoveAt(0);
            }
            foreach (var item in toDelete)
            {
                Console.WriteLine($"Deleting {item.Path} -> {ByteSize.FromBytes(item.Size)}");
                await item.Delete();
            }
        }

        private async Task HandleMaxSize(IContainerConfig configContainer, IContainer container)
        {
            ByteSize max = ByteSize.Parse(configContainer.MaxSize);
            var files = container.Items.Where(x => !x.Deleted).OrderBy(x => x.Date).ToList();
            ByteSize containerSize = ByteSize.FromBytes(files.Sum(x => x.Size));
            Console.WriteLine($"Handling {configContainer.Path} with MaxSize: {max}. Current: {containerSize}");
            if (containerSize > max)
            {
                var toDelete = new List<IItem>();
                while (files.Sum(x => x.Size) > max.Bytes && files.Count > configContainer.MinFiles)
                {
                    toDelete.Add(files[0]);
                    files.RemoveAt(0);
                }
                foreach (var item in toDelete)
                {
                    Console.WriteLine($"Deleting {item.Path} -> {ByteSize.FromBytes(item.Size)}");
                    await item.Delete();
                }
            }
        }
    }
}