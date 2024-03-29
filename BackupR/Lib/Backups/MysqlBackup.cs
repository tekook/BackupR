﻿using MySqlConnector;
using NLog;
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
        protected ILogger Logger { get; set; } = LogManager.GetCurrentClassLogger();
        protected IMysqlBackup Settings { get; set; }
        protected string TempDirectory { get; set; }

        public MysqlBackup(IMysqlBackup settings)
        {
            this.Settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public override void CleanupTask()
        {
            base.CleanupTask();
            if (!string.IsNullOrEmpty(this.TempDirectory) && Directory.Exists(this.TempDirectory))
            {
                Directory.Delete(this.TempDirectory, true);
            }
        }

        public override async Task<FileInfo> CreateBackup()
        {
            this.CleanupTask();
            this.TempDirectory = GetTempDir();
            this.TempFile = Path.GetTempFileName();
            Logger.Info("Enumerating databases to backup...");
            List<string> dbs = this.Settings.FetchDatabases ? await this.FetchDatabases() : new List<string>(this.Settings.Databases ?? Array.Empty<string>());
            if (dbs.Count == 0)
            {
                dbs.Add(null);
            }
            List<FileInfo> files = new();
            Logger.Info("Databases to backup: {database_count}", dbs.Count);
            string name, dump;
            foreach (string db in dbs)
            {
                name = Path.Combine(this.TempDirectory, (db ?? "-all-databases-") + ".sql");
                dump = await this.MakeDump(name, db);
                files.Add(new FileInfo(dump));
            }
            Logger.Debug("All databases dumped. Starting to create archive.");
            await Task.Run(() =>
            {
                using var archive = TarArchive.Create();
                using (archive.PauseEntryRebuilding())
                {
                    foreach (var file in files)
                    {
                        Logger.Trace("Adding {file}", file.Name);
                        archive.AddEntry(file.Name, file.OpenRead(), true, file.Length, file.LastWriteTime);
                    }
                }
                Logger.Trace("Saving archive.");
                archive.SaveTo(TempFile, CompressionType.GZip);
                Logger.Trace("Done.");
            });
            Directory.Delete(this.TempDirectory, true);
            this.BackupFile = new FileInfo(TempFile);
            return this.BackupFile;
        }

        public override string ToString()
        {
            List<string> config = new();
            if (this.Settings.FetchDatabases)
            {
                config.Add("fetch-databases");
            }
            if (this.Settings.Databases.Count() > 0 && !this.Settings.FetchDatabases)
            {
                config.Add($"databases: [{string.Join(", ", this.Settings.Databases)}]");
            }
            if (this.Settings.Excludes.Count() > 0)
            {
                config.Add($"excludes: [{string.Join(", ", this.Settings.Excludes)}]");
            }
            return "{" +
                $"{this.GetType().Name}|{this.Settings.Name}: {this.Settings.Host} " +
                string.Join(", ", config) +
                "}";
        }

        private static string GetTempDir()
        {
            string file = Path.GetTempFileName();
            File.Delete(file);
            Directory.CreateDirectory(file);
            return file;
        }

        private async Task<List<string>> FetchDatabases()
        {
            try
            {
                Logger.Debug("Connecting to {host} to fetch available databases.", this.Settings.Host);
                var x = new MySqlConnectionStringBuilder();
                x.UserID = this.Settings.Username;
                x.Password = this.Settings.Password;
                x.Server = this.Settings.Host;
                List<string> dbs = new();
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
                Logger.Debug("Fetched databases: [{databases}]", string.Join(", ", dbs));
                return dbs;
            }
            catch (MySqlException e)
            {
                throw new BackupException(this, "Failed to enumerate databases", e);
            }
        }

        private string GetArguments(string file, string db = null, bool showPassword = false)
        {
            List<string> args = new();
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
            if (!this.Settings.PasswordViaEnvironment && !string.IsNullOrEmpty(this.Settings.Password))
            {
                string pw = !showPassword ? "*********" : this.Settings.Password;
                args.Add($"--password=\"{pw}\"");
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

        private async Task<string> MakeDump(string file, string db = null)
        {
            Logger.Debug("Dumping database {database}", db);
            Logger.Debug($"Command: {GetArguments(file, db)}");
            string args = GetArguments(file, db, true);

            var process = new Process
            {
                StartInfo =
                {
                    FileName = this.Settings.MysqlDumpPath,
                    Arguments = args,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardError = true
                }
            };
            if (this.Settings.PasswordViaEnvironment && !string.IsNullOrEmpty(this.Settings.Password))
            {
                process.StartInfo.EnvironmentVariables["MYSQL_PWD"] = this.Settings.Password;
            }
            process.Start();
            await process.WaitForExitAsync();
            if (process.ExitCode == 0)
            {
                return file;
            }
            else
            {
                string error = await process.StandardError.ReadToEndAsync();
                Logger.Trace("mysqldump stderr: {stderr}", error.Trim());
                throw new BackupException(this, error.Trim());
            }
        }
    }
}