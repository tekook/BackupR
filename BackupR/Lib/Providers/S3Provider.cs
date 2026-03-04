using Amazon.S3;
using Amazon.S3.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tekook.BackupR.Lib.Contracts;

namespace Tekook.BackupR.Lib.Providers
{
    public class S3Provider : BaseProvider, IProvider, IDisposable
    {
        public string RootPath => this.Config.Path;

        public char Seperator { get; } = '/';

        protected IS3Config Config { get; set; }

        protected AmazonS3Client Client { get; set; }

        public S3Provider(IS3Config config)
        {
            this.Config = config ?? throw new ArgumentNullException(nameof(config));
            this.CreateClient();
        }

        public S3Provider(IOptions options)
        {
            this.Config = Resolver.ResolveConfig<IS3Config>(options);
            this.CreateClient();
        }

        public string Combine(params string[] paths)
        {
            return string.Join(this.Seperator, paths);
        }

        public Task Delete(IItem item)
        {
            if (item.Container.Provider != this)
            {
                throw new InvalidOperationException($"Invalid {nameof(IItem)} provided. (Invalid {nameof(IProvider)})");
            }
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Task Download(IItem item, string localPath)
        {
            throw new NotImplementedException();
        }

        public async Task<IContainer> GetContainer(string path, bool recursive = false)
        {

            S3Container root = new S3Container(this, path);
            S3Container container;
            string prefix = path.EndsWith(this.Seperator) ? path : path + this.Seperator;
            var request = new ListObjectsV2Request
            {
                BucketName = this.Config.BucketName,
                Prefix = prefix,
            };
            var objects = new List<S3Object>();
            ListObjectsV2Response response;
            do
            {
                response = await this.Client.ListObjectsV2Async(request);
                objects.AddRange(response.S3Objects);
                request.ContinuationToken = response.NextContinuationToken;

            } while (response.IsTruncated == true);
            foreach(S3Object obj in objects)
            {
                root.Items.Add(new S3Item(root)
                {
                    Date = obj.LastModified ?? DateTime.Now,
                    Name = obj.Key,
                    Path = this.Combine(prefix, obj.Key),
                    Size = obj.Size ?? 0
                });
            }
            return root;
        }

        public Task<IContainer> GetRoot()
        {
            throw new NotImplementedException();
        }

        public Task Upload(FileInfo file, IContainer target, string name = null)
        {
            throw new NotImplementedException();
        }

        public Task Validate()
        {
            try
            {

            }
        }

        protected void CreateClient()
        {
            if (this.Client == null)
            {
                var config = new AmazonS3Config
                {
                    ServiceURL = this.Config.ServiceUrl,
                    ForcePathStyle = true
                };
                this.Client = new AmazonS3Client(this.Config.AccessKey, this.Config.SecretKey, config);
            }
        }
    }
}