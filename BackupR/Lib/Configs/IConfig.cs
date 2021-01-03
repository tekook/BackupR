namespace Tekook.BackupR.Lib.Configs
{
    public interface IConfig<TConfig>
    {
        IProvider<TConfig> Provider { get; set; }
    }
}