namespace LudicWorlds
{
    using UnityEngine;

    public class StateMachineDebug : MonoBehaviour
    {
#if UNITY_EDITOR
        private StateMachine _stateMachine;
        private string _currentStateID = "none";
        private string _previousStateID = "none";

        private void Awake()
        {
            _stateMachine = GetComponent<StateMachine>();
        }

        private void Update()
        {
            _currentStateID = _stateMachine.CurrentState?.ID ?? "none";
            _previousStateID = _stateMachine.PreviousState?.ID ?? "none";
        }


        private void OnDrawGizmos()
        {
            UnityEditor.Handles.Label(
                transform.position + Vector3.up * 1.5f,
                $"cur: {_currentStateID}\nprev: {_previousStateID}"
            );
        }
#endif
    }
}
