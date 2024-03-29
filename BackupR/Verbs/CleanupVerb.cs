﻿using ByteSizeLib;
using Config.Net;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration.Provider;
using System.Linq;
using System.Threading.Tasks;
using Tekook.BackupR.Lib.Config;
using Tekook.BackupR.Lib.Contracts;
using Tekook.VerbR.Resolvers;

namespace Tekook.BackupR.Verbs
{
    internal class CleanupVerb : VerbR.Verb<CleanupOptions, IConfig>
    {
        protected ILogger Logger { get; set; } = LogManager.GetCurrentClassLogger();

        public CleanupVerb(CleanupOptions options) : base(options)
        {
            this.Resolver = new ConfigNetResolver<IConfig, CleanupOptions>((builder) => builder.UseJsonFile(options.Config));
        }

        public override async Task<int> InvokeAsync()
        {
            try
            {
                Logger.Info("Starting cleanup");
                using IProvider provider = Lib.Resolver.ResolveProvider(this.Config, this.Options);
                Logger.Info("Validating provider: {provider}", provider.GetType().Name);
                await provider.Validate();
                IContainer root = await provider.GetRoot();

                foreach (IContainerConfig configContainer in this.Config.Containers)
                {
                    IContainer container = root.GetContainer(configContainer.Path);
                    if (container == null)
                    {
                        Logger.Info("Container not found for {path}", configContainer.Path);
                        continue;
                    }
                    if (configContainer.MaxSize != null)
                    {
                        try
                        {
                            await HandleMaxSize(configContainer, container);
                        }
                        catch (Exception e)
                        {
                            Logger.Error("Error while handling maxsize of {container}", container);
                            Logger.Error(e, e.Message);
                        }
                    }
                    if (configContainer.MaxFiles != null)
                    {
                        try
                        {
                            await HandleMaxFiles(configContainer, container);
                        }
                        catch (Exception e)
                        {
                            Logger.Error("Error while handling maxfiles of {container}", container);
                            Logger.Error(e, e.Message);
                        }
                    }
                }
            }
            catch (ProviderException e)
            {
                Logger.Error(e, e.Message);
                return 1;
            }
            catch (Exception e)
            {
                Logger.Error("Unkown error caught -> {type}", e.GetType().FullName);
                Logger.Error(e);
                return 1;
            }
            return 0;
        }

        private async Task HandleMaxFiles(IContainerConfig configContainer, IContainer container)
        {
            var files = container.Items.Where(x => !x.Deleted).OrderBy(x => x.Date).ToList();
            Logger.Info("Handling {path} with MaxFiles: {max_files}. Current: {file_count}", configContainer.Path, configContainer.MaxFiles, files.Count);
            var toDelete = new List<IItem>();
            while (files.Count > configContainer.MinFiles && files.Count > configContainer.MaxFiles)
            {
                toDelete.Add(files[0]);
                files.RemoveAt(0);
            }
            if (toDelete.Count > 0)
            {
                foreach (var item in toDelete)
                {
                    Logger.Info("Deleting {path} -> {size}", item.Path, ByteSize.FromBytes(item.Size));
                    await item.Delete();
                }
                Logger.Info("Remaining files: {count}", files.Count);
            }
        }

        private async Task HandleMaxSize(IContainerConfig configContainer, IContainer container)
        {
            ByteSize max = ByteSize.Parse(configContainer.MaxSize);
            var files = container.Items.Where(x => !x.Deleted).OrderBy(x => x.Date).ToList();
            ByteSize containerSize = ByteSize.FromBytes(files.Sum(x => x.Size));
            Logger.Info("Handling {path} with MaxSize: {max_size}. Current: {container_size}", configContainer.Path, max, containerSize);
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
                    Logger.Info("Deleting {path} -> {size}", item.Path, ByteSize.FromBytes(item.Size));
                    await item.Delete();
                }
                containerSize = ByteSize.FromBytes(files.Sum(x => x.Size));
                Logger.Info("Remaining size: {size}", containerSize);
            }
        }
    }
}