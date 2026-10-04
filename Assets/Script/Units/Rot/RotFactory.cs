namespace Project
{
    using DG.Tweening;
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class RotFactory : MonoBehaviour, IUnitFactory
    {
        [SerializeField]
        private Rot _rotPf;
        
        private DamageableRegistry _damageableRegistry;
        private UnitRegistry _unitRegistry;
        private bool _isInitialized = false;

        public void Init(DamageableRegistry damageableRegistry, UnitRegistry unitRegistry)
        {
            _damageableRegistry = damageableRegistry;
            _unitRegistry = unitRegistry;
            _isInitialized = true;
        }

        public void Create(Vector3 position, Team team, Team enemyTeam, float direction)
        {
            var unit = Instantiate(_rotPf, position, Quaternion.identity, transform);
            unit.Init(team, enemyTeam, direction, _damageableRegistry, _unitRegistry);

            _damageableRegistry.Add(unit);
            _unitRegistry.Add(unit);
        }

#if UNITY_EDITOR
        private void Start()
        {
            if (!_isInitialized)
            {
                Debug.LogError($"{nameof(RotFactory)} not initialized", this);
            }
        }
#endif
    }
}