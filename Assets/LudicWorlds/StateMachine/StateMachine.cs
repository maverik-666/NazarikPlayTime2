//Finite State Machine
using System;
using System.Collections.Generic;

namespace LudicWorlds
{
	public class StateMachine : IStateMachine
	{
		protected IState currentState = null;
		protected IState prevState;
		protected Dictionary<string, IState> states = new Dictionary<string, IState>();

		public IState CurrentState
		{
			get{ return currentState; }
		}

		public IState PreviousState
		{
			get{ return prevState; }
		}

		public Action<IState> StateEntered;

		public void Update()
		{
            if (currentState != null)
            {
                currentState.UpdateState();
            }
		}

        public void AddState(IState state)
		{
			this.states.Add(state.ID, state);
		}

        public bool SetState(IState state)
        {
            if (state != null)
            {
                currentState?.Exit();

                prevState = currentState;
                currentState = state;
                currentState.Enter();
                StateEntered?.Invoke(currentState);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool SetState(string stateID)
        {
            if (currentState != null)
            {
                if (stateID.Equals(currentState.ID))
                    return true;
            }

            return SetState(states[stateID]);
        }

        public void ClearStates()
        {
            foreach (var state in states.Values)
            {
                state?.Dispose();
            }

            states.Clear();

            currentState = null;
            prevState = null;
        }
    }
}
