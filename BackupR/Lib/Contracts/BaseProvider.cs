using System;
using System.Threading.Tasks;

namespace Tekook.BackupR.Lib.Contracts
{
    public abstract class BaseProvider: IHandlesException
    {
        /// <inheritdoc/>
#pragma warning disable CS1998 // Bei der asynchronen Methode fehlen "await"-Operatoren. Die Methode wird synchron ausgeführt.
        public async virtual Task HandleException(Exception exception)
#pragma warning restore CS1998 // Bei der asynchronen Methode fehlen "await"-Operatoren. Die Methode wird synchron ausgeführt.
        {
            return;
        }
    }
}
