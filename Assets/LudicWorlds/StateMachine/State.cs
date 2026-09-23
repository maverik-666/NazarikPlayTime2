using System;

namespace LudicWorlds
{
	public abstract class State : IState
	{
        protected string Id;
		protected IStateMachine StateMachine;

        protected Boolean IsDisposed = false;

		public string ID
		{
			get { return Id; }
		}

		public State(IStateMachine stateMachine, string id)
		{
			this.Id = id;
			this.StateMachine = stateMachine;
		}

        public abstract void Enter();
        public abstract void UpdateState();
        public abstract void Exit();

        public virtual void Dispose()
        {
            Id = null;
            StateMachine = null;
            IsDisposed = true;
        }
	}
}
