namespace Project
{
    using System;
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class UnitSpawnService : MonoBehaviour, IService
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
        private Vector2 _yOffsetRange = new Vector2(-0.25f, 0.25f);
        
        private Structure _structure;
        private MoneyCounter _moneyCounter;
        private Timer _cooldownTimer;
        
        private IUnitFactory _unitFactory;

        private bool _isAvailable = false;
        private bool _isInitialized = false;

        public event Action StateChanged;
        
        public int Price => _price;

        public float Cooldown => _cooldown;

        public float CurrentCooldown => _cooldownTimer.Current;
        
        public bool IsAvailable => _isAvailable;
        public Unlockable Unlockable => _unlockable;

        public ServiceButton ServiceButtonPf => _serviceButtonPf;

        public void Init(Structure structure, MoneyCounter moneyCounter, DamageableRegistry damageableRegistry)
        {
            _structure = structure;
            _moneyCounter = moneyCounter;
            _cooldownTimer = new Timer();
            
            _unitFactory = GetComponentInChildren<IUnitFactory>();
            _unitFactory.Init(damageableRegistry);

            _cooldownTimer.Start(_cooldown);
            
            _moneyCounter = moneyCounter;
            _moneyCounter.ValueChanged += OnMoneyCounterChange;
            
            TryUpdateState();
            
            _isInitialized = true;
        }

        public void Execute()
        {
            _cooldownTimer.Start(_cooldown);
            _moneyCounter.Subtract(_price);
            
            TryUpdateState();
            
            SpawnUnit();
        }

        private void SpawnUnit()
        {
            var pos = _structure.Position + Vector2.up * UnityEngine.Random.Range(_yOffsetRange.x, _yOffsetRange.y);
            _unitFactory.Create(pos, _structure.Team, _structure.EnemyTeam, _structure.Direction);
        }

        private void Update()
        {
            _cooldownTimer.Tick(Time.deltaTime);

            if (_cooldownTimer.IsStopped)
            {
                TryUpdateState();
            }
        } 
        
        private void OnMoneyCounterChange(int _)
        {
            TryUpdateState();
        }

        private void TryUpdateState()
        {
            bool isAvailable = _moneyCounter.CheckAffordable(_price) && _cooldownTimer.IsStopped;
            
            if (_isAvailable != isAvailable)
            {
                _isAvailable = isAvailable;
                StateChanged?.Invoke();
            }
        }

        private void OnDestroy()
        {
            _moneyCounter.ValueChanged -= OnMoneyCounterChange;
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