using System.Runtime.Serialization;

namespace Tekook.BackupR.Lib.Exceptions
{
    internal class BaseException : System.Exception
    {
        public string ClassName { get; set; }

        public BaseException()
        {
            this.ClassName = this.GetType().Name;
        }

        public BaseException(string message) : base(message)
        {
            this.ClassName = this.GetType().Name;
        }

        public BaseException(string message, System.Exception innerException) : base(message, innerException)
        {
            this.ClassName = this.GetType().Name;
        }
    }
}