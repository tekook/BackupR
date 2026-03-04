using System;
using System.Threading.Tasks;
using Tekook.BackupR.Lib.Contracts;

namespace Tekook.BackupR.Lib.Providers
{
    public class S3Item : IItem
    {
        public S3Container Container { get; }

        public DateTime Date { get; set; }

        public bool Deleted { get; protected set; }

        public string Name { get; set; }

        public string Path { get; set; }

        public long Size { get; set; }

        IContainer IItem.Container => this.Container;

        public S3Item(S3Container container)
        {
            this.Container = container ?? throw new ArgumentNullException(nameof(container));
        }

        public Task Delete()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return $"{this.GetType().Name}:{this.Name} ({ByteSizeLib.ByteSize.FromBytes(this.Size)})";
        }
    }
}