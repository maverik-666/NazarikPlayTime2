namespace Project
{
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class TimeSpeedHandler : MonoBehaviour
    {
        private bool _isInitialized = false;

        public void Init()
        {
            _isInitialized = true;
        }
        
        public void StopTime()
        {
            Time.timeScale = 0;
        }
        
        public void RenewTime()
        {
            Time.timeScale = 1;
        }


#if UNITY_EDITOR
        private void Start()
        {
            if (!_isInitialized)
            {
                Debug.LogError($"{nameof(TimeSpeedHandler)} not initialized", this);
            }
        }
#endif
    }
}