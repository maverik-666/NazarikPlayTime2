namespace Project
{
    using DG.Tweening;
    using LudicWorlds;
    using UnityEngine;

    public class RotAttackState : MonoBehaviour, IState, IAttackState
    { 
        [Header("Debug")]
        [SerializeField] 
        private string _id;
        
        private IAttacker _attacker;
        private IUnit _unit;
        private IStateMachine _stateMachine;
        private UnitRegistry _unitRegistry;

        private Tentacle _tentacle;
        private float _attackDistance;
        private string _afterAttackStateId;
        private IDamageable _playerStructure;
        
        private IDamageable _target;
        private bool _isConsuming;
        private bool _isOnCooldown;

        public string ID => _id;
        public bool CanAttack => !_isOnCooldown;

        public void Init(string id, IAttacker attacker, IUnit unit, Tentacle tentacle, float attackDistance, string afterAttackStateId, UnitRegistry unitRegistry, IStateMachine stateMachine)
        {
            _id = id;
            _attacker = attacker;
            _unit = unit;

            _tentacle = tentacle;
            _attackDistance = attackDistance;
            _afterAttackStateId = afterAttackStateId;
            
            _playerStructure = this.Resolve<PlayerStructure>().Structure;

            _unitRegistry = unitRegistry;
            _stateMachine = stateMachine;
        }

        public void Enter()
        {
            if (TryFindTarget(out var target))
            { 
                target.PickedUp();
                Attack(target);
            }
            else if (TryFindPlayerStructure())
            {
                Attack(_playerStructure);
            }
            else
            {
                Pass();
            }
        }

        private void Pass()
        {
            _stateMachine.SetState(_afterAttackStateId);
        }

        private void Attack(IDamageable target)
        {
            _isConsuming = true;
            _target = target;
            _tentacle.StartMoveTo(target.transform, Grab, Consume);
            
            _isOnCooldown = true;
            DOTween.Sequence()
                .SetLink(_attacker.gameObject, LinkBehaviour.KillOnDisable)
                .AppendInterval(_attacker.AttackCooldown)
                .AppendCallback(OnCooldownPass);
        }

        private void Grab()
        {
            
        }

        private void Consume()
        {
            Debug.Log("Consume", _target.transform.gameObject);
            
            if (_target.IsDead)
            {
                return;
            }

            _unit.SetInvincible(true);
            _target.transform.DOScale(0, 0.5f)
                .SetLink(gameObject, LinkBehaviour.KillOnDisable)
                .SetEase(Ease.OutSine)
                .OnComplete(AfterConsume);
        }

        private void AfterConsume()
        {
            _target.InstantDeath();
            _unit.SetInvincible(false);
            _stateMachine.SetState("Idle");
        }
        
        private void OnCooldownPass()
        {
            _isOnCooldown = false;
        }

        public void UpdateState()
        {
        }
        
        private bool TryFindTarget(out IUnit target)
        {
            target = null;
            bool foundAny = false;
            float closestDist = float.MaxValue;

            foreach (var unit in _unitRegistry.Items)
            {
                if (IsAttackable(unit, out float distance))
                    if (closestDist > distance)
                    {
                        target = unit;
                        foundAny = true;
                        closestDist = distance;
                    }
            }

            return foundAny;
        }

        private bool TryFindPlayerStructure()
        {
            return Mathf.Abs(_playerStructure.Position.x - _attacker.Position.x) <= _attackDistance;
        }

        private bool IsAttackable(IUnit unit, out float distance)
        {
            distance = Mathf.Abs(unit.Position.x - _attacker.Position.x);

            return !unit.IsDead
                   && !unit.CanNotTakeDamage
                   && distance <= _attackDistance
                   && unit.Team == _attacker.EnemyTeam
                   && unit.Type == DamageableType.Unit
                   && (unit.Properties & DamageableProperty.Intangible) == DamageableProperty.None;
        }

        public void Exit()
        {
            if (_isConsuming)
            {
                _tentacle.Discard();
                _target.InstantDeath();
            }

            _isConsuming = false;
        }

        public void Dispose() { }
    }
}
