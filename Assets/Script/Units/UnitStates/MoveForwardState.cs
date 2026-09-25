namespace Project
{
    using LudicWorlds;
    using UnityEngine;

    public class MoveForwardState : MonoBehaviour, IState
    { 
        [Header("Debug")]
        [SerializeField] 
        private string _id;
        
        private IMover _mover;
        private IStateMachine _stateMachine;
        
        private Registry<IDamageable> _enemies;
        private Registry<IDamageable> _attackTargets;
        
        private IAttackState _attackState;
        private string _idleStateId;

        public string ID => _id;

        public void Init(string id, IMover mover, Registry<IDamageable> enemies, Registry<IDamageable> attackTargets, IAttackState attackState, string idleStateId, IStateMachine stateMachine)
        {
            _id = id;
            _mover = mover;
            _enemies = enemies;
            _attackTargets = attackTargets;
            _attackState = attackState;
            _idleStateId = idleStateId;
            _stateMachine = stateMachine;
        }

        public void Enter() { }

        public void UpdateState()
        {
            _attackTargets.Clear();
            bool foundAny = false;

            foreach (var enemy in _enemies.Items)
            {
                if (_mover.IsEnemyInVision(enemy))
                {
                    _attackTargets.Add(enemy);
                    foundAny = true;
                }
            }

            if (foundAny)
                FoundEnemy();
            else
                Move();
        }

        private void FoundEnemy()
        {
            if (_attackState.CanAttack)
                _stateMachine.SetState(_attackState.ID);
            else
                _stateMachine.SetState(_idleStateId);
        }

        private void Move()
        {
            _mover.transform.position += Vector3.right * (_mover.Direction * _mover.MoveSpeed * Time.deltaTime);
        }

        public void Exit() { }

        public void Dispose() { }
    }
}
