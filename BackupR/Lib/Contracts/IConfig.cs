namespace Tekook.BackupR.Lib.Contracts
{
    public interface IConfig<T>
    {
        IProvider<T> Provider { get; set; }
    }
}