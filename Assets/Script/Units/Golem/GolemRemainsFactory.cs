namespace Project
{
    using UnityEngine;

    public class GolemRemainsFactory : MonoBehaviour
    {
        [SerializeField]
        private GolemRemains _golemPf;

        private Registry<IDamageable> _damageableRegistry;
        private bool _isInitialized = false;

        public void Init(Registry<IDamageable> damageableRegistry)
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
                Debug.LogError($"{nameof(GolemRemainsFactory)} not initialized", this);
            }
        }
#endif
    }
}