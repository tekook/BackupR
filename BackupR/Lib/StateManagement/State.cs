using System.Reflection;

namespace Tekook.BackupR.Lib.StateManagement
{
    internal class State
    {
        /// <summary>
        /// Version of the app which created the state.
        /// </summary>
        public string AppVersion { get; set; }

        /// <summary>
        /// Current BackupState
        /// </summary>
        public BackupState BackupState { get; set; } = new();

        /// <summary>
        /// Current CleanupState
        /// </summary>
        public CleanupState CleanupState { get; set; } = new();

        /// <summary>
        /// Sets the <see cref="AppVersion"/> to current asembly version.
        /// </summary>
        public void SetAppVersion()
        {
            this.AppVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
    }
}