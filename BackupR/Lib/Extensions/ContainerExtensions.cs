using NLog;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Tekook.BackupR.Lib.Contracts;

namespace Tekook.BackupR.Lib.Extensions
{
    public static class ContainerExtensions
    {
        public static async Task Upload(this IContainer container, int retries, FileInfo file, string name = null, ILogger logger = null, int waitBetweenRetries = 10000)
        {
            if (retries < 0)
            {
                retries *= -1;
            }
            if (retries == 0)
            {
                retries = 1;
            }
            for (int tries = 1; tries <= retries; tries++)
            {
                try
                {
                    await container.Upload(file, name);
                    break;
                }
                catch (Exception ex)
                {
                    logger?.Error("Caught exception while uploading. Error: {upload_error} | Try: {validation_try})", ex, tries + 1);
                    logger?.Error(ex);
                    if (tries == retries)
                    {
                        throw;
                    }
                    else
                    {
                        Thread.Sleep(waitBetweenRetries);
                    }
                }
            }
        }
    }
}