namespace Project
{
    using System;
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class MoneyCounter : MonoBehaviour
    {
        [SerializeField]
        private int _initialValue = 0;

        private int _currentValue = 0;
        private bool _isInitialized = false;

        public Action<int> ValueChanged;
        
        public int CurrentValue => _currentValue;
        
        public void Init()
        {
            _currentValue = _initialValue;
            _isInitialized = true;
        }

        public void Add(int amount)
        {
            _currentValue += amount;
            ValueChanged?.Invoke(amount);
        }

        public void Subtract(int amount)
        {
            _currentValue -= amount;
            ValueChanged?.Invoke(amount);
        }

        public bool CheckAffordable(int amount)
        {
            return (_currentValue - amount) >= 0;
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