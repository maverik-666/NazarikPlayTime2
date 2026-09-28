namespace Project
{
    using DG.Tweening;
    using LudicWorlds;
    using UnityEngine;

    public class ShootAttackState : MonoBehaviour, IState, IAttackState
    {
        [SerializeField]
        private Transform _shootPoint;

        [Header("Debug")]
        [SerializeField] 
        private string _id;

        private IAttacker _attacker;
        private IStateMachine _stateMachine;
        private ProjectileFactory _projectileFactory;
        
        private Timer _afterAttackTimer;
        private float _attackDuration;
        private string _afterAttackStateId;
        
        private bool _isOnCooldown;

        public string ID => _id;
        public bool CanAttack => !_isOnCooldown;

        public void Init(string id, IAttacker attacker, float attackDuration, string afterAttackStateId, ProjectileFactory projectileFactory, IStateMachine stateMachine)
        {
            _id = id;
            _attacker = attacker;
            _attackDuration = attackDuration;
            _afterAttackStateId = afterAttackStateId;
            _projectileFactory = projectileFactory;
            
            _stateMachine = stateMachine;

            _afterAttackTimer = new Timer();
            
            GoOnCooldown();
        }

        public void Enter()
        {
            _afterAttackTimer.Start(_attackDuration);
            
            GoOnCooldown();
        }

        private void GoOnCooldown()
        {
            _isOnCooldown = true;
            DOTween.Sequence()
                .SetLink(_attacker.gameObject, LinkBehaviour.KillOnDisable)
                .AppendInterval(_attacker.AttackCooldown)
                .AppendCallback(OnCooldownPass);
        } 
        
        private void OnCooldownPass()
        {
            _isOnCooldown = false;
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
            _projectileFactory.Create(_shootPoint.position, _attacker.EnemyTeam, _attacker.Direction);
            _stateMachine.SetState(_afterAttackStateId);
        }

        public void Exit()
        {
            _afterAttackTimer.Reset();
        }

        public void Dispose() { }
    }
}