namespace Project
{
    using System;

    [Serializable]
    public class Timer
    { 
        private float _current = 0;
        
        public bool IsRunning => _current > 0;
        public bool IsStopped => _current <= 0;
        
        public float Current => _current;
        
        public void Start(float duration)
        {
            _current = duration;
        }

        public void Tick(float deltaTime)
        {
            _current -= deltaTime;
            if (_current <= 0)
            {
                _current = 0;
            }
        }

        public void Reset()
        {
            _current = 0;
        }
    }
}