using System;
using System.Collections.Generic;
using System.Linq;

namespace Tekook.BackupR.Lib.StateManagement
{
    internal abstract class CommandState
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public List<StateTask> Tasks { get; set; } = new();
        public double TotalSize { get; set; } = 0;
        public bool HasProviderError { get; set; } = false;
        public bool HasFailedTasks { get; set; } = false;
        public int TotalTasks { get; set; } = 0;
        public int FailedTasks { get; set; } = 0;
        public int SuccessfullTasks { get; set; } = 0;
        public bool Success { get; set; } = false;

        public StateTask AddTask(string name, bool success = false, double size = 0)
        {
            StateTask task = new()
            {
                Name = name,
                Success = success,
                Size = size
            };
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