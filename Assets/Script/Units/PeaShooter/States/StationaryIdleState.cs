namespace Project
{
    using LudicWorlds;
    using UnityEngine;

    public class StationaryIdleState : MonoBehaviour, IState
    {
        [Header("Debug")]
        [SerializeField] 
        private string _id;

        private IIdler _idler;
        private IStateMachine _stateMachine;
        
        private DamageableRegistry _enemies;
        
        private IAttackState _attackState;
        private DamageableProperty _allowedProperties;

        public string ID => _id;

        public void Init(string id, IIdler idler, DamageableRegistry enemies, IAttackState attackState, DamageableProperty allowedProperties, IStateMachine stateMachine)
        {
            _id = id;
            _idler = idler;
            _enemies = enemies;
            _attackState = attackState;
            _allowedProperties = allowedProperties;
            _stateMachine = stateMachine;
        }

        public void Enter() { }

        public void UpdateState()
        {
            bool foundAny = false;

            foreach (var enemy in _enemies.Items)
            {
                if (IsInVision(enemy))
                {
                    foundAny = true;
                }
            }

            if (foundAny)
                FoundAny();
        }
        
        private bool IsInVision(IDamageable enemy)
        {
            return !enemy.IsDead 
                   && enemy.Team == _idler.EnemyTeam
                   && Mathf.Abs(enemy.Position.x - _idler.Position.x) <= _idler.VisionDistance            
                   && _allowedProperties.HasFlag(enemy.Properties);
        }


        private void FoundAny()
        {
            if (_attackState.CanAttack)
                _stateMachine.SetState(_attackState.ID);
        }

        public void Exit() { }

        public void Dispose() { }
    }
}