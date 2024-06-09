using System;

namespace Tekook.BackupR.Lib.StateManagement
{
    internal abstract class CommandState
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public void Start()
        {
            this.StartTime = DateTime.Now;
            this.StateStarted();
        }

        public void Stop()
        {
            this.EndTime = DateTime.Now;
            this.StateStoped();
        }

        protected abstract void StateStarted();

        protected abstract void StateStoped();
    }
}