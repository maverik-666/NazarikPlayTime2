namespace Project
{
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(GhostFactory))]
    public class AnimatronicFactory : MonoBehaviour, IUnitFactory
    {
        [SerializeField]
        private Animatronic _animatronicPf;
        
        private GhostFactory _ghostFactory;
        private DamageableRegistry _damageableRegistry;
        private UnitRegistry _unitRegistry;
        private bool _isInitialized = false;

        public void Init(DamageableRegistry damageableRegistry, UnitRegistry unitRegistry)
        {
            _damageableRegistry = damageableRegistry;
            _unitRegistry = unitRegistry;
            _ghostFactory = GetComponent<GhostFactory>();
            _ghostFactory.Init(damageableRegistry, unitRegistry);

            _isInitialized = true;
        }

        public void Create(Vector3 position, Team team, Team enemyTeam, float direction)
        {
            var unit = Instantiate(_animatronicPf, position, Quaternion.identity, transform);
            unit.Init(team, enemyTeam, direction, _damageableRegistry, _ghostFactory);
            _damageableRegistry.Add(unit);
            _unitRegistry.Add(unit);
        }

#if UNITY_EDITOR
        private void Start()
        {
            if (!_isInitialized)
            {
                Debug.LogError($"{nameof(AnimatronicFactory)} not initialized", this);
            }
        }
#endif
    }
}