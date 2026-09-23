namespace LudicWorlds
{
	public abstract class GameObjectState : State
	{
		public GameObjectState(IStateMachine stateMachine, string id) : base(stateMachine, id)
		{
		}

        public virtual void LateUpdate() {}
        public virtual void OnGUI() {}
		public virtual void OnPostRender() {}
    }
}
