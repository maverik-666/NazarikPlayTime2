namespace Project
{
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class GhostFactory : MonoBehaviour, IUnitFactory
    {
        [SerializeField]
        private Ghost _ghostPf;

        private DamageableRegistry _damageableRegistry;
        private UnitRegistry _unitRegistry;
        private bool _isInitialized = false;

        public void Init(DamageableRegistry damageableRegistry, UnitRegistry unitRegistry)
        {
            _damageableRegistry = damageableRegistry;
            _unitRegistry = unitRegistry;    
            _isInitialized = true;
        }

        public IDamageable Create(Vector3 position, Team team, Team enemyTeam, float direction)
        {
            var unit = Instantiate(_ghostPf, position, Quaternion.identity, transform);
            unit.Init(team, enemyTeam, direction, _damageableRegistry);
            _damageableRegistry.Add(unit);
            _unitRegistry.Add(unit);
            
            return unit;
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