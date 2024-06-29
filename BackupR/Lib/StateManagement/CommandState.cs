using System;
using System.Collections.Generic;
using System.Linq;

namespace Tekook.BackupR.Lib.StateManagement
{
    internal abstract class CommandState<T> where T : StateTask, new()
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

        /// <summary>
        /// Adds a tasks to the collection.
        /// </summary>
        /// <param name="task">The Task to add.</param>
        /// <returns>The added task.</returns>
        public T AddTask(T task)
        {
            this.Tasks.Add(task);
            return task;
        }

        /// <summary>
        /// Creates a new Task with the given parameters and adds it to the collection.
        /// </summary>
        /// <param name="name"><see cref="StateTask.Name"/></param>
        /// <param name="success"><see cref="StateTask.Success"/></param>
        /// <param name="size"><see cref="StateTask.Size"/></param>
        /// <returns>The added task.</returns>
        public T AddTask(string name, bool success = false, double size = -1)
        {
            T task = new()
            {
                Name = name,
                Success = success,
                Size = size
            };
            this.Tasks.Add(task);
            return task;
        }

        /// <summary>
        /// Starts the state and updates <see cref="StartTime"/>.
        /// </summary>
        public virtual void Start()
        {
            this.StartTime = DateTime.Now;
        }

        /// <summary>
        /// Stops the state updating <see cref="EndTime"/> and calculates counters.
        /// <see cref="Success"/> will be updated if <see cref="HasFailedTasks"/> is > 0.
        /// </summary>
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