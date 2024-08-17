using FluentFTP;
using FluentFTP.Exceptions;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Tekook.BackupR.Lib.Contracts;
using Tekook.BackupR.Lib.Exceptions;

namespace Tekook.BackupR.Lib.Providers
{
    public class FtpProvider : IProvider, IDisposable
    {
        /// <inheritdoc/>
        public string RootPath => this.Config.Path;

        /// <inheritdoc/>
        public char Seperator { get; } = '/';

        /// <summary>
        /// FtpClient this provider uses.
        /// </summary>
        protected AsyncFtpClient Client { get; set; }

        /// <summary>
        /// Configuration for this provider.
        /// </summary>
        protected IFtpConfig Config { get; set; }

        /// <summary>
        /// Creates a new provider.
        /// </summary>
        /// <param name="config">Configuration this provider will use.</param>
        /// <exception cref="ArgumentNullException">If config is null</exception>
        public FtpProvider(IFtpConfig config)
        {
            this.Config = config ?? throw new ArgumentNullException(nameof(config));
        }

        /// <summary>
        /// Creates a new provider.
        /// </summary>
        /// <param name="options">Options to create our configuration from.</param>
        public FtpProvider(IOptions options)
        {
            this.Config = Resolver.ResolveConfig<IFtpConfig>(options);
        }

        /// <inheritdoc/>
        public string Combine(params string[] paths)
        {
            return Path.Combine(paths);
        }

        /// <inheritdoc/>
        public async Task Delete(IItem item)
        {
            if (item.Container.Provider != this)
            {
                throw new InvalidOperationException($"Invalid {nameof(IItem)} provided. (Invalid {nameof(IProvider)})");
            }
            await this.Client.DeleteFile(item.Path);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.Client?.Disconnect();
            this.Client?.Dispose();
            GC.SuppressFinalize(this);
        }

        public async Task Download(IItem item, string localPath)
        {
            await this.EnsureClientConnected();
            using var stream = File.OpenWrite(localPath);
            await this.Client.DownloadStream(stream, item.Path);
        }

        /// <inheritdoc/>
        public async Task<IContainer> GetContainer(string path, bool recursive = false)
        {
            try
            {
                await this.EnsureClientConnected();
                FtpContainer root = new FtpContainer(this, path);
                FtpContainer container;
                if (!await this.Client.DirectoryExists(path))
                {
                    await this.Client.CreateDirectory(path);
                }
                foreach (FtpListItem item in await this.Client.GetListing(path))
                {
                    if (item.Type == FtpObjectType.File)
                    {
                        root.Items.Add(new FtpItem(root)
                        {
                            Date = await this.Client.GetModifiedTime(item.FullName),
                            Name = item.Name,
                            Path = item.FullName,
                            Size = await this.Client.GetFileSize(item.FullName),
                        });
                    }
                    else if (item.Type == FtpObjectType.Directory && recursive)
                    {
                        container = (FtpContainer)await this.GetContainer(item.FullName, recursive);
                        root.Containers.Add(container);
                    }
                }
                return root;
            }
            catch (Exception e)
            {
                throw new ProviderException(e.Message, e);
            }
        }

        /// <inheritdoc/>
        public async Task<IContainer> GetRoot()
        {
            try
            {
                await this.EnsureClientConnected();
                return await GetContainer(this.Config.Path, true);
            }
            catch (Exception e)
            {
                throw new ProviderException(e.Message, e);
            }
        }

        /// <inheritdoc/>
        public async Task Upload(FileInfo file, IContainer target, string name = null)
        {
            try
            {
                await this.EnsureClientConnected();
                if (target.Provider != this)
                {
                    throw new InvalidOperationException("Invalid container provided. Provider does not match!");
                }
                using FileStream stream = file.OpenRead();
                await this.Client.UploadStream(stream, Path.Combine(target.Path, name ?? file.Name));
            }
            catch (FtpException e)
            {
                throw new ProviderException("Failed to upload file to provider", e);
            }
        }

        /// <inheritdoc/>
        public async Task Validate()
        {
            try
            {
                await this.EnsureClientConnected();
            }
            catch (Exception e)
            {
                throw new ProviderException($"Could not connected to ftp host -> {e.Message}", e);
            }
        }

        /// <summary>
        /// Ensures that the <see cref="FtpClient"/> is connected.
        /// </summary>
        /// <returns></returns>
        protected async Task EnsureClientConnected()
        {
            if (this.Client == null)
            {
                this.Client = new AsyncFtpClient(this.Config.Host)
                {
                    Port = this.Config.Port
                };
                var creds = this.Config.Username != null && this.Config.Password != null ? new NetworkCredential(this.Config.Username, this.Config.Password) : null;
                if (creds != null)
                {
                    this.Client.Credentials = creds;
                }
            }
            if (!this.Client.IsConnected)
            {
                await this.Client.Connect();
            }
        }
    }
}