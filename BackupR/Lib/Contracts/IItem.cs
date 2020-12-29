using System;
using System.Threading.Tasks;

namespace Tekook.BackupR.Lib.Contracts
{
    public interface IItem
    {
        IContainer Container { get; }
        DateTime Date { get; }
        string FullName { get; }
        string Name { get; }
        long Size { get; }

        Task Delete();
    }
}