namespace Project
{
    using LudicWorlds;
    using UnityEngine;

    public class DefendState : MonoBehaviour, IState
    {
        [Header("Debug")]
        [SerializeField]
        private string _id;

        private IDamageable _damageable;
        private IStateMachine _stateMachine;

        private float _duration;
        private string _afterDefendStateId;

        private Timer _healTimer;

        public string ID => _id;

        public void Init(string id, IDamageable damageable, string afterDefendStateId, float duration,
            IStateMachine stateMachine)
        {
            _id = id;
            _damageable = damageable;

            _afterDefendStateId = afterDefendStateId;
            _duration = duration;

            _healTimer = new Timer();

            _stateMachine = stateMachine;
        }

        public void Enter()
        {
            _damageable.SetInvincible(true);
            _healTimer.Start(_duration);
        }

        public void UpdateState()
        {
            _healTimer.Tick(Time.deltaTime);

            if (_healTimer.IsStopped)
            {
                _stateMachine.SetState(_afterDefendStateId);
            }
        }

        public void Exit()
        {
            _damageable.SetInvincible(false);
            _healTimer.Reset();
        }

        public void Dispose()
        {
        }
    }
}