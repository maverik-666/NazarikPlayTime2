namespace Project
{
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class ProjectileFactory : MonoBehaviour
    {
        [SerializeField]
        private Projectile _projectilePf;
        
        private Registry<IDamageable> _damageableRegistry;
        private bool _isInitialized = false;

        public void Init(Registry<IDamageable> damageableRegistry)
        {
            _damageableRegistry = damageableRegistry;
            _isInitialized = true;
        }

        public void Create(Vector3 position, Team enemyTeam, float direction)
        {
            var projectile = Instantiate(_projectilePf, position, Quaternion.identity, transform);
            projectile.Init(enemyTeam, direction, _damageableRegistry);
        }

#if UNITY_EDITOR
        private void Start()
        {
            if (!_isInitialized)
            {
                Debug.LogError($"{nameof(ProjectileFactory)} not initialized", this);
            }
        }
#endif
    }
}