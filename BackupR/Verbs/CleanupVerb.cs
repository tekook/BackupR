using ByteSizeLib;
using Config.Net;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration.Provider;
using System.Linq;
using System.Threading.Tasks;
using Tekook.BackupR.Lib.Config;
using Tekook.BackupR.Lib.Contracts;
using Tekook.BackupR.Lib.ProviderExtensions;
using Tekook.BackupR.Lib.StateManagement;
using Tekook.VerbR.Resolvers;

namespace Tekook.BackupR.Verbs
{
    internal class CleanupVerb : VerbR.Verb<CleanupOptions, IConfig>
    {
        protected ILogger Logger { get; set; } = LogManager.GetCurrentClassLogger();
        protected CleanupState State { get; set; } = new();

        public CleanupVerb(CleanupOptions options) : base(options)
        {
            this.Resolver = new ConfigNetResolver<IConfig, CleanupOptions>((builder) => builder.UseJsonFile(options.Config));
        }

        public override async Task<int> InvokeAsync()
        {
            try
            {
                Logger.Info("Starting cleanup");
                StateManager.Init(this.Config.StateFile);
                this.State.Start();
                using IProvider provider = Lib.Resolver.ResolveProvider(this.Config, this.Options);
                Logger.Info("Validating provider: {provider}", provider.GetType().Name);
                await provider.Validate(3, Logger);
                IContainer root = await provider.GetRoot();

                foreach (IContainerConfig configContainer in this.Config.Containers)
                {
                    CleanupTask task = new()
                    {
                        Name = configContainer.Path,
                        Success = true,
                        MaxFilesConfig = configContainer.MaxFiles,
                        MaxSizeConfig = configContainer.MaxSize,
                    };
                    this.State.AddTask(task);

                    IContainer container = root.GetContainer(configContainer.Path);
                    if (container == null)
                    {
                        Logger.Info("Container not found for {path}", configContainer.Path);
                        task.Success = false;
                        continue;
                    }
                    if (configContainer.MaxSize != null)
                    {
                        try
                        {
                            await HandleMaxSize(configContainer, container, task);
                        }
                        catch (Exception e)
                        {
                            Logger.Error("Error while handling maxsize of {container}", container);
                            Logger.Error(e, e.Message);
                            task.Success = false;
                        }
                    }
                    if (configContainer.MaxFiles != null)
                    {
                        try
                        {
                            await HandleMaxFiles(configContainer, container, task);
                        }
                        catch (Exception e)
                        {
                            Logger.Error("Error while handling maxfiles of {container}", container);
                            Logger.Error(e, e.Message);
                            task.Success = false;
                        }
                    }
                    // List Files
                    var files = container.Items.Where(x => !x.Deleted).OrderBy(x => x.Date).ToList();
                    task.Size = files.Sum(x => x.Size);
                    task.RemainingFiles = files.Count;
                    task.TotalDeletedFiles = task.MaxFilesDeletedFiles + task.MaxSizeDeletedFiles;
                    task.TotalDeletedFilesSize = task.MaxFilesDeletedFilesSize + task.MaxSizeDeletedFilesSize;
                    Logger.Info("Cleanup done");
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
            finally
            {
                StateManager.Stop(this.State);
            }
            return 0;
        }

        private async Task HandleMaxFiles(IContainerConfig configContainer, IContainer container, CleanupTask task = null)
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
            if (task != null)
            {
                task.MaxFilesDeletedFiles = toDelete.Count;
                task.MaxFilesDeletedFilesSize = toDelete.Sum(x => x.Size);
            }
        }

        private async Task HandleMaxSize(IContainerConfig configContainer, IContainer container, CleanupTask task = null)
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
                if (task != null)
                {
                    task.MaxSizeDeletedFiles = toDelete.Count;
                    task.MaxSizeDeletedFilesSize = toDelete.Sum(x => x.Size);
                }
                containerSize = ByteSize.FromBytes(files.Sum(x => x.Size));
                Logger.Info("Remaining size: {size}", containerSize);
            }
            else if (task != null)
            {
                task.MaxSizeDeletedFiles = 0;
                task.MaxSizeDeletedFilesSize = 0;
            }
        }
    }
}