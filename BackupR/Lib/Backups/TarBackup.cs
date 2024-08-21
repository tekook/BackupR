using NLog;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Tekook.BackupR.Lib.Config;
using Tekook.BackupR.Lib.Exceptions;

namespace Tekook.BackupR.Lib.Backups
{
    internal class TarBackup : Backup
    {
        protected ITarBackup Settings { get; set; }
        protected ILogger Logger { get; set; } = LogManager.GetCurrentClassLogger();

        public TarBackup(ITarBackup settings)
        {
            this.Settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public override async Task<FileInfo> CreateBackup()
        {
            Logger.Info("Creating tar archive of {path}", this.Settings.Path);
            this.TempFile = Path.GetTempFileName();
            StringBuilder sb = new();
            sb.Append("--create");
            if (this.Settings.Compress)
            {
                sb.Append(" --gzip");
            }
            foreach (string exclude in this.Settings.Excludes)
            {
                sb.Append($" --exclude={exclude}");
            }
            sb.Append($" --file={this.TempFile}");
            if (!string.IsNullOrEmpty(this.Settings.AdditionalOptions))
            {
                sb.Append($" {this.Settings.AdditionalOptions}");
            }
            sb.Append($" -C {this.Settings.Path} .");

            var process = new Process
            {
                StartInfo =
                {
                    FileName = "tar",
                    Arguments = sb.ToString(),
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                }
            };
            Logger.Trace("Starting {command} with args {args}", process.StartInfo.FileName, process.StartInfo.Arguments);
            process.Start();
            await process.WaitForExitAsync();
            if (process.ExitCode != 0)
            {
                var stdout = await process.StandardOutput.ReadToEndAsync();
                var stderr = await process.StandardError.ReadToEndAsync();
                if (process.ExitCode == 1)
                {
                    Logger.Warn("Tar Command Result: 'Some files differ' / {exitcode} - see logs for details", process.ExitCode);
                    Logger.Debug("stdout: {stdout}", stdout);
                    Logger.Warn("stderr: {stderr}", stderr);
                }
                else
                {
                    Logger.Error("Tar command ran into an error (ExitCode: {exitcode} - see log for details", process.ExitCode);
                    Logger.Debug("stdout: {stdout}", stdout);
                    Logger.Error("stderr: {stderr}", stderr);
                    throw new BackupException(this, stderr);
                }
            }
            this.BackupFile = new FileInfo(this.TempFile);
            return this.BackupFile;
        }

        public override string ToString()
        {
            return $"{{{this.GetType().Name}|{this.Settings.Name}: {this.Settings.Path}}}";
        }
    }
}