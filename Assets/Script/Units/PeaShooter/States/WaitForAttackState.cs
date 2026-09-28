namespace Project
{
    using LudicWorlds;
    using UnityEngine;

    public class WaitForAttackState : MonoBehaviour, IState, IAttackState
    {
        [Header("Debug")]
        [SerializeField]
        private string _id;

        private IStateMachine _stateMachine;

        private IAttackState _attackState;

        public bool CanAttack => _attackState.CanAttack;
        public string ID => _id;

        public void Init(string id, IAttackState attackState, IStateMachine stateMachine)
        {
            _id = id;

            _attackState = attackState;
            _stateMachine = stateMachine;
        }

        public void Enter()
        {
        }

        public void UpdateState()
        {
            if (_attackState.CanAttack)
            {
                _stateMachine.SetState(_attackState.ID);
            }
        }

        public void Exit()
        {
        }
    }
}