namespace Project
{
    using System;
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class OlmekSpawnService : MonoBehaviour, IService
    {
        [SerializeField]
        private int _price = 75;

        [SerializeField]
        private float _cooldown = 10;

        [SerializeField]
        private Unlockable _unlockable = Unlockable.None;
        
        [SerializeField]
        private ServiceButton _serviceButtonPf;
        
        [SerializeField]
        private Sprite _aimSprite;
        
        private Structure _structure;
        private MoneyCounter _moneyCounter;
        private Timer _cooldownTimer;
        
        private OlmekFactory _unitFactory;
        private UnitSelector _unitSelector;

        private DamageableRegistry _damageableRegistry;
        
        private bool _isAvailable = false;
        private bool _isInitialized = false;

        public event Action StateChanged;
        
        public int Price => _price;

        public float Cooldown => _cooldown;

        public float CurrentCooldown => _cooldownTimer.Current;
        
        public bool IsAvailable => _isAvailable;
        public Unlockable Unlockable => _unlockable;

        public ServiceButton ServiceButtonPf => _serviceButtonPf;

        public void Init(Structure structure, MoneyCounter moneyCounter, UnitSelector unitSelector, CameraMover cameraMover, DamageableRegistry damageableRegistry)
        {
            _structure = structure;
            _moneyCounter = moneyCounter;
            _unitSelector = unitSelector;
            _cooldownTimer = new Timer();
            
            _unitFactory = GetComponentInChildren<OlmekFactory>();
            _unitFactory.Init(cameraMover, damageableRegistry);
            
            _moneyCounter = moneyCounter;
            _damageableRegistry = damageableRegistry;
  
            _cooldownTimer.Start(_cooldown);
            
            TryUpdateState();
            
            _isInitialized = true;
        }

        public void Execute()
        {
            _cooldownTimer.Start(_cooldown);
            _moneyCounter.Subtract(_price);
            
            TryUpdateState();
            
            RequestPosition();
        }

        private void RequestPosition()
        {
            _unitSelector.RequestSelection(_structure, _structure.Team, DamageableType.Unit | DamageableType.Structure, _aimSprite, SpawnUnitOn);
        }
        
        private void SpawnUnitOn(IDamageable damageable)
        {
            var pos = damageable.Position;
            _unitFactory.Create(pos, _structure.Team);
        }

        private void Update()
        {
            _cooldownTimer.Tick(Time.deltaTime);
            
            TryUpdateState();
        } 

        private void TryUpdateState()
        {
            bool isAvailable = _moneyCounter.CheckAffordable(_price) && _cooldownTimer.IsStopped && HasAnyTeammate();
            
            if (_isAvailable != isAvailable)
            {
                _isAvailable = isAvailable;
                StateChanged?.Invoke();
            }
        }

        private bool HasAnyTeammate()
        {
            foreach (var damageable in _damageableRegistry.Items)
            {
                if (damageable.Team == _structure.Team && _structure.gameObject != damageable.transform.gameObject)
                    return true;
            }

            return false;
        }
        
#if UNITY_EDITOR
        private void Start()
        {
            if (!_isInitialized)
            {
                Debug.LogError($"{nameof(UnitSpawnService)} not initialized", this);
            }
        }
#endif
    }
}