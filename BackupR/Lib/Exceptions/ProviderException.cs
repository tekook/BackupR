using System;
using System.Runtime.Serialization;

namespace Tekook.BackupR.Lib.Exceptions
{
    internal class ProviderException : BaseException
    {
        public ProviderException()
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