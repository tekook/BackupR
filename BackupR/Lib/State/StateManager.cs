using Config.Net;

namespace Tekook.BackupR.Lib.State
{
    internal class StateManager
    {
        private IState state;

        public IState State
        {
            get
            {
                if (state == null)
                {
                    state = Builder.Build();
                }
                return state;
            }
        }

        protected ConfigurationBuilder<IState> Builder { get; set; } = new ConfigurationBuilder<IState>();

        public StateManager(string file)
        {
            if (file != null)
            {
                this.Builder.UseJsonFile(file);
            } else
            {
                this.Builder.UseInMemoryDictionary();
            }
        }

        public static IState GetState(string file)
        {
            return (new StateManager(file)).State;
        }
    }
}