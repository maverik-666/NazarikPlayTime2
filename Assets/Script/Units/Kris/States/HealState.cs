namespace Project
{
    using LudicWorlds;
    using UnityEngine;

    public class HealState : MonoBehaviour, IState
    {
        [Header("Debug")]
        [SerializeField] 
        private string _id;

        private IDamageable _damageable;
        private IStateMachine _stateMachine;

        private float _duration;
        private string _afterHealStateId;
        private int _healAmount;
        
        private Timer _healTimer;
        
        public string ID => _id;

        public void Init(string id, IDamageable damageable, string afterHealStateId, int healAmount, float duration, IStateMachine stateMachine)
        {
            _id = id;
            _damageable = damageable;

            _afterHealStateId = afterHealStateId;
            _duration = duration;
            _healAmount = healAmount;
            
            _healTimer = new Timer();
            
            _stateMachine = stateMachine;
        }

        public void Enter()
        {
            _damageable.SetInvincible(true);
            _healTimer.Start(_duration);
            _damageable.Health.Heal(_healAmount);
        }

        public void UpdateState()
        {
            _healTimer.Tick(Time.deltaTime);

            if (_healTimer.IsStopped)
            {
                _stateMachine.SetState(_afterHealStateId);
            }
        }

        public void Exit()
        {
            _damageable.SetInvincible(false);
            _healTimer.Reset();
        }

        public void Dispose() { }
    }
}