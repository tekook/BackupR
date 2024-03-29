﻿namespace Tekook.BackupR.Lib.Providers
{
    /// <summary>
    /// Configuration for an <see cref="FsProvider"/>.
    /// </summary>
    public interface IFsConfig
    {
        /// <summary>
        /// Root path to connect to.
        /// </summary>
        public string Path { get; set; }
    }
}