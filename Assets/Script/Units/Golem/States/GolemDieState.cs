namespace Project
{
    using LudicWorlds;
    using UnityEngine;

    public class GolemDieState : MonoBehaviour, IState
    {
        [Header("Debug")]
        [SerializeField] 
        private string _id;

        private IUnit _unit;
        private GolemRemainsFactory _golemRemainsFactory;
        private IStateMachine _stateMachine;
        
        private DamageableRegistry _damageableRegistry;
        
        public string ID => _id;

        public void Init(string id, Golem unit, DamageableRegistry damageableRegistry, GolemRemainsFactory golemRemainsFactory, IStateMachine stateMachine)
        {
            _id = id;
            _unit = unit;
            _golemRemainsFactory = golemRemainsFactory;
            _damageableRegistry = damageableRegistry;
            _stateMachine = stateMachine;
        }

        public void Enter()
        {
            _unit.SetInvincible(true);
            _damageableRegistry.Remove(_unit);
            
            _golemRemainsFactory.Create(_unit.Position, _unit.Team, _unit.Direction);
            _unit.Dispose();
        }

        public void UpdateState()
        {
        }

        public void Exit() { }

        public void Dispose() { }
    }
}