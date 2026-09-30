namespace Project
{
    using DG.Tweening;
    using TMPro;
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class StructureSkinRenderer : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _healthText;

        [SerializeField]
        private ParticleSystem _dieParticle;
        
        [Header("Sounds")]
        [SerializeField]
        private RandomSoundData _damageSound;
        
        private Health _health; 
        private Tween _shakeTween;
        private bool _isInitialized = false;

        public void Init(Health health)
        {
            _health = health;
            UpdateHealthText();
            _isInitialized = true;
        }

        public void TakeDamage()
        {
            _shakeTween?.Kill(true); 
            _shakeTween = transform.DOShakePosition(0.15f, new Vector3(0.25f, 0, 0), 10, 90, false)
                .SetLink(gameObject, LinkBehaviour.KillOnDisable);
            
            SoundManager.Instance.Play(_damageSound.GetRandomSound());

            UpdateHealthText();
        }

        private void UpdateHealthText()
        {
            _healthText.text = $"{_health.Current}/{_health.Max}";
        }

        public void Die()
        {
            _shakeTween?.Kill(true);
            _shakeTween = transform.DOShakePosition(0.15f, new Vector3(0.25f, 0, 0))
                .SetLink(gameObject, LinkBehaviour.KillOnDisable)
                .SetUpdate(true)
                .SetLoops(-1, LoopType.Restart);
            
            UpdateHealthText();
            
            _dieParticle.Play();
        }

#if UNITY_EDITOR
        private void Start()
        {
            if (!_isInitialized)
            {
                Debug.LogError($"{nameof(StructureSkinRenderer)} not initialized", this);
            }
        }
#endif
    }
}