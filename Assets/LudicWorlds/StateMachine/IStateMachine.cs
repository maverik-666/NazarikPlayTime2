namespace LudicWorlds
{
	public interface IStateMachine
	{
		IState CurrentState { get; }
		IState PreviousState { get; }

		void AddState(IState state);
		bool SetState(IState state);
		bool SetState(string stateID);
		void ClearStates();
	}
}
