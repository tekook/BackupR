using ByteSizeLib;
using NLog;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Tekook.BackupR.Lib.Config;
using Tekook.BackupR.Lib.Exceptions;

namespace Tekook.BackupR.Lib.Backups
{
    internal class CommandBackup : Backup
    {
        protected ILogger Logger { get; set; } = LogManager.GetCurrentClassLogger();
        protected ICommandBackup Settings { get; set; }

        public CommandBackup(ICommandBackup settings)
        {
            this.Settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public override async Task<FileInfo> CreateBackup()
        {
            this.RemoveBackup();
            var process = new Process
            {
                StartInfo =
                {
                    FileName = this.Settings.Command,
                    Arguments = this.Settings.Arguments,
                    WorkingDirectory = this.Settings.WorkingDirectory,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                }
            };
            Logger.Info("Starting {command} with {args}", this.Settings.Command, this.Settings.Arguments);
            process.Start();
            await process.WaitForExitAsync();
            if (process.ExitCode == 0)
            {
                string o = await process.StandardOutput.ReadLineAsync();
                if (string.IsNullOrEmpty(o) || !File.Exists(o))
                {
                    Logger.Debug("stdout: {stdout}", o);
                    Logger.Error("stderr: {stderr}", await process.StandardError.ReadToEndAsync());
                    throw new BackupException(this, "Command did not return any file or the returned file does not exist!");
                }
                this.BackupFile = new FileInfo(o);
                Logger.Debug("Command created backup successfully -> {file}", this.BackupFile.FullName);
                return this.BackupFile;
            }
            else
            {
                throw new BackupException(this, await process.StandardError.ReadToEndAsync());
            }
        }

        public override string ToString()
        {
            return $"{{{this.GetType().Name}|{this.Settings.Name}: {this.Settings.Command}}}";
        }
    }
}