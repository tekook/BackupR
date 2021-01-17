using System.Collections.Generic;

namespace Tekook.BackupR.Lib.Configs
{
    public interface IConfig
    {
        IEnumerable<IConfigContainer> Containers { get; }
        string Name { get; set; }
        string Type { get; set; }
    }

    public interface IConfig<T> : IConfig
    {
        T Provider { get; set; }
    }
}