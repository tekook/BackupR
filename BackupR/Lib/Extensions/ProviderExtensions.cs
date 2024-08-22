using NLog;
using SharpCompress.Archives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tekook.BackupR.Lib.Contracts;

namespace Tekook.BackupR.Lib.Extensions
{
    public static class ProviderExtensions
    {
        public static async Task Validate(this IProvider provider, int retries, ILogger logger = null, int waitBetweenRetries = 10000)
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
                    await provider.Validate();
                    break;
                }
                catch (Exception ex)
                {
                    logger?.Error("Caught exception while trying to validate provider. Error: {validation_error} | Try: {validation_try})", ex, tries + 1);
                    logger?.Error(ex);
                    if (tries == retries)
                    {
                        throw;
                    } else
                    {
                        Thread.Sleep(waitBetweenRetries);
                    }

                }
            }
        }
    }
}