namespace Project
{
    using LudicWorlds;
    using UnityEngine;

    public class TakeDamageState : MonoBehaviour, IState
    {
        [Header("Debug")]
        [SerializeField] 
        private string _id;
        
        private IUnit _unit;
        private IStateMachine _stateMachine;

        private string _dieStateId;
        private string _afterDamageStateId;
        
        private JumpAnimator _jumpAnimator;

        public string ID => _id;

        public void Init(string id, IUnit unit, string dieStateId, string afterDamageStateId, IStateMachine stateMachine)
        {
            _id = id;
            _unit = unit;
            _dieStateId = dieStateId;
            _afterDamageStateId = afterDamageStateId;
            _stateMachine = stateMachine;
            _jumpAnimator = new JumpAnimator(unit.transform);
        }

        public void Enter()
        {
            _unit.SetInvincible(true);
            _jumpAnimator.Play(-_unit.Direction, SetAfterDamageState);
        }

        private void SetAfterDamageState()
        {
            _stateMachine.SetState(_unit.Health.IsDead ? _dieStateId : _afterDamageStateId);
        }

        public void UpdateState()
        {
            _jumpAnimator.Tick(Time.deltaTime);
        }

        public void Exit()
        {
            _unit.SetInvincible(false);
        }

        public void Dispose() { }
    }
}
