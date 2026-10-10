namespace Project
{
    using LudicWorlds;
    using UnityEngine;

    public class WaitForHealState : MonoBehaviour, IState
    {
        [Header("Debug")]
        [SerializeField] 
        private string _id;

        private IDamageable _damageable;
        private IStateMachine _stateMachine;

        private float _healCooldown;
        private string _healStateId;

        private Timer _timer;
        private bool _isOnCooldown;

        public string ID => _id;

        public void Init(string id, IDamageable damageable, float healCooldown, string healStateId, IStateMachine stateMachine)
        {
            _id = id;
            _damageable = damageable;
            
            _healStateId = healStateId;
            _healCooldown = healCooldown;
            
            _timer = new Timer();
            _stateMachine = stateMachine;
        }

        public void Enter()
        {
            if (_timer.IsStopped)
            {
                _timer.Start(_healCooldown);
            }
        }

        public void UpdateState()
        {
            _timer.Tick(Time.deltaTime);
            
            if (_timer.IsStopped)
            {
                TryHeal();
            }
        }

        private void TryHeal()
        {
            if (_damageable.Health.Current != _damageable.Health.Max)
            {
                _stateMachine.SetState(_healStateId);
            }
            else
            {
                _timer.Start(_healCooldown);
            }
        }

        public void Exit()
        {
        }

        public void Dispose() { }
    }
}