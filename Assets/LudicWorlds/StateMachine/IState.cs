using System;

namespace LudicWorlds
{
	public interface IState : IDisposable
	{
		string ID { get; }

		void Enter();
		void UpdateState();
		void Exit();
	}
}
