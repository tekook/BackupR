using System.Reflection;

namespace Tekook.BackupR.Lib.StateManagement
{
    internal class State
    {
        public string AppVersion { get; set; }

        public BackupState BackupState { get; set; } = new();
        public CleanupState CleanupState { get; set; } = new();

        public State()
        {
        }

        public void SetAppVersion()
        {
            this.AppVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
    }
}