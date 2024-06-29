using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tekook.BackupR.Lib.StateManagement
{
    internal class StateTask
    {
        /// <summary>
        /// Determinates if the task was a success.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Size of the Task.
        /// Can be size of created backups or freed up space.
        /// </summary>
        public double Size { get; set; } = 0;

        /// <summary>
        /// Name/Identifier of the task.
        /// </summary>
        public string Name { get; set; }
    }
}