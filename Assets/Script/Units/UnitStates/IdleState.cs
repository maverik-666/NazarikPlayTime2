namespace Project
{
    using LudicWorlds;
    using UnityEngine;

    public class IdleState : MonoBehaviour, IState
    {
        [Header("Debug")]
        [SerializeField] 
        private string _id;

        private IIdler _idler;
        private IStateMachine _stateMachine;
        
        private DamageableRegistry _enemies;
        
        private IAttackState _attackState;
        private string _moveStateId;

        public string ID => _id;

        public void Init(string id, IIdler idler, DamageableRegistry enemies, IAttackState attackState, string moveStateId, IStateMachine stateMachine)
        {
            _id = id;
            _idler = idler;
            _enemies = enemies;
            _attackState = attackState;
            _moveStateId = moveStateId;
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
            else
                _stateMachine.SetState(_moveStateId);
        }
        
        private bool IsInVision(IDamageable enemy)
        {
            return !enemy.IsDead 
                   && enemy.Team == _idler.EnemyTeam
                   && Mathf.Abs(enemy.Position.x - _idler.Position.x) <= _idler.VisionDistance;
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
