using System;
using System.Runtime.Serialization;

namespace Tekook.BackupR.Lib.Exceptions
{
    internal class BackupException : Exception
    {
        public BackupException()
        {
        }

        public BackupException(string message) : base(message)
        {
        }

        public BackupException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected BackupException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}