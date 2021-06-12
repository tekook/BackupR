using MySqlConnector;
using SharpCompress.Archives;
using SharpCompress.Archives.Tar;
using SharpCompress.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Tekook.BackupR.Lib.Config;
using Tekook.BackupR.Lib.Exceptions;

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
            string tmpdir = this.GetTempDir();
            string tmpname = Path.GetTempFileName();
            List<string> dbs = this.Settings.FetchDatabases ? await this.FetchDatabases() : new List<string>(this.Settings.Databases ?? Array.Empty<string>());
            if (dbs.Count == 0)
            {
                dbs.Add(null);
            }
            List<FileInfo> files = new List<FileInfo>();

            string name, dump;
            foreach (string db in dbs)
            {
                name = Path.Combine(tmpdir, (db ?? "-all-databases-") + ".sql");
                dump = await this.MakeDump(name, db);
                files.Add(new FileInfo(dump));
            }
            using (var archive = TarArchive.Create())
            {
                using (archive.PauseEntryRebuilding())
                {
                    foreach (var file in files)
                    {
                        archive.AddEntry(file.Name, file.OpenRead(), true, file.Length, file.LastWriteTime);
                    }
                }
                archive.SaveTo(tmpname, CompressionType.GZip);
            }
            Directory.Delete(tmpdir, true);
            this.BackupFile = new FileInfo(tmpname);
            return this.BackupFile;
        }

        private async Task<List<string>> FetchDatabases()
        {
            var x = new MySqlConnectionStringBuilder();
            x.UserID = this.Settings.Username;
            x.Password = this.Settings.Password;
            x.Server = this.Settings.Host;
            List<string> dbs = new List<string>();
            using (var connection = new MySqlConnection(x.ToString()))
            {
                await connection.OpenAsync();
                using var command = new MySqlCommand("show databases;", connection);
                using var reader = await command.ExecuteReaderAsync();
                string db;
                while (await reader.ReadAsync())
                {
                    db = reader.GetString(0);
                    if (!this.Settings.Excludes.Contains(db))
                    {
                        dbs.Add(db);
                    }
                }
            }
            return dbs;
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

        private string GetTempDir()
        {
            string file = Path.GetTempFileName();
            File.Delete(file);
            Directory.CreateDirectory(file);
            return file;
        }

        private async Task<string> MakeDump(string file, string db = null)
        {
            string args = GetArguments(file, db);
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
                return file;
            }
            else
            {
                string error = process.StandardError.ReadToEnd();
                throw new BackupException(error);
            }
        }
    }
}