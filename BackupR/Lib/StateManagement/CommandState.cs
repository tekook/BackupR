using System;
using System.Collections.Generic;
using System.Linq;

namespace Tekook.BackupR.Lib.StateManagement
{
    internal abstract class CommandState<T> where T : StateTask
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public List<T> Tasks { get; set; } = [];
        public double TotalSize { get; set; } = 0;
        public bool HasProviderError { get; set; } = false;
        public bool HasFailedTasks { get; set; } = false;
        public int TotalTasks { get; set; } = 0;
        public int FailedTasks { get; set; } = 0;
        public int SuccessfullTasks { get; set; } = 0;
        public bool Success { get; set; } = false;

        public T AddTask(T task)
        {
            this.Tasks.Add(task);
            return task;
        }

        public virtual void Start()
        {
            this.StartTime = DateTime.Now;
        }

        public virtual void Stop()
        {
            this.EndTime = DateTime.Now;
            this.TotalTasks = this.Tasks.Count;
            this.SuccessfullTasks = this.Tasks.Count(x => x.Success);
            this.FailedTasks = this.Tasks.Count(x => !x.Success);
            this.HasFailedTasks = this.Tasks.Any(x => !x.Success);
            this.Success = !this.HasFailedTasks && !this.HasProviderError;
            this.TotalSize = this.Tasks.Where(x => x.Size > 0).Sum(x => x.Size);
        }
    }
}