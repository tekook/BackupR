using System;
using System.Collections.Generic;

namespace Tekook.BackupR.Lib.StateManagement
{
    internal class BackupTask : StateTask
    {
        /// <summary>
        /// Type of the backup.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Errors of this task
        /// </summary>
        public List<Exception> Errors { get; set; } = [];
    }
}