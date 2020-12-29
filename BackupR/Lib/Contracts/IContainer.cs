using System.Collections.Generic;

namespace Tekook.BackupR.Lib.Contracts
{
    public interface IContainer
    {
        public IEnumerable<IContainer> Containers { get; }
        public IEnumerable<IItem> Items { get; }
        public string Name { get; }
        public string Path { get; }
        public IProvider Provider { get; }
        public long Size { get; }
    }
}