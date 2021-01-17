using ByteSizeLib;
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

            foreach (IConfigContainer configContainer in this.Config.Containers)
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

        private async Task HandleMaxFiles(IConfigContainer configContainer, IContainer container)
        {
            Console.WriteLine($"Handling {configContainer.Path} with MaxFiles: {configContainer.MaxFiles}. Current: {container.Items.Count()}");
            var files = container.Items.OrderBy(x => x.Date).ToList();
            var toDelete = new List<IItem>();
            while (files.Count > configContainer.MinFiles && files.Count > configContainer.MaxFiles)
            {
                toDelete.Add(files[0]);
                files.RemoveAt(0);
            }
            foreach (var item in toDelete)
            {
                Console.WriteLine($"Deleting {item.Path} -> {ByteSize.FromBytes(item.Size)}");
                //await item.Delete();
            }
        }

        private async Task HandleMaxSize(IConfigContainer configContainer, IContainer container)
        {
            ByteSize max = ByteSize.Parse(configContainer.MaxSize);
            ByteSize containerSize = ByteSize.FromBytes(container.Size);
            Console.WriteLine($"Handling {configContainer.Path} with MaxSize: {max}. Current: {containerSize}");
            if (containerSize > max)

            {
                var files = container.Items.OrderBy(x => x.Date).ToList();
                var toDelete = new List<IItem>();
                while (files.Sum(x => x.Size) > max.Bytes && files.Count > configContainer.MinFiles)
                {
                    toDelete.Add(files[0]);
                    files.RemoveAt(0);
                }
                foreach (var item in toDelete)
                {
                    Console.WriteLine($"Deleting {item.Path} -> {ByteSize.FromBytes(item.Size)}");
                    //await item.Delete();
                }
            }
        }
    }
}