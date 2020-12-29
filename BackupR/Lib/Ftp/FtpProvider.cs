using FluentFTP;
using System;
using System.Threading.Tasks;
using Tekook.BackupR.Lib.Contracts;

namespace Tekook.BackupR.Lib.Ftp
{
    public class FTPProvider : IProvider, IDisposable
    {
        protected FtpClient Client { get; set; }
        protected FtpConfig Config { get; set; }

        public FTPProvider(FtpConfig config)
        {
            this.Config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public async Task Delete(IItem item)
        {
            if (item.Container.Provider != this)
            {
                throw new InvalidOperationException($"Invalid {nameof(IItem)} provided. (Invalid {nameof(IProvider)})");
            }
            await this.Client.DeleteFileAsync(item.FullName);
        }

        public void Dispose()
        {
            this.Client?.Dispose();
        }

        public async Task<IContainer> Read()
        {
            this.Client = new FtpClient(this.Config.Host);
            var creds = this.Config.GetNetworkCredential();
            if (creds != null)
            {
                this.Client.Credentials = creds;
            }
            await this.Client.ConnectAsync();
            return await GetContainer(this.Config.Path);
        }

        private async Task<FtpContainer> GetContainer(string path)
        {
            FtpContainer root = new FtpContainer(this, path);
            foreach (FtpListItem item in await this.Client.GetListingAsync(path))
            {
                if (item.Type == FtpFileSystemObjectType.File)
                {
                    root.Items.Add(new FtpItem(root)
                    {
                        Date = await this.Client.GetModifiedTimeAsync(item.FullName),
                        Name = item.Name,
                        FullName = item.FullName,
                        Size = await this.Client.GetFileSizeAsync(item.FullName),
                    });
                }
                else if (item.Type == FtpFileSystemObjectType.Directory)
                {
                    root.Containers.Add(await this.GetContainer(item.FullName));
                }
            }
            return root;
        }
    }
}