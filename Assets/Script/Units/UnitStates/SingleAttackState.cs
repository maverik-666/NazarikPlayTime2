namespace Project
{
    using System;
    using DG.Tweening;
    using LudicWorlds;
    using UnityEngine;

    public class SingleAttackState : MonoBehaviour, IState, IAttackState
    { 
        [Header("Debug")]
        [SerializeField] 
        private string _id;
        
        private IAttacker _attacker;
        private IStateMachine _stateMachine;
        private DamageableRegistry _damageableRegistry;

        private Timer _afterAttackTimer;
        private float _attackDuration;
        private float _attackDistance;
        private string _afterAttackStateId;
        
        private bool _isOnCooldown;

        public string ID => _id;
        public bool CanAttack => !_isOnCooldown;

        public void Init(string id, IAttacker attacker, float attackDuration, float attackDistance, string afterAttackStateId, DamageableRegistry damageableRegistry, IStateMachine stateMachine)
        {
            _id = id;
            _attacker = attacker;
            _attackDuration = attackDuration;
            _attackDistance = attackDistance;
            _afterAttackStateId = afterAttackStateId;
            
            _damageableRegistry = damageableRegistry;
            _stateMachine = stateMachine;

            _afterAttackTimer = new Timer();
        }

        public void Enter()
        {
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
            if (TryFindTarget(out var target))
            {
                target.TakeDamage(_attacker.Attack);
            }
            
            _stateMachine.SetState(_afterAttackStateId);
        }
        
        private bool TryFindTarget(out IDamageable target)
        {
            target = null;
            bool foundAny = false;
            float closestDist = float.MaxValue;
            
            foreach (var damageable in _damageableRegistry.Items)
            {
                if(IsAttackable(damageable, out float distance))
                    if (closestDist > distance)
                    {
                        target = damageable;
                        foundAny = true;
                        closestDist = distance;
                    }
            }
            
            return foundAny;
        }

        private bool IsAttackable(IDamageable damageable, out float distance)
        {
            distance = Mathf.Abs(damageable.Position.x - _attacker.Position.x);
            
            return !damageable.IsDead 
                   && !damageable.CanNotTakeDamage
                   && damageable.Team == _attacker.EnemyTeam 
                   && distance <= _attackDistance;
        }

        public void Exit()
        {
            _afterAttackTimer.Reset();
        }

        public void Dispose() { }
    }
}
