namespace LudicWorlds
{
	public interface IStateMachine
	{
		IState CurrentState { get; }
		IState PreviousState { get; }

		void AddState(IState state);
		void SetState(IState state);
		void SetState(string stateID);
		void ClearStates();
	}
}
