namespace Tekook.BackupR.Lib.StateManagement
{
    internal class BackupState : CommandState
    {
        public bool Successfull { get; set; }
        public bool HasFailedTask { get; set; }
        public bool HasProviderError { get; set; }
        public int TotalTasks { get; set; } = -1;
        public int SuccessfullTasks { get; set; } = -1;
        public int FailedTasks { get; set; } = -1;

        protected override void StateStarted()
        {
            this.TotalTasks = -1;
            this.SuccessfullTasks = -1;
            this.FailedTasks = -1;
        }

        protected override void StateStoped()
        {
            this.HasFailedTask = this.FailedTasks > 0;
            this.Successfull = !this.HasFailedTask && !this.HasProviderError;
        }
    }
}