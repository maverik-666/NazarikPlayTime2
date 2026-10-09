namespace Project
{
    using LudicWorlds;
    using UnityEngine;

    public class WaitState : MonoBehaviour, IState
    {
        [Header("Debug")]
        [SerializeField]
        private string _id;

        private IStateMachine _stateMachine;
        
        private string _afterWaitStateId;
        private Timer _timer;
        private float _prepareDuration;

        public string ID => _id;

        public void Init(string id, float prepareDuration, string afterWaitStateId, IStateMachine stateMachine)
        {
            _id = id;
            
            _prepareDuration = prepareDuration;
            _afterWaitStateId = afterWaitStateId;
            _timer = new Timer();
            _stateMachine = stateMachine;
        }

        public void Enter()
        {
            _timer.Start(_prepareDuration);
        }

        public void UpdateState()
        {
            _timer.Tick(Time.deltaTime);
            
            if (_timer.IsStopped)
            {
                _stateMachine.SetState(_afterWaitStateId);
            }
        }

        public void Exit()
        {
        }

        public void Dispose()
        {
        }
    }
}
