namespace Project
{
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class TimeSpeedHandler : MonoBehaviour
    {
        private float _currentSpeed = 1;
        private bool _isInitialized = false;

        public void Init()
        {
            _currentSpeed = 1;
            Time.timeScale = _currentSpeed;
            _isInitialized = true;
        }
        
        public void StopTime()
        {
            Time.timeScale = 0;
        }
        
        public void RenewTime()
        {
            Time.timeScale = _currentSpeed;
        }

        public void SetSpeed(float speed)
        {
            _currentSpeed = speed;
            Time.timeScale = _currentSpeed;
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