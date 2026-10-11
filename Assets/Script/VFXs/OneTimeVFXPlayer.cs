namespace Project
{
    using UnityEngine;

    public class OneTimeVFXPlayer : MonoBehaviour
    {
        [SerializeField] 
        private ParticleSystem _vfx;

        [SerializeField]
        private VFXId _vfxId;
        
        private ParticleSystem _vfxLeft;

        private bool _isInitialized = false;

        public VFXId VFXId => _vfxId;

        public void Init()
        {
            _vfxLeft = Instantiate(_vfx, transform);
            _vfxLeft.transform.localScale = new Vector3(-_vfx.transform.localScale.x, 1, 1);
            _isInitialized = true;
        }
        
        public void Play(Vector3 position, float direction)
        {
            var vfx = GetVFXBy(direction);
            
            vfx.transform.position = position;
            vfx.Play(true);
        }

        private ParticleSystem GetVFXBy(float direction)
        {
            return direction > 0 ? _vfx : _vfxLeft;
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