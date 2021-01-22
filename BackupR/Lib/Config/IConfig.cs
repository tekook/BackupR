using System.Collections.Generic;

namespace Tekook.BackupR.Lib.Config
{
    public interface IConfig
    {
        IConfigBackup Backup { get; }
        IEnumerable<IConfigContainer> Containers { get; }
        string Name { get; }
        string Type { get; }
    }

    public interface IConfig<T> : IConfig
    {
        T Provider { get; }
    }
}