using Newtonsoft.Json;
using NLog;
using System;
using System.IO;

namespace Tekook.BackupR.Lib.StateManagement
{
    internal class StateManager
    {
        /// <summary>
        /// Singleton instance of the <see cref="StateManager"/>.
        /// </summary>
        public static StateManager Instance { get; private set; }

        /// <summary>
        /// Current State of <see cref="Instance"/>.
        /// </summary>
        public static State State => Instance?.CurrentState;

        /// <summary>
        /// Current BackupState.
        /// </summary>
        public static BackupState BackupState => State?.BackupState;

        /// <summary>
        /// Current CleanupState.
        /// </summary>
        public static CleanupState CleanupState => State?.CleanupState;

        protected ILogger Logger { get; set; } = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The file to read or write the state to.
        /// </summary>
        protected FileInfo StateFile { get; }

        /// <summary>
        /// The current runtime state.
        /// </summary>
        protected State CurrentState { get; private set; }

        /// <summary>
        /// Initilized the <see cref="StateManager.Instance"/> if it is not yet initilized.
        /// </summary>
        /// <param name="stateFile"></param>
        public static void Init(FileInfo stateFile)
        {
            Instance ??= new StateManager(stateFile);
        }

        /// <summary>
        /// Initilized the <see cref="StateManager.Instance"/> if it is not yet initilized.
        /// </summary>
        /// <param name="stateFile"></param>
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

        /// <summary>
        /// Stops the given state and saves it as <see cref="BackupState"/>.
        /// </summary>
        /// <param name="state"></param>
        public static void Stop(BackupState state)
        {
            state.Stop();
            State.BackupState = state;
        }

        /// <summary>
        /// Stops the given state and saves it as <see cref="CleanupState"/>.
        /// </summary>
        /// <param name="state"></param>
        public static void Stop(CleanupState state)
        {
            state.Stop();
            State.CleanupState = state;
        }

        /// <summary>
        /// Creates a new instance of the StateManager.
        /// </summary>
        /// <param name="stateFile"></param>
        private StateManager(FileInfo stateFile)
        {
            StateFile = stateFile;
            this.LoadState();
        }

        /// <summary>
        /// Load state from file if it exists and is not null.
        /// </summary>
        protected void LoadState()
        {
            if (this.StateFile?.Exists == true)
            {
                try
                {
                    string json = File.ReadAllText(this.StateFile.FullName);
                    this.CurrentState = JsonConvert.DeserializeObject<State>(json);
                }
                catch (Exception ex)
                {
                    Logger.Trace("Could not load state from {state_file}: {state_error}", this.StateFile, ex.Message);
                    Logger.Trace(ex);
                }
            }
            this.CurrentState ??= new State();
            this.CurrentState.SetAppVersion();
        }

        /// <summary>
        /// Saves the State to file.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the <see cref="StateManager"/> is not yet initilized.</exception>
        public static void Save()
        {
            if (Instance == null)
            {
                throw new InvalidOperationException("StateManager has not been initialized");
            }
            Instance.SaveStateToFile();
        }

        /// <summary>
        /// Save the state to json if <see cref="StateFile"/> is set.
        /// Catches all exceptions silently and logs them to <see cref="Logger"/> to not interfere with backup.
        /// </summary>
        protected void SaveStateToFile()
        {
            if (this.StateFile == null)
            {
                return;
            }
            try
            {
                Logger.Trace("Converting state to json");
                var json = JsonConvert.SerializeObject(this.CurrentState, Formatting.Indented);
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