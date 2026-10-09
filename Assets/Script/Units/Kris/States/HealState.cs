namespace Project
{
    using LudicWorlds;
    using UnityEngine;

    public class HealState : MonoBehaviour, IState
    {
        [Header("Debug")]
        [SerializeField] 
        private string _id;

        private IIdler _idler;
        private IStateMachine _stateMachine;
        
        public string ID => _id;

        public void Init(string id, IIdler idler, IStateMachine stateMachine)
        {
            _id = id;
            _idler = idler;
            _stateMachine = stateMachine;
        }

        public void Enter() { }

        public void UpdateState()
        {
        }
        
        public void Exit() { }

        public void Dispose() { }
    }
}