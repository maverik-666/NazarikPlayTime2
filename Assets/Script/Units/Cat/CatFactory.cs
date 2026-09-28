namespace Project
{
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class CatFactory : MonoBehaviour, IUnitFactory
    {
        [SerializeField]
        private Cat _catPf;
        
        private DamageableRegistry _damageableRegistry;
        private bool _isInitialized = false;

        public void Init(DamageableRegistry damageableRegistry)
        {
            _damageableRegistry = damageableRegistry;
            _isInitialized = true;
        }

        public void Create(Vector3 position, Team team, Team enemyTeam, float direction)
        {
            var unit = Instantiate(_catPf, position, Quaternion.identity, transform);
            unit.Init(team, enemyTeam, direction, _damageableRegistry);
            _damageableRegistry.Add(unit);
        }

#if UNITY_EDITOR
        private void Start()
        {
            if (!_isInitialized)
            {
                Debug.LogError($"{nameof(CatFactory)} not initialized", this);
            }
        }
#endif
    }
}