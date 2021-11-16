using System.Runtime.Serialization;

namespace Tekook.BackupR.Lib.Exceptions
{
    internal class BaseException : System.Exception
    {
        public BaseException()
        {
        }

        public BaseException(string message) : base(message)
        {
        }

        public BaseException(string message, System.Exception innerException) : base(message, innerException)
        {
        }

        protected BaseException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}