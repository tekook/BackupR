using Renci.SshNet;
using Renci.SshNet.Sftp;
using System;
using System.IO;
using System.Threading.Tasks;
using Tekook.BackupR.Lib.Contracts;
using Tekook.BackupR.Lib.Exceptions;

namespace Tekook.BackupR.Lib.Providers
{
    public class SftpProvider : IProvider, IDisposable
    {
        /// <inheritdoc/>
        public string RootPath => this.Config.Path;

        protected SftpClient Client { get; set; }

        /// <summary>
        /// Configuration for this provider.
        /// </summary>
        protected ISftpConfig Config { get; set; }

        /// <summary>
        /// Creates a new provider.
        /// </summary>
        /// <param name="config">Configuration this provider will use.</param>
        /// <exception cref="ArgumentNullException">If config is null</exception>
        public SftpProvider(ISftpConfig config)
        {
            this.Config = config ?? throw new ArgumentNullException(nameof(config));
        }

        /// <summary>
        /// Creates a new provider.
        /// </summary>
        /// <param name="options">Options to create our configuration from.</param>
        public SftpProvider(IOptions options)
        {
            this.Config = Resolver.ResolveConfig<ISftpConfig>(options);
        }

        /// <inheritdoc/>
        public string Combine(params string[] paths)
        {
            return Path.Combine(paths).Replace('\\', '/');
        }

        /// <inheritdoc/>
        public async Task Delete(IItem item)
        {
            if (item.Container.Provider != this)
            {
                throw new InvalidOperationException($"Invalid {nameof(IItem)} provided. (Invalid {nameof(IProvider)})");
            }
            await Task.Run(() => this.Client.DeleteFile(item.Path));
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.Client?.Disconnect();
            this.Client?.Dispose();
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc/>
        public async Task<IContainer> GetContainer(string path, bool recursive = false)
        {
            try
            {
                path = Combine(path);
                await this.EnsureClientConnected();
                SftpContainer root = new SftpContainer(this, path);
                await Task.Run(() =>
                {
                    if (!this.Client.Exists(path))
                    {
                        this.Client.CreateDirectory(path);
                    }
                });
                SftpContainer container;
                foreach (SftpFile item in await Task.Run(() => this.Client.ListDirectory(path)))
                {
                    if (item.IsRegularFile)
                    {
                        root.Items.Add(new SftpItem(root)
                        {
                            Date = item.LastWriteTime,
                            Name = item.Name,
                            Path = item.FullName,
                            Size = item.Length
                        });
                    }
                    else if (item.IsDirectory && recursive)
                    {
                        container = (SftpContainer)await this.GetContainer(item.FullName, recursive);
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
            await this.EnsureClientConnected();
            if (target.Provider != this)
            {
                throw new InvalidOperationException("Invalid container provided. Provider does not match!");
            }
            using FileStream stream = file.OpenRead();
            await Task.Run(() => this.Client.UploadFile(stream, Combine(target.Path, name ?? file.Name)));
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
                throw new ProviderException($"Could not connected to sftp host -> {e.Message}", e);
            }
        }

        /// <summary>
        /// Ensures that the <see cref="SftpClient"/> is connected.
        /// </summary>
        /// <returns></returns>
        protected async Task EnsureClientConnected()
        {
            if (this.Client == null)
            {
                this.Client = new SftpClient(this.Config.Host, this.Config.Port ?? 22, this.Config.Username, this.Config.Password);
            }
            if (!this.Client.IsConnected)
            {
                await Task.Run(this.Client.Connect);
            }
        }
    }
}