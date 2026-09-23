namespace Project
{
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class UnitFactory : MonoBehaviour
    {
        [SerializeField]
        private Unit _unitPf;
        
        private Registry<IDamageable> _damageableRegistry;
        private bool _isInitialized = false;

        public void Init(Registry<IDamageable> damageableRegistry)
        {
            _damageableRegistry = damageableRegistry;
            _isInitialized = true;
        }

        public void Create(Vector3 position, Team team, Team enemyTeam, float direction)
        {
            var unit = Instantiate(_unitPf, position, Quaternion.identity, transform);
            unit.Init(team, enemyTeam, direction, _damageableRegistry);
            _damageableRegistry.Add(unit);
        }

#if UNITY_EDITOR
        private void Start()
        {
            if (!_isInitialized)
            {
                Debug.LogError($"{nameof(UnitFactory)} not initialized", this);
            }
        }
#endif
    }
}