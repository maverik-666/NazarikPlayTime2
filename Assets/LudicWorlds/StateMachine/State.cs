using System;

namespace LudicWorlds
{
	public abstract class State : IState
	{
		private readonly string _id;
		
		protected IStateMachine StateMachine;

		public string ID => _id;

		protected State(IStateMachine stateMachine, string id)
		{
			_id = id;
			StateMachine = stateMachine;
		}

        public abstract void Enter();
        
        public abstract void UpdateState();
        
        public abstract void Exit();
	}
}
