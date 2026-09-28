namespace Project
{
    using LudicWorlds;
    using UnityEngine;

    public class DieState : MonoBehaviour, IState
    {
        [Header("Debug")]
        [SerializeField] 
        private string _id;

        private IUnit _unit;
        private IStateMachine _stateMachine;
        
        private DamageableRegistry _damageableRegistry;
        
        private JumpAnimator _jumpAnimator;

        public string ID => _id;

        public void Init(string id, IUnit unit, DamageableRegistry damageableRegistry, IStateMachine stateMachine)
        {
            _id = id;
            _unit = unit;
            _damageableRegistry = damageableRegistry;
            _stateMachine = stateMachine;
            _jumpAnimator = new JumpAnimator(transform);
        }

        public void Enter()
        {
            _unit.SetInvincible(true);
            _damageableRegistry.Remove(_unit);
            _jumpAnimator.Play(-_unit.Direction, _unit.Dispose);
        }

        public void UpdateState()
        {
            _jumpAnimator.Tick(Time.deltaTime);
        }

        public void Exit() { }

        public void Dispose() { }
    }
}
