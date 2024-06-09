using System;

namespace Tekook.BackupR.Lib.StateManagement
{
    internal abstract class StateTimer
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public void Start()
        {
            this.StartTime = DateTime.Now;
        }

        public void Stop()
        {
            this.EndTime = DateTime.Now;
        }
    }
}