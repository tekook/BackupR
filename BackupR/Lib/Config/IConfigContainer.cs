using Config.Net;

namespace Tekook.BackupR.Lib.Config
{
    public interface IConfigContainer
    {
        long? MaxFiles { get; }

        string MaxSize { get; }

        [Option(DefaultValue = 0L)]
        long MinFiles { get; }

        string Path { get; }
    }
}