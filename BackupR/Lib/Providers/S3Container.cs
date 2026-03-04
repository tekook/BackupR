using Tekook.BackupR.Lib.Contracts;

namespace Tekook.BackupR.Lib.Providers
{
    public class S3Container : Container<S3Provider, S3Container, S3Item>, IContainer
    {
        public S3Container(S3Provider provider, string path) : base(provider, path)
        {
        }
    }
}