﻿using FluentFTP;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Tekook.BackupR.Lib.Contracts;

namespace Tekook.BackupR.Lib.Ftp
{
    public class FtpProvider : IProvider, IDisposable
    {
        /// <summary>
        /// FtpClient this provider uses.
        /// </summary>
        protected FtpClient Client { get; set; }

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
        public async Task Delete(IItem item)
        {
            if (item.Container.Provider != this)
            {
                throw new InvalidOperationException($"Invalid {nameof(IItem)} provided. (Invalid {nameof(IProvider)})");
            }
            await this.Client.DeleteFileAsync(item.Path);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.Client?.Disconnect();
            this.Client?.Dispose();
        }

        /// <inheritdoc/>
        public async Task<IContainer> GetRoot()
        {
            this.Client = new FtpClient(this.Config.Host);
            var creds = this.Config.Username != null && this.Config.Password != null ? new NetworkCredential(this.Config.Username, this.Config.Password) : null;
            if (creds != null)
            {
                this.Client.Credentials = creds;
            }
            await this.Client.ConnectAsync();
            return await GetContainer(this.Config.Path);
        }

        /// <inheritdoc/>
        public async Task Upload(FileInfo file, IContainer target, string name = null)
        {
            if (target.Provider != this)
            {
                throw new InvalidOperationException("Invalid container provided. Provider does not match!");
            }
            await this.Client.UploadAsync(file.OpenRead(), Path.Combine(target.Path, name ?? file.Name));
        }

        /// <summary>
        /// Reads the Container via ftp recursivly
        /// </summary>
        /// <param name="path">Path to read</param>
        /// <returns>Read container.</returns>
        private async Task<FtpContainer> GetContainer(string path, FtpContainer rootParent = null)
        {
            FtpContainer root = new FtpContainer(this, path);
            if (rootParent == null)
            {
                rootParent = root;
            }
            FtpContainer container;
            foreach (FtpListItem item in await this.Client.GetListingAsync(path))
            {
                if (item.Type == FtpFileSystemObjectType.File)
                {
                    root.Items.Add(new FtpItem(root)
                    {
                        Date = await this.Client.GetModifiedTimeAsync(item.FullName),
                        Name = item.Name,
                        Path = item.FullName,
                        Size = await this.Client.GetFileSizeAsync(item.FullName),
                    });
                }
                else if (item.Type == FtpFileSystemObjectType.Directory)
                {
                    container = await this.GetContainer(item.FullName, rootParent);
                    root.Containers.Add(container);
                    rootParent.AllContainers.Add(container);
                }
            }
            return root;
        }
    }
}