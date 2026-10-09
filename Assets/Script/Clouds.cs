namespace Project
{
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class Clouds : MonoBehaviour
    {
        [SerializeField]
        private Transform _start; 
        
        [SerializeField]
        private Transform _end;
        
        private bool _isInitialized = false;

        public void Init()
        {
            foreach (var cloud in GetComponentsInChildren<Cloud>(true))
            {
                cloud.Init(_start, _end);
            }

            _isInitialized = true;
        }

#if UNITY_EDITOR
        private void Start()
        {
            if (!_isInitialized)
            {
                Debug.LogError($"{GetType().Name} not initialized", this);
            }
        }
#endif
    }
}