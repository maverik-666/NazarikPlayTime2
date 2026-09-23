using UnityEngine;
using System.Collections.Generic;

namespace LudicWorlds
{
	public class GameObjectStateMachine : MonoBehaviour, IStateMachine
	{
		protected GameObjectState currentState;
		protected GameObjectState prevState;
		protected GameObjectState nextState;
		protected Dictionary<string, GameObjectState> states;

		public IState CurrentState
		{
			get{ return currentState; }
		}

        public IState PreviousState
        {
            get{ return prevState; }
        }

		protected virtual void Awake()
		{
			InitStateMachine();
		}

		protected virtual void Start()
		{
			InitStates();
		}

	 	protected virtual void Update()
		{
			currentState?.UpdateState();
		}

        protected virtual void FixedUpdate() {}

        protected virtual void LateUpdate()
        {
			currentState?.LateUpdate();
        }

        private void InitStateMachine()
		{
            currentState = null;
			states = new Dictionary<string, GameObjectState>();
		}

		protected virtual void InitStates() {}

		public void AddState(IState state)
		{
			this.states.Add(state.ID, state as GameObjectState);
        }

		public bool SetState(IState state)
		{
            if (state != null)
			{
                currentState?.Exit();

				prevState = currentState;
				currentState = state as GameObjectState;
				currentState.Enter();
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

            if(!states.ContainsKey(stateID))
            {
                Debug.LogError("LL-> GameObjectStateMachine::ChangeState() - Can't find stateID: " + stateID);
                return false;
            }

            return SetState(states[stateID]);
		}

        public void ClearStates()
        {
            foreach (KeyValuePair<string, GameObjectState> state in states)
            {
                state.Value.Dispose();
            }

            currentState = null;
            prevState = null;
            nextState = null;

            states.Clear();
            states = null;
        }

        protected virtual void OnDestroy()
        {
            ClearStates();
        }
    }
}
