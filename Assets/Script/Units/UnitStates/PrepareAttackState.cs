namespace Project
{
    using LudicWorlds;
    using UnityEngine;

    public class PrepareAttackState : MonoBehaviour, IState, IAttackState
    {
        [Header("Debug")]
        [SerializeField]
        private string _id;

        private IStateMachine _stateMachine;
        
        private IAttackState _attackState;
        private Timer _timer;
        private float _prepareDuration;

        public bool CanAttack => _attackState.CanAttack;
        public string ID => _id;

        public void Init(string id, float prepareDuration, IAttackState attackState, IStateMachine stateMachine)
        {
            _id = id;
            
            _prepareDuration = prepareDuration;
            _attackState = attackState;
            _timer = new Timer();
            _stateMachine = stateMachine;
        }

        public void Enter()
        {
            _timer.Start(_prepareDuration);
        }

        public void UpdateState()
        {
            _timer.Tick(Time.deltaTime);
            
            if (_timer.IsStopped)
            {
                _stateMachine.SetState(_attackState.ID);
            }
        }

        public void Exit()
        {
        }

        public void Dispose()
        {
        }
    }
}