using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Tekook.BackupR.Lib.Config;

namespace Tekook.BackupR.Lib.Backups
{
    internal class CommandBackup : Backup
    {
        protected ICommandBackup Settings { get; set; }

        public CommandBackup(ICommandBackup settings)
        {
            this.Settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public override async Task<FileInfo> CreateBackup()
        {
            var process = new Process
            {
                StartInfo =
                {
                    FileName = this.Settings.Command,
                    Arguments = this.Settings.Arguments,
                    WorkingDirectory = this.Settings.WorkingDirectory,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true
                }
            };
            process.Start();
            await process.WaitForExitAsync();
            if (process.ExitCode == 0)
            {
                string o = await process.StandardOutput.ReadLineAsync();
                return new FileInfo(o);
            }
            return null;
        }
    }
}