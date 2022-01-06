﻿using System.Collections.Generic;

namespace Tekook.BackupR.Lib.Config
{
    public interface IConfig
    {
        IBackupConfig Backup { get; }
        IEnumerable<IContainerConfig> Containers { get; }
        string Name { get; }
        string StateFile { get; }
        string Type { get; }
    }

    public interface IConfig<T> : IConfig
    {
        T Provider { get; }
    }
}