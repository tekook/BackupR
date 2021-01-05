namespace Tekook.BackupR.Lib.Configs
{
    public interface IConfig
    {
        string Name { get; set; }
        string Type { get; set; }
    }

    public interface IConfig<T> : IConfig
    {
        T Provider { get; set; }
    }
}