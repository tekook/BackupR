using System.Threading.Tasks;

namespace Tekook.BackupR.Lib.Contracts
{
    public interface IProvider
    {
        Task Delete(IItem item);

        Task<IContainer> Read();
    }
}