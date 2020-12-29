using System.Collections.Generic;

namespace Tekook.BackupR.Lib.Contracts
{
    public interface IContainer
    {
        IEnumerable<IContainer> Containers { get; }
        IEnumerable<IItem> Items { get; }
        string Name { get; }
        string Path { get; }
        IProvider Provider { get; }
        long Size { get; }
    }
}