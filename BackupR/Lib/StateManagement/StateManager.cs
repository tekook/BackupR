using Newtonsoft.Json;
using NLog;
using System;
using System.IO;

namespace Tekook.BackupR.Lib.StateManagement
{
    internal class StateManager
    {
        public static StateManager Instance { get; private set; }
        public static State CurrentState => Instance?.State;
        public static BackupState BackupState => CurrentState?.BackupState;
        public static CleanupState CleanupState => CurrentState?.CleanupState;
        protected ILogger Logger { get; set; } = LogManager.GetCurrentClassLogger();

        protected FileInfo StateFile { get; }
        protected State State { get; private set; }

        public static void Init(FileInfo stateFile)
        {
            Instance ??= new StateManager(stateFile);
        }

        public static void Init(string stateFile)
        {
            if (stateFile != null)
            {
                Instance ??= new StateManager(new FileInfo(stateFile));
            }
            else
            {
                Instance ??= new StateManager(null);
            }
        }

        private StateManager(FileInfo stateFile)
        {
            StateFile = stateFile;
            this.LoadState();
        }

        protected void LoadState()
        {
            if (this.StateFile?.Exists == true)
            {
                try
                {
                    string json = File.ReadAllText(this.StateFile.FullName);
                    this.State = JsonConvert.DeserializeObject<State>(json);
                }
                catch (Exception ex)
                {
                    Logger.Trace("Could not load state from {state_file}: {state_error}", this.StateFile, ex.Message);
                    Logger.Trace(ex);
                }
            }
            this.State ??= new State();
            this.State.SetAppVersion();
        }

        public static void Save()
        {
            if (Instance == null)
            {
                throw new InvalidOperationException("StateManager has not been initialized");
            }
            Instance.SaveStateToFile();
        }

        protected void SaveStateToFile()
        {
            if (this.StateFile == null)
            {
                return;
            }
            try
            {
                Logger.Trace("Converting state to json");
                var json = JsonConvert.SerializeObject(this.State);
                Logger.Trace("Writing state to {state_file}", this.StateFile);
                File.WriteAllText(this.StateFile.FullName, json);
            }
            catch (Exception e)
            {
                Logger.Error(e, "Could not save state to file: {state_error}", e.Message);
            }
        }
    }
}