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
        private IStateMachine _stateMachine;
        private UnitRegistry _unitRegistry;

        private Tentacle _tentacle;
        private float _attackDistance;
        private string _afterAttackStateId;
        
        private bool _isOnCooldown;

        public string ID => _id;
        public bool CanAttack => !_isOnCooldown;

        public void Init(string id, IAttacker attacker, Tentacle tentacle, float attackDistance, string afterAttackStateId, UnitRegistry unitRegistry, IStateMachine stateMachine)
        {
            _id = id;
            _attacker = attacker;

            _tentacle = tentacle;
            _attackDistance = attackDistance;
            _afterAttackStateId = afterAttackStateId;

            _unitRegistry = unitRegistry;
            _stateMachine = stateMachine;
        }

        public void Enter()
        {
            if (TryFindTarget(out var target))
            {
                Attack(target);
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

        private void Attack(IUnit target)
        {
            target.PickedUp();
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

        private bool IsAttackable(IUnit unit, out float distance)
        {
            distance = Mathf.Abs(unit.Position.x - _attacker.Position.x);

            return !unit.IsDead
                   && !unit.CanNotTakeDamage
                   && unit.Team == _attacker.EnemyTeam
                   && distance <= _attackDistance;
        }

        public void Exit()
        {
        }

        public void Dispose() { }
    }
}
