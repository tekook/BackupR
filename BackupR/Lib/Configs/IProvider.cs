namespace Tekook.BackupR.Lib.Configs
{
    public interface IProvider<T>
    {
        T Config { get; }
        string ConfigType { get; }
        string Type { get; }
    }
}