using System;

namespace Tekook.BackupR.Lib.Exceptions
{
    internal class ProviderException : BaseException
    {
        public ProviderException() : base()
        {
        }

        public ProviderException(string message) : base(message)
        {
        }

        public ProviderException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}