namespace Tekook.BackupR.Lib.Providers
{
    /// <summary>
    /// Configuration for an <see cref="SftpProvider"/>.
    /// </summary>
    public interface IS3Config
    {
        /// <summary>
        /// ServiceUrl of the S3 Service
        /// </summary>
        public string ServiceUrl { get; set; }

        /// <summary>
        /// Accesskey for S3
        /// </summary>
        public string AccessKey { get; set; }

        /// <summary>
        /// SecretKey for S3
        /// </summary>
        public string SecretKey { get; set; }

        /// <summary>
        /// Base path for all uploads
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Name of the bucket
        /// </summary>
        public string BucketName { get; set; }
    }
}