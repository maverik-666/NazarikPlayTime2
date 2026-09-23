namespace Project
{
    using LudicWorlds;
    using UnityEngine;

    public class DieState : MonoBehaviour, IState
    {
        [Header("Debug")]
        [SerializeField] 
        private string _id;

        private Unit _unit;
        private Registry<IDamageable> _damageableRegistry;
        private IStateMachine _stateMachine;
        private JumpAnimator _jumpAnimator;

        public string ID => _id;

        public void Init(string id, Unit unit, Registry<IDamageable> damageableRegistry, IStateMachine stateMachine)
        {
            _id = id;
            _unit = unit;
            _damageableRegistry = damageableRegistry;
            _stateMachine = stateMachine;
            _jumpAnimator = new JumpAnimator(transform);
        }

        public void Enter()
        {
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
