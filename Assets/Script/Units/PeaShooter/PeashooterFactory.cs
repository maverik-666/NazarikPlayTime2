namespace Project
{
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(ProjectileFactory))]
    public class PeashooterFactory : MonoBehaviour, IUnitFactory
    {
        [SerializeField]
        private Peashooter _peaShooterPf;
        
        private ProjectileFactory _projectileFactory;
        private Registry<IDamageable> _damageableRegistry;
        private bool _isInitialized = false;

        public void Init(Registry<IDamageable> damageableRegistry)
        {
            _damageableRegistry = damageableRegistry;

            _projectileFactory = GetComponent<ProjectileFactory>();
            _projectileFactory.Init(damageableRegistry);
            
            _isInitialized = true;
        }

        public void Create(Vector3 position, Team team, Team enemyTeam, float direction)
        {
            var unit = Instantiate(_peaShooterPf, position, Quaternion.identity, transform);
            unit.Init(team, enemyTeam, direction, _damageableRegistry, _projectileFactory);
            _damageableRegistry.Add(unit);
        }

#if UNITY_EDITOR
        private void Start()
        {
            if (!_isInitialized)
            {
                Debug.LogError($"{nameof(PeashooterFactory)} not initialized", this);
            }
        }
#endif
    }
}