namespace Project
{
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(GolemRemainsFactory))]
    public class GolemFactory : MonoBehaviour, IUnitFactory
    {
        [SerializeField]
        private Golem _golemPf;
        
        private DamageableRegistry _damageableRegistry;
        private GolemRemainsFactory _golemRemainsFactory;
        private UnitRegistry _unitRegistry;
        private bool _isInitialized = false;

        public void Init(DamageableRegistry damageableRegistry, UnitRegistry unitRegistry)
        {
            _damageableRegistry = damageableRegistry;
            _unitRegistry = unitRegistry;
            
            _golemRemainsFactory = GetComponent<GolemRemainsFactory>();
            _golemRemainsFactory.Init(damageableRegistry);
            
            _isInitialized = true;
        }

        public void Create(Vector3 position, Team team, Team enemyTeam, float direction)
        {
            var unit = Instantiate(_golemPf, position, Quaternion.identity, transform);
            unit.Init(team, enemyTeam, direction, _damageableRegistry, _golemRemainsFactory);
            _damageableRegistry.Add(unit);
            _unitRegistry.Add(unit);
        }

#if UNITY_EDITOR
        private void Start()
        {
            if (!_isInitialized)
            {
                Debug.LogError($"{nameof(GolemFactory)} not initialized", this);
            }
        }
#endif
    }
}