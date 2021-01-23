using Config.Net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Tekook.BackupR.Lib.Backups;
using Tekook.BackupR.Lib.Config;
using Tekook.BackupR.Lib.Contracts;
using Tekook.VerbR.Resolvers;

namespace Tekook.BackupR.Verbs.Backup
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
            await this.HandleFolders(backup.Folders);
            await this.HandleCommands(backup.Commands);
            await this.HandleMysqlBackups(backup.MysqlBackups);
            return 0;
        }

        private async Task HandleBackupFile(IBackupTask task, IBackup backup)
        {
            await task.CreateBackup();
            var container = await this.Provider.GetContainer(Path.Combine(this.Provider.RootPath, backup.UploadPath));
            var name = backup.UploadName?.Replace("$date", DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"), StringComparison.CurrentCultureIgnoreCase);
            if (name != null && string.IsNullOrEmpty(Path.GetExtension(name)))
            {
                name += task.BackupFile.Extension;
            }
            await container.Upload(task.BackupFile, name);
            task.RemoveBackup();
        }

        private async Task HandleCommands(IEnumerable<ICommandBackup> commands)
        {
            foreach (ICommandBackup command in commands)
            {
                IBackupTask task = new CommandBackup(command);
                await this.HandleBackupFile(task, command);
            }
        }

        private async Task HandleFolders(IEnumerable<IFolderBackup> folders)
        {
            foreach (IFolderBackup folder in folders)
            {
                var task = new FolderBackup(folder);
                await this.HandleBackupFile(task, folder);
            }
        }

        private async Task HandleMysqlBackups(IEnumerable<IMysqlBackup> mysqlBackups)
        {
        }
    }
}