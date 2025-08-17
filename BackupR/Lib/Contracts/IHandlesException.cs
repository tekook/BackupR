using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tekook.BackupR.Lib.Contracts
{
    public interface IHandlesException
    {
        /// <summary>
        /// Lets the Provider implement some custom logic for handling exceptions.
        /// </summary>
        /// <param name="exception">The Exception which got caught</param>
        /// <returns></returns>
        Task HandleException(Exception exception);
    }
}
