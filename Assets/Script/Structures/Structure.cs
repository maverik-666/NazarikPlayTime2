namespace Project
{
    using System;
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class Structure : MonoBehaviour, IDamageable
    {
        [SerializeField]
        private int _heathPoints = 50;
        
        [SerializeField]
        private Team _team = Team.Red;
        
        [SerializeField]
        private Team _enemyTeam = Team.Blue;
        
        [SerializeField]
        private int _direction = 1;
        
        [SerializeField]
        private DamageableProperty _properties = DamageableProperty.Normal;
        
        private Health _health;

        private StructureSkinRenderer _structureSkinRenderer;
        private bool _canNotTakeDamage = false;
        private bool _isInitialized = false;

        public Action Death;

        public Vector2 Position => transform.position;
        
        public float Radius => 0;
        
        public Health Health => _health;
        public Team Team => _team;
        
        public DamageableType Type => DamageableType.Base;
              public DamageableProperty Properties => _properties;

        public Team EnemyTeam => _enemyTeam;
        public int Direction => _direction;

        public bool CanNotTakeDamage => _canNotTakeDamage;
        
        public void Init()
        {
            _health = new Health(_heathPoints);
            _structureSkinRenderer = GetComponentInChildren<StructureSkinRenderer>();
            
            _structureSkinRenderer.Init(_health);
            _isInitialized = true;
        }
        
        public void SetInvincible(bool value)
        {
            _canNotTakeDamage = value;
        }
        
        public void TakeDamage(int damage)
        {
            _health.TakeDamage(damage);

            _structureSkinRenderer.TakeDamage();
            
            if (_health.IsDead)
            {
                Die();
            }
        }

        private void Die()
        {
            _canNotTakeDamage = true;
            
            _structureSkinRenderer.Die();
            
            Death?.Invoke();
        }

        public void InstantDeath()
        {
            _health.SetCurrentHealth(0);
            Die();
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