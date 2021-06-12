using Config.Net;
using System;
using System.Collections.Generic;
using System.IO;
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
        protected IProvider Provider { get; set; }

        public BackupVerb(BackupOptions options) : base(options)
        {
            this.Resolver = new ConfigNetResolver<IConfig, BackupOptions>((builder) => builder.UseJsonFile(options.Config));
        }

        public override async Task<int> InvokeAsync()
        {
            this.Provider = Lib.Resolver.ResolveProvider(this.Config, this.Options);
            var backup = this.Config.Backup;
            await this.Handle<FolderBackup, IFolderBackup>(backup.Folders);
            await this.Handle<CommandBackup, ICommandBackup>(backup.Commands);
            await this.Handle<MysqlBackup, IMysqlBackup>(backup.MysqlBackups);
            return 0;
        }

        private async Task Handle<T, T2>(IEnumerable<T2> settings) where T : Backup where T2 : IBackup
        {
            foreach (T2 setting in settings)
            {
                T task = (T)Activator.CreateInstance(typeof(T), setting);
                await this.HandleTask(task, setting);
            }
        }

        private async Task HandleTask(IBackupTask task, IBackup backup)
        {
            await task.CreateBackup();
            var container = await this.Provider.GetContainer(Path.Combine(this.Provider.RootPath, backup.UploadPath));
            var name = task.GetUploadName(backup);
            if (name != null && string.IsNullOrEmpty(Path.GetExtension(name)))
            {
                name += task.BackupFile.Extension;
            }
            if (task.BackupFile != null)
            {
                await container.Upload(task.BackupFile, name);
                task.RemoveBackup();
            }
            else
            {
                throw new BackupException("no backupfile created");
            }
        }
    }
}