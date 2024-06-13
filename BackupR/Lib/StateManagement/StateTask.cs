using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tekook.BackupR.Lib.StateManagement
{
    internal class StateTask
    {
        public bool Success { get; set; }
        public double Size { get; set; } = 0;
        public string Name { get; set; }
    }
}