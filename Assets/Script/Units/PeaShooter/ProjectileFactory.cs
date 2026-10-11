namespace Project
{
    using UnityEngine;
    using UnityEngine.Pool;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class ProjectileFactory : MonoBehaviour
    {
        [SerializeField]
        private Projectile _projectilePf;

        [SerializeField]
        private int _defaultCapacity = 10;

        private DamageableRegistry _damageableRegistry;
        private ObjectPool<Projectile> _pool;
        private bool _isInitialized = false;

        public void Init(DamageableRegistry damageableRegistry)
        {
            _damageableRegistry = damageableRegistry;
            _pool = new ObjectPool<Projectile>(
                createFunc: CreateProjectile,
                actionOnGet: p => p.gameObject.SetActive(true),
                actionOnRelease: p => p.gameObject.SetActive(false),
                actionOnDestroy: p => Destroy(p.gameObject),
                defaultCapacity: _defaultCapacity
            );
            _isInitialized = true;
        }

        public void Create(Vector3 position, Team enemyTeam, float direction)
        {
            var projectile = _pool.Get();
            projectile.transform.position = position;
            projectile.StartFly(enemyTeam, direction);
        }

        private Projectile CreateProjectile()
        {
            var projectile = Instantiate(_projectilePf, transform);
            projectile.Init(_damageableRegistry, _pool.Release);
            projectile.gameObject.SetActive(false);
            
            return projectile;
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