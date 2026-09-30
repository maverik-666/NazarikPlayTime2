namespace Project
{
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class OlmekFactory : MonoBehaviour
    {
        [SerializeField]
        private Olmek _olmekPf;

        [SerializeField]
        private float _floorY = 1.25f;
        
        private DamageableRegistry _damageableRegistry;
        private CameraMover _cameraMover;
        private bool _isInitialized = false;

        public void Init(CameraMover cameraMover, DamageableRegistry damageableRegistry)
        {
            _damageableRegistry = damageableRegistry;
            _cameraMover = cameraMover;
            
            _isInitialized = true;
        }

        public void Create(Vector3 position, Team team)
        {
            position.y = _floorY;
            
            var olmek = Instantiate(_olmekPf, position, Quaternion.identity, transform);
            olmek.Init(team, _cameraMover, _damageableRegistry);
        }
        
#if UNITY_EDITOR
        private void Start()
        {
            if (!_isInitialized)
            {
                Debug.LogError($"{nameof(OlmekFactory)} not initialized", this);
            }
        }
#endif
    }
}