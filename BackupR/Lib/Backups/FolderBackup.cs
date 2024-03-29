﻿using NLog;
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
        protected IEnumerable<Regex> Excludes { get; set; }
        protected ILogger Logger { get; set; } = LogManager.GetCurrentClassLogger();
        protected IFolderBackup Settings { get; set; }

        public FolderBackup(IFolderBackup settings)
        {
            this.Settings = settings ?? throw new ArgumentNullException(nameof(settings));
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
                        foreach (string path in Directory.EnumerateFiles(this.Settings.Path, "*.*", SearchOption.AllDirectories))
                        {
                            string p = path[this.Settings.Path.Length..];
                            string pt = p.Trim(Path.DirectorySeparatorChar);
                            if (!this.Excludes.Any(x => x.IsMatch(pt)))
                            {
                                var fileInfo = new FileInfo(path);
                                Logger.Trace("Adding file: {file}", p);
                                archive.AddEntry(p, fileInfo.OpenRead(), true, fileInfo.Length,
                                                fileInfo.LastWriteTime);
                            }
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
    }
}