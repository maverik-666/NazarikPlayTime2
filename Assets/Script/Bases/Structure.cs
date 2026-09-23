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
        
        private Health _health;

        private StructureSkinRenderer _structureSkinRenderer;
        private bool _isInitialized = false;

        public Vector2 Position => transform.position;
        public Health Health => _health;
        public Team Team => _team;
        
        public bool CanNotTakeDamage => true;
        
        public void Init()
        {
            _health = new Health(_heathPoints);
            _structureSkinRenderer = GetComponentInChildren<StructureSkinRenderer>();
            
            _structureSkinRenderer.Init(_health);
            _isInitialized = true;
        }
        
        public void TakeDamage(int damage)
        {
            _structureSkinRenderer.TakeDamage();
        }

        public void Die()
        {
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