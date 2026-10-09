namespace Project
{
    using TMPro;
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class MoneyCounterView : MonoBehaviour
    {
        [SerializeField]
        private float _changeSpeed = 0.3f;
        
        [SerializeField]
        private TMP_Text _text;
        
        [SerializeField]
        private RectTransform _icon;
        
        private MoneyCounter _moneyCounter;
        private float _currentValue = 0;
        
        private bool _isInitialized = false;

        public Transform IconCenter => _icon; 
        
        public void Init(MoneyCounter moneyCounter)
        {
            _moneyCounter = moneyCounter;
            _currentValue = _moneyCounter.CurrentValue;
            UpdateText();
            
            _isInitialized = true;
        }

        private void Update()
        {
            _currentValue = Mathf.MoveTowards(_currentValue, _moneyCounter.CurrentValue, Time.deltaTime * _changeSpeed);
            UpdateText();
        }

        private void UpdateText()
        {
            _text.text = $"{Mathf.CeilToInt(_currentValue)}";
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