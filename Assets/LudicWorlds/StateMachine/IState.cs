using System;

namespace LudicWorlds
{
	public interface IState
	{
		string ID { get; }

		void Enter();
		void UpdateState();
		void Exit();
	}
}
