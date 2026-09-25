namespace Project
{
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class GolemRemains : MonoBehaviour, IDamageable
    {
        [SerializeField]
        private int _heathPoints = 50;
        
        private Team _team = Team.Red;
        
        private float _direction = 1;
        
        private Health _health;

        private GolemRemainsSkinRenderer _skinRenderer;
        private Registry<IDamageable> _damageableRegistry;
        private JumpAnimator _jumpAnimator;
        
        private bool _canNotTakeDamage = false;
        private bool _isInitialized = false;

        public Vector2 Position => transform.position;
        public Health Health => _health;
        public Team Team => _team;

        public bool CanNotTakeDamage => _canNotTakeDamage;
        
        public void Init(Team team, float direction, Registry<IDamageable> damageableRegistry)
        {
            _team = team;
            _direction = direction;
            
            _health = new Health(_heathPoints);
            _skinRenderer = GetComponentInChildren<GolemRemainsSkinRenderer>();
            _jumpAnimator = new JumpAnimator(transform);

            _damageableRegistry = damageableRegistry;
            
            _skinRenderer.Init();
            
            SetDirection(_direction);
            _isInitialized = true;
        }
        
        public void SetInvincible(bool value)
        {
            _canNotTakeDamage = value;
        }
        
        public void TakeDamage(int damage)
        {
            _health.TakeDamage(damage);

            if (_health.IsDead)
            {
                AnimateDying();
            }
            
            _skinRenderer.TakeDamage();
        }

        private void AnimateDying()
        {
            _skinRenderer.Tilt(_direction);
            _jumpAnimator.Play(-_direction, Die);
        }

        private void Die()
        {
            _canNotTakeDamage = true;
            _damageableRegistry.Remove(this);
            
            _skinRenderer.Die();
        }

        private void Update()
        {
            _jumpAnimator.Tick(Time.deltaTime);
        }

        public void SetDirection(float direction)
        {
            _direction = Mathf.Sign(direction);
            transform.localScale = new Vector3(_direction, 1, 1);
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