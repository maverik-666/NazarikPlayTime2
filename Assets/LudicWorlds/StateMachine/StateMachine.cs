namespace LudicWorlds 
{ 
	using UnityEngine;
	using System;
	using System.Collections.Generic;
	
	public class StateMachine : MonoBehaviour, IStateMachine
	{
		private IState _currentState = null;
		
		private IState _prevState;
		private readonly Dictionary<string, IState> _states = new Dictionary<string, IState>();

		public IState CurrentState => _currentState;

		public IState PreviousState => _prevState;

		public Action<IState> StateEntered;

		private void Update()
        {
            _currentState?.UpdateState();
        }

        public void AddState(IState state)
		{
			_states.Add(state.ID, state);
		}

        public void SetState(IState state)
        {
            _currentState?.Exit();

            _prevState = _currentState;
            _currentState = state;
            _currentState.Enter();
            StateEntered?.Invoke(_currentState);
        }

        public void SetState(string stateID)
        { 
	        SetState(_states[stateID]);
        }

        public void ClearStates()
        {
            _states.Clear();

            _currentState = null;
            _prevState = null;
        }
    }
}
