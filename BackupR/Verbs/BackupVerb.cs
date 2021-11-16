using ByteSizeLib;
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
using Tekook.VerbR.Resolvers;

namespace Tekook.BackupR.Verbs
{
    internal class BackupVerb : VerbR.Verb<BackupOptions, IConfig>
    {
        protected ILogger Logger { get; set; } = LogManager.GetCurrentClassLogger();
        protected IProvider Provider { get; set; }

        public BackupVerb(BackupOptions options) : base(options)
        {
            this.Resolver = new ConfigNetResolver<IConfig, BackupOptions>((builder) => builder.UseJsonFile(options.Config));
        }

        public override async Task<int> InvokeAsync()
        {
            Logger.Info("------- Starting backup -------");
            this.Provider = Lib.Resolver.ResolveProvider(this.Config, this.Options);
            Logger.Debug("Provider: {provider}", this.Provider.GetType().Name);
            var backup = this.Config.Backup;
            await this.Handle<FolderBackup, IFolderBackup>(backup.Folders);
            await this.Handle<CommandBackup, ICommandBackup>(backup.Commands);
            await this.Handle<MysqlBackup, IMysqlBackup>(backup.MysqlBackups);
            Logger.Info("------- Backup finished -------");
            return 0;
        }

        private async Task Handle<T, T2>(IEnumerable<T2> settings) where T : Backup where T2 : IBackup
        {
            Logger.Info("------- Handling {type:l}s: {count} -------", typeof(T).Name, settings.Count());
            foreach (T2 setting in settings)
            {
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
                }
                catch (ProviderException e)
                {
                    LogException(e);
                }
                catch (Exception e)
                {
                    Logger.Error("Unkown error caught -> {type}!", e.GetType().FullName);
                    LogException(e);
                }
                finally
                {
                    task?.RemoveBackup();
                    task?.CleanupTask();
                    Logger.Info("------- Task done -------");
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
                string path = Path.Combine(this.Provider.RootPath, backup.UploadPath);
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