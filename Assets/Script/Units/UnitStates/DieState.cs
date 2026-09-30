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
        private UnitRegistry _unitRegistry;
        
        public string ID => _id;

        public void Init(string id, IUnit unit, DamageableRegistry damageableRegistry, IStateMachine stateMachine)
        {
            _id = id;
            _unit = unit;
            _damageableRegistry = damageableRegistry;
            _unitRegistry = this.Resolve<UnitRegistry>();
            _stateMachine = stateMachine;
        }

        public void Enter()
        {
            _unit.SetInvincible(true);
            _damageableRegistry.Remove(_unit);
            _unitRegistry.Remove(_unit);
        }

        public void UpdateState()
        {
        }

        public void Exit() { }

        public void Dispose() { }
    }
}
