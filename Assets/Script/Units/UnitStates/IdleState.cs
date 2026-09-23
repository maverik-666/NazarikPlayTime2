namespace Project
{
    using LudicWorlds;
    using UnityEngine;

    public class IdleState : MonoBehaviour, IState
    {
        [Header("Debug")]
        [SerializeField] 
        private string _id;

        private Unit _unit;
        private Registry<IDamageable> _enemies;
        private Registry<IDamageable> _attackTargets;
        private IAttackState _attackState;
        private string _moveStateId;
        private IStateMachine _stateMachine;

        public string ID => _id;

        public void Init(string id, Unit unit, Registry<IDamageable> enemies, Registry<IDamageable> attackTargets, IAttackState attackState, string moveStateId, IStateMachine stateMachine)
        {
            _id = id;
            _unit = unit;
            _enemies = enemies;
            _attackTargets = attackTargets;
            _attackState = attackState;
            _moveStateId = moveStateId;
            _stateMachine = stateMachine;
        }

        public void Enter() { }

        public void UpdateState()
        {
            _attackTargets.Clear();
            bool foundAny = false;

            foreach (var enemy in _enemies.Items)
            {
                if (_unit.IsEnemyInVision(enemy))
                {
                    _attackTargets.Add(enemy);
                    foundAny = true;
                }
            }

            if (foundAny)
                FoundAny();
            else
                _stateMachine.SetState(_moveStateId);
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
