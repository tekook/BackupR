using System.Collections.Generic;

namespace Tekook.BackupR.Lib.Configs
{
    public interface IConfig
    {
        List<IConfigContainer> Containers { get; set; }
        string Name { get; set; }
        string Type { get; set; }
    }

    public interface IConfig<T> : IConfig
    {
        T Provider { get; set; }
    }
}