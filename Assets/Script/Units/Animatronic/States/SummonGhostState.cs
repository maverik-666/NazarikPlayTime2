namespace Project
{
    using LudicWorlds;
    using UnityEngine;
    
    public class SummonGhostState : MonoBehaviour, IState, IAttackState
    {
        [Header("Debug")]
        [SerializeField] 
        private string _id;

        private IUnit _unit;
        private IStateMachine _stateMachine;
        private GhostFactory _ghostFactory;
        
        private Timer _afterAttackTimer;
        private float _attackDuration;

        private string _afterAttackStateId;
        
        private int _damageCount; 
        
        private bool _isOnCooldown;
        
        public string ID => _id;
        public bool CanAttack => !_isOnCooldown;

        public void Init(string id, IUnit unit, float attackDuration, GhostFactory ghostFactory, string afterAttackStateId, IStateMachine stateMachine)
        {
            _id = id;
            _unit = unit;
            _ghostFactory = ghostFactory;
            _attackDuration = attackDuration;

            _afterAttackStateId = afterAttackStateId;
            
            _stateMachine = stateMachine;

            _afterAttackTimer = new Timer();
        }

        public void Enter()
        {
            _afterAttackTimer.Start(_attackDuration);
        }

        public void UpdateState()
        {
            _afterAttackTimer.Tick(Time.deltaTime);
            if (_afterAttackTimer.IsStopped)
            {
                AfterAttack();
            }
        }
        
        private void AfterAttack()
        {
            _ghostFactory.Create(_unit.transform.position, _unit.Team,  _unit.EnemyTeam, _unit.Direction);
            _stateMachine.SetState(_afterAttackStateId);
        }

        public void Exit()
        {
            _afterAttackTimer.Reset();
        }

        public void Dispose() { }
    }
}