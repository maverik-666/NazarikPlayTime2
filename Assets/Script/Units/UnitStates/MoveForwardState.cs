namespace Project
{
    using LudicWorlds;
    using UnityEngine;
    using UnityEngine.UIElements;

    public class MoveForwardState : MonoBehaviour, IState
    { 
        [Header("Debug")]
        [SerializeField] 
        private string _id;
        
        private IMover _mover;
        private IStateMachine _stateMachine;
        
        private DamageableRegistry _enemies;
        
        private IAttackState _attackState;
        private string _idleStateId;

        public string ID => _id;

        public void Init(string id, IMover mover, DamageableRegistry enemies, IAttackState attackState, string idleStateId, IStateMachine stateMachine)
        {
            _id = id;
            _mover = mover;
            _enemies = enemies;
            _attackState = attackState;
            _idleStateId = idleStateId;
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
                FoundEnemy();
            else
                Move();
        }

        private bool IsInVision(IDamageable enemy)
        {
            return !enemy.IsDead 
                   && enemy.Team == _mover.EnemyTeam
                   && Mathf.Abs(enemy.Position.x - _mover.Position.x) <= _mover.VisionDistance;
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
