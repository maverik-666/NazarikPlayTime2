namespace Project
{
    using LudicWorlds;
    using UnityEngine;

    public class AnimatronicDieState : MonoBehaviour, IState
    {
        [Header("Debug")]
        [SerializeField]
        private string _id;

        private Animatronic _unit;
        private IStateMachine _stateMachine;

        private DamageableRegistry _damageableRegistry;
        private GhostFactory _ghostFactory;
        private UnitRegistry _unitRegistry;

        public string ID => _id;

        public void Init(string id, Animatronic unit, DamageableRegistry damageableRegistry, GhostFactory ghostFactory, IStateMachine stateMachine)
        {
            _id = id;
            _unit = unit;
            _ghostFactory = ghostFactory;
            _damageableRegistry = damageableRegistry;
            _unitRegistry = this.Resolve<UnitRegistry>();
            _stateMachine = stateMachine;
        }

        public void Enter()
        {
            _unit.SetInvincible(true);
            _damageableRegistry.Remove(_unit);
            _unitRegistry.Remove(_unit);

            if (!_unit.IsDepleted)
            { 
                _ghostFactory.Create(_unit.Position, _unit.Team, _unit.EnemyTeam, _unit.Direction);
                _unit.SetDepleted();
            }
        }

        public void UpdateState()
        {
        }

        public void Exit()
        {
        }

        public void Dispose()
        {
        }
    }
}