namespace Project
{
    using UnityEngine;

    public class GolemRemainsFactory : MonoBehaviour
    {
        [SerializeField]
        private GolemRemains _golemPf;

        private DamageableRegistry _damageableRegistry;
        private bool _isInitialized = false;

        public void Init(DamageableRegistry damageableRegistry)
        {
            _damageableRegistry = damageableRegistry;
            _isInitialized = true;
        }

        public void Create(Vector3 position, Team team, float direction)
        {
            var unit = Instantiate(_golemPf, position, Quaternion.identity, transform);
            unit.Init(team, direction, _damageableRegistry);
            _damageableRegistry.Add(unit);
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