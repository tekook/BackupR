﻿using ByteSizeLib;
using Config.Net;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Tekook.BackupR.Lib.Backups;
using Tekook.BackupR.Lib.Config;
using Tekook.BackupR.Lib.Contracts;
using Tekook.BackupR.Lib.Exceptions;
using Tekook.BackupR.Lib.ProviderExtensions;
using Tekook.BackupR.Lib.StateManagement;
using Tekook.VerbR.Resolvers;

namespace Tekook.BackupR.Verbs
{
    internal class BackupVerb : VerbR.Verb<BackupOptions, IConfig>
    {
        protected ILogger Logger { get; set; } = LogManager.GetCurrentClassLogger();
        protected IProvider Provider { get; set; }
        protected BackupState State { get; set; } = new();

        public BackupVerb(BackupOptions options) : base(options)
        {
            this.Resolver = new ConfigNetResolver<IConfig, BackupOptions>((builder) => builder.UseJsonFile(options.Config));
        }

        public override async Task<int> InvokeAsync()
        {
            try
            {
                Logger.Info("------- Starting backup -------");
                StateManager.Init(this.Config.StateFile);
                this.State.Start();
                this.Provider = Lib.Resolver.ResolveProvider(this.Config, this.Options);
                Logger.Info("Validating provider: {provider}...", this.Provider.GetType().Name);
                await this.Provider.Validate(3, Logger);
                var backup = this.Config.Backup;
                await this.Handle<FolderBackup, IFolderBackup>(backup.Folders);
                await this.Handle<CommandBackup, ICommandBackup>(backup.Commands);
                await this.Handle<MysqlBackup, IMysqlBackup>(backup.MysqlBackups);
                await this.Handle<TarBackup, ITarBackup>(backup.TarBackups);
                Logger.Info("------- Backup finished -------");
            }
            catch (ProviderException e)
            {
                this.State.HasProviderError = true;
                LogException(e);
                Logger.Warn("------ Backup could not be started! -------");
                return 1;
            }
            finally
            {
                StateManager.Stop(this.State);
                this.Provider?.Dispose();
            }
            return 0;
        }

        private async Task Handle<T, T2>(IEnumerable<T2> settings) where T : Backup where T2 : IBackup
        {
            Logger.Info("------- Handling {type:l}s: {count} -------", typeof(T).Name, settings.Count());
            foreach (T2 setting in settings)
            {
                Exception exception = null;
                if (setting.Disabled)
                {
                    Logger.Debug("Skipping disabled task: {name}", setting.Name);
                    continue;
                }
                Logger.Info("Starting: {backup_name}", setting.Name);
                T task = (T)Activator.CreateInstance(typeof(T), setting);
                try
                {
                    await this.HandleTask(task, setting);
                    Logger.Info("Finished: {backup_name}", setting.Name);
                }
                catch (BackupException e)
                {
                    Logger.Error("Backup encountered an error and could not be completed. {error}", e.Message);
                    LogException(e);
                    exception = e;
                }
                catch (ProviderException e)
                {
                    Logger.Error("Encountered an error with the provider: {error}", e.Message);
                    LogException(e);
                    exception = e;
                }
                catch (Exception e)
                {
                    Logger.Error("Unkown error caught -> {type}!", e.GetType().FullName);
                    LogException(e);
                    exception = e;
                }
                finally
                {
                    double? size = task?.BackupFile?.Length;
                    task?.RemoveBackup();
                    task?.CleanupTask();
                    var sTask = this.State.AddTask(new BackupTask()
                    {
                        Name = setting.Name,
                        Size = size ?? -1,
                        Type = typeof(T).Name
                    });
                    if (exception == null)
                    {
                        Logger.Info("------- Task: {backup_name} finished -------", setting.Name);
                        sTask.Success = true;
                    }
                    else
                    {
                        sTask.Success = false;
                        Logger.Error("------- Task: {backup_name} failed with errors. Backup has not been created. -------", setting.Name);
                    }
                }
            }
        }

        private async Task HandleTask(IBackupTask task, IBackup backup)
        {
            Logger.Debug("Backup configuration: {task:l}", task.ToString());
            await task.CreateBackup();
            if (task.BackupFile != null)
            {
                Logger.Info("Backup created with size: {size}", ByteSize.FromBytes(task.BackupFile.Length));
                var name = task.GetUploadName(backup);
                if (name != null && string.IsNullOrEmpty(Path.GetExtension(name)))
                {
                    name += task.BackupFile.Extension;
                }
                string path = this.Provider.Combine(this.Provider.RootPath, backup.UploadPath);
                Logger.Info("Uploading backup as {filename} to {path}", name, path);
                var container = await this.Provider.GetContainer(path);
                await container.Upload(task.BackupFile, name);
                Logger.Info("Upload finished");
            }
            else
            {
                throw new BackupException(task, "Task did not create any backup file.");
            }
        }

        private void LogException(Exception e)
        {
            Logger.Error(e, e.Message);
            if (e.InnerException != null)
            {
                LogException(e.InnerException);
            }
        }
    }
}