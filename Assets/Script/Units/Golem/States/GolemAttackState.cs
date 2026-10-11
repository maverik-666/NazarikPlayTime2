namespace Project
{
    using DG.Tweening;
    using LudicWorlds;
    using UnityEngine;

    public class GolemAttackState : MonoBehaviour, IState, IAttackState
    { 
        [Header("Debug")]
        [SerializeField] 
        private string _id;
        
        private IAttacker _attacker;
        private IDamageable _damageable; 
        private DamageableRegistry _damageableRegistry;
        private IStateMachine _stateMachine;

        private Timer _afterAttackTimer;
        private float _attackDuration;
        private float _attackDistance;
        private string _afterAttackStateId;

        private CameraMover _cameraMover;
        
        private bool _isOnCooldown;

        public string ID => _id;
        public bool CanAttack => !_isOnCooldown;

        public void Init(string id, IAttacker attacker, IDamageable damageable, float attackDuration, float attackDistance, string afterAttackStateId, DamageableRegistry damageableRegistry, IStateMachine stateMachine)
        {
            _id = id;
            _attacker = attacker;
            _damageable = damageable;
            _damageableRegistry = damageableRegistry;
            _attackDuration = attackDuration;
            _attackDistance = attackDistance;
            _afterAttackStateId = afterAttackStateId;
            _stateMachine = stateMachine;
            _cameraMover = this.Resolve<CameraMover>();
            
            _afterAttackTimer = new Timer();
        }

        public void Enter()
        {
            _damageable.SetInvincible(true);
            
            _afterAttackTimer.Start(_attackDuration);
            
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
            for (int i = _damageableRegistry.Items.Count - 1; i >= 0; i--)
            {
                var damageable = _damageableRegistry.Items[i];
                if (IsAttackable(damageable))
                {
                    damageable.TakeDamage(_attacker.Attack);
                }
            }

            _cameraMover.Shake(0.3f, 1);
            _stateMachine.SetState(_afterAttackStateId);
        }
        
        private bool IsAttackable(IDamageable damageable)
        {
            float distance = damageable.Position.x - _attacker.Position.x;

            if (distance * _attacker.Direction < 0)
            {
                return false;
            }
            
            distance = Mathf.Abs(distance);
            
            return !damageable.IsDead 
                   && !damageable.CanNotTakeDamage
                   && (damageable.Team == _attacker.EnemyTeam) 
                   && distance <= _attackDistance;
        }


        public void Exit()
        {
            _afterAttackTimer.Reset();
        }

        public void Dispose() { }
    }
}
