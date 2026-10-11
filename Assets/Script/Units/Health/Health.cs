namespace Project
{
    using System;
    using UnityEngine;

    [Serializable]
    public class Health
    {
        [SerializeField]
        private int _current = 0;

        [SerializeField]
        private int _max = 0;
        
        public int Current => _current;
        public int Max => _max;
        
        public bool IsDead => Current <= 0;

        public Health(int initial)
        {
            SetHealth(initial);
        }
        
        public void SetHealth(int max)
        {
            _current = _max = max;
        }
        
        public void SetCurrentHealth(int current)
        {
            _current = Mathf.Min(current, _max);
        }

        public void TakeDamage(int damage)
        {
            _current -= damage;
            
            if (_current < 0)
            { 
                _current = 0;
            }
        }

        public void Heal(int heal)
        {
            _current += heal;
            
            if (_current > _max)
            { 
                _current = _max;
            }
        }
    }
}