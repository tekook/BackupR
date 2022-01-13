using System;
using System.IO;
using System.Threading.Tasks;
using Tekook.BackupR.Lib.Contracts;
using Tekook.BackupR.Lib.Exceptions;

namespace Tekook.BackupR.Lib.Providers
{
    public class FsProvider : IProvider, IDisposable
    {
        /// <inheritdoc/>
        public string RootPath => this.Config.Path;

        /// <inheritdoc/>
        public char Seperator { get => Path.DirectorySeparatorChar; }

        /// <summary>
        /// Configuration for this provider.
        /// </summary>
        protected IFsConfig Config { get; set; }

        /// <summary>
        /// Creates a new provider.
        /// </summary>
        /// <param name="config">Configuration this provider will use.</param>
        /// <exception cref="ArgumentNullException">If config is null</exception>
        public FsProvider(IFsConfig config)
        {
            this.Config = config ?? throw new ArgumentNullException(nameof(config));
        }

        /// <summary>
        /// Creates a new provider.
        /// </summary>
        /// <param name="options">Options to create our configuration from.</param>
        public FsProvider(IOptions options)
        {
            this.Config = Resolver.ResolveConfig<IFsConfig>(options);
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
            File.Delete(item.Path);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public async Task Download(IItem item, string localPath)
        {
            await Task.Run(() =>
            {
                File.Copy(item.Path, localPath, true);
            });
        }

        /// <inheritdoc/>
        public async Task<IContainer> GetContainer(string path, bool recursive = false)
        {
            try
            {
                FsContainer root = new FsContainer(this, path);
                FsContainer container;
                DirectoryInfo dir = new DirectoryInfo(path);
                if (!dir.Exists)
                {
                    dir.Create();
                }
                foreach (FileInfo item in dir.GetFiles())
                {
                    root.Items.Add(new FsItem(root)
                    {
                        Date = item.LastWriteTime,
                        Name = item.Name,
                        Path = item.FullName,
                        Size = item.Length
                    });
                }
                if (recursive)
                {
                    foreach (DirectoryInfo item in dir.GetDirectories())
                    {
                        container = (FsContainer)await this.GetContainer(item.FullName, recursive);
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
                if (target.Provider != this)
                {
                    throw new InvalidOperationException("Invalid container provided. Provider does not match!");
                }
                if (!Directory.Exists(target.Path))
                {
                    Directory.CreateDirectory(target.Path);
                }
                File.Copy(file.FullName, Path.Combine(target.Path, name ?? file.Name), true);
            }
            catch (IOException e)
            {
                throw new ProviderException("Failed to upload file to provider", e);
            }
        }

        /// <inheritdoc/>
        public async Task Validate()
        {
            if (!Directory.Exists(this.RootPath))
            {
                throw new ProviderException($"Target directory does not exist: {RootPath}");
            }
        }
    }
}