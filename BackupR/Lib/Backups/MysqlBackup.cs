using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Tekook.BackupR.Lib.Config;

namespace Tekook.BackupR.Lib.Backups
{
    internal class MysqlBackup : Backup
    {
        protected IMysqlBackup Settings { get; set; }

        public MysqlBackup(IMysqlBackup settings)
        {
            this.Settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public override async Task<FileInfo> CreateBackup()
        {
            string tmpname = Path.GetTempFileName();
            string args = GetArguments(tmpname);
            var process = new Process
            {
                StartInfo =
                {
                    FileName = this.Settings.MysqlDumpPath,
                    Arguments = args,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                }
            };
            process.Start();
            await process.WaitForExitAsync();
            if (process.ExitCode == 0)
            {
                this.BackupFile = new FileInfo(tmpname);
                return this.BackupFile;
            }
            else
            {
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
            }
            return null;
        }

        private string GetArguments(string file, string db = null)
        {
            List<string> args = new List<string>();
            if (this.Settings.AddLocks)
            {
                args.Add("--add-locks");
            }
            if (this.Settings.Events)
            {
                args.Add("--events");
            }
            if (this.Settings.Routines)
            {
                args.Add("--routines");
            }
            if (this.Settings.Triggers)
            {
                args.Add("--triggers");
            }
            if (this.Settings.FlushPrivileges)
            {
                args.Add("--flush-privileges");
            }
            if (!string.IsNullOrEmpty(this.Settings.Password))
            {
                args.Add($"--password=\"{this.Settings.Password}\"");
            }
            if (!string.IsNullOrEmpty(this.Settings.Username))
            {
                args.Add($"--user={this.Settings.Username}");
            }
            if (!string.IsNullOrEmpty(this.Settings.Host))
            {
                args.Add($"--host={this.Settings.Host}");
            }
            if (this.Settings.ColumnStatistics)
            {
                args.Add("--column-statistics");
            }
            else
            {
                args.Add("--column-statistics=0");
            }
            if (string.IsNullOrEmpty(db))
            {
                args.Add("--all-databases");
            }
            else
            {
                args.Add(db);
            }
            args.Add($"--result-file=\"{file}\"");
            return string.Join(' ', args);
        }
    }
}