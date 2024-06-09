using NLog;
using SharpCompress.Archives;
using SharpCompress.Archives.Tar;
using SharpCompress.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Tekook.BackupR.Lib.Config;

namespace Tekook.BackupR.Lib.Backups
{
    internal class FolderBackup : Backup
    {
        protected DateTime? MaxAge = null;
        protected DateTime? MinAge = null;
        protected IEnumerable<Regex> Excludes { get; set; }
        protected ILogger Logger { get; set; } = LogManager.GetCurrentClassLogger();
        protected IFolderBackup Settings { get; set; }

        public FolderBackup(IFolderBackup settings)
        {
            this.Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            this.Parse(this.Settings.MaxAge, nameof(this.Settings.MaxAge), ref this.MaxAge);
            this.Parse(this.Settings.MinAge, nameof(this.Settings.MinAge), ref this.MinAge);
            var excludes = new List<Regex>();
            foreach (string ex in this.Settings.Excludes)
            {
                excludes.Add(
                    new Regex(
                        "^" + ex
                        .Replace(@"\", @"\\")
                        .Replace(".", "[.]")
                        .Replace("*", ".*")
                        .Replace("?", ".")
                        )
                    );
            }
            this.Excludes = excludes.ToArray();
        }

        public override async Task<FileInfo> CreateBackup()
        {
            Logger.Info("Creating archive of {path}", this.Settings.Path);
            await Task.Run(() =>
            {
                string tempFile = Path.GetTempFileName();
                using (var archive = TarArchive.Create())
                {
                    using (archive.PauseEntryRebuilding())
                    {
                        var paths = Directory.EnumerateFiles(this.Settings.Path, "*.*", SearchOption.AllDirectories)
                        .Select(x => new { FileInfo = new FileInfo(x), RelPath = x[this.Settings.Path.Length..] })
                        .Where(x => this.IsFileValid(x.FileInfo, x.RelPath));
                        foreach (var path in paths)
                        {
                            if (!path.FileInfo.Exists)
                            {
                                Logger.Trace("Cannot add file: {file} because is does not exist.", path.FileInfo.FullName);
                                continue;
                            }
                            Logger.Trace("Adding file: {file}", path.FileInfo.FullName);
                            archive.AddEntry(path.RelPath, path.FileInfo.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite), true, path.FileInfo.Length,
                                            path.FileInfo.LastWriteTime);
                        }
                    }
                    Logger.Trace("Saving to archive.");
                    archive.SaveTo(tempFile, CompressionType.GZip);
                    Logger.Trace("Done.");
                }
                this.BackupFile = new FileInfo(tempFile);
                Logger.Debug("Archive created at {file}", tempFile);
            });
            return this.BackupFile;
        }

        public override string ToString()
        {
            return $"{{{this.GetType().Name}|{this.Settings.Name}: {this.Settings.Path}}}";
        }

        protected bool IsFileValid(FileInfo file, string relPath)
        {
            string trim = relPath[1..];
            if (this.Excludes.Any(x => x.IsMatch(trim)))
            {
                Logger.Trace("File {file} is excluded by Excludes", relPath);
                return false;
            }
            DateTime date = this.Settings.UseCreationDate ? file.CreationTime : file.LastWriteTime;
            if (this.MaxAge != null && date < this.MaxAge)
            {
                Logger.Trace("File {file} is excluded by MaxAge.", relPath);
                return false;
            }
            if (this.MinAge != null && date > this.MinAge)
            {
                Logger.Trace("File {file} is excluded by MinAge.", relPath);
                return false;
            }
            return true;
        }

        protected void Parse(string span, string name, ref DateTime? date)
        {
            if (string.IsNullOrEmpty(span))
            {
                Logger.Trace("No value set for " + name);
                return;
            }
            if (TimeSpan.TryParse(span, out TimeSpan ts))
            {
                date = DateTime.Now.Subtract(ts);
                Logger.Info(name + ": {" + name + "} via {field}", date, this.Settings.UseCreationDate ? "CreationTime" : "LastWriteTime");
            }
            else
            {
                Logger.Warn(name + " could not be parsed via TimeSpan.Parse. Ignoring!");
            }
        }
    }
}