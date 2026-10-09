namespace Project
{
    using DG.Tweening;
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class GolemRemainsSkinRenderer : MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer _shadow;
        
        [SerializeField]
        private ParticleSystem _dieParticle;
        
        [SerializeField]
        private float _tiltRotation = 30f;
        
        [Header("Shaders")]
        [SerializeField]
        private float _flashColorTo = 0.4f;

        [SerializeField]
        private float _flashDuration = 0.15f;

        [Header("Sounds")]
        [SerializeField]
        private RandomSoundData _deathSound;

        private SpriteRenderer _spriteRenderer;
        private SpriteFlasher _spriteFlasher;
        private Tween _shakeTween;
        private bool _isInitialized = false;

        public void Init()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _spriteFlasher = new SpriteFlasher(gameObject, _spriteRenderer);
            _isInitialized = true;
        }

        public void TakeDamage()
        {
            _shakeTween?.Kill(true); 
            _shakeTween = transform.DOShakePosition(0.15f, new Vector3(0.125f, 0, 0), 10, 90, false)
                .SetLink(gameObject, LinkBehaviour.KillOnDisable);
            _spriteFlasher.FlashColor(Color.red, _flashColorTo, _flashDuration);
        }

        public void Tilt(float direction)
        {
            _shakeTween?.Kill(true); 
            transform.rotation = Quaternion.Euler(0f, 0f, _tiltRotation * direction); 
        }

        public void Die()
        {
            _spriteRenderer.enabled = false;
            _shadow.enabled = false;
            _dieParticle.Play();
            
            SoundManager.Instance.Play(_deathSound.GetRandomSound());
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