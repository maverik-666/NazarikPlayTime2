namespace Project
{
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
        
        private Health _health;

        private StructureSkinRenderer _structureSkinRenderer;
        private bool _canNotTakeDamage = false;
        private bool _isInitialized = false;

        public Vector2 Position => transform.position;
        public Health Health => _health;
        public Team Team => _team;

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
        
        public void TakeDamage(int damage)
        {
            _health.TakeDamage(damage);

            if (_health.IsDead)
            {
                Die();
            }
            
            _structureSkinRenderer.TakeDamage();
        }

        private void Die()
        {
            _canNotTakeDamage = true;
            
            _structureSkinRenderer.Die();
        }
     
#if UNITY_EDITOR
        private void Start()
        {
            if (!_isInitialized)
            {
                Debug.LogError($"{nameof(Structure)} not initialized", this);
            }
        }
#endif
      
    }
}