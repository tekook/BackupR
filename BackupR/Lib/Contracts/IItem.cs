using System;
using System.Threading.Tasks;

namespace Tekook.BackupR.Lib.Contracts
{
    public interface IItem
    {
        public IContainer Container { get; }
        public DateTime Date { get; }
        public string Name { get; }
        public long Size { get; }
    }
}