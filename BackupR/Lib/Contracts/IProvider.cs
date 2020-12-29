using System.Threading.Tasks;

namespace Tekook.BackupR.Lib.Contracts
{
    public interface IProvider
    {
        public Task<IContainer> Read();
    }
}