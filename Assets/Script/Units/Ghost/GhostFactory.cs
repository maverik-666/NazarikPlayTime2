namespace Project
{
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class GhostFactory : MonoBehaviour, IUnitFactory
    {
        [SerializeField]
        private Ghost _ghostPf;

        [SerializeField]
        private Team _ghostTeam = Team.None;
        
        private DamageableRegistry _damageableRegistry;
        private bool _isInitialized = false;

        public void Init(DamageableRegistry damageableRegistry)
        {
            _damageableRegistry = damageableRegistry;
            _isInitialized = true;
        }

        public void Create(Vector3 position, Team team, Team enemyTeam, float direction)
        {
            var unit = Instantiate(_ghostPf, position, Quaternion.identity, transform);
            unit.Init(_ghostTeam, enemyTeam, direction, _damageableRegistry);
            _damageableRegistry.Add(unit);
        }

#if UNITY_EDITOR
        private void Start()
        {
            if (!_isInitialized)
            {
                Debug.LogError($"{nameof(GhostFactory)} not initialized", this);
            }
        }
#endif
    }
}