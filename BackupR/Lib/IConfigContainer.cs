namespace Tekook.BackupR.Lib
{
    public interface IConfigContainer
    {
        long MaxFiles { get; }
        string MaxSize { get; }
        string Path { get; }
    }
}