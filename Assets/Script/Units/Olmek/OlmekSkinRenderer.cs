namespace Project
{
    using System;
    using DG.Tweening;
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(SpriteRenderer))]
    public class OlmekSkinRenderer : MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer _shadow;
        
        [SerializeField]
        private VFXSource _fallVfx;

        [SerializeField]
        private float _fallDuration = 2;

        [SerializeField]
        private float _fallHeight = 10;
        
        [SerializeField]
        private float _jumpHeight = 10;
        
        [Header("Shaders")]
        [SerializeField]
        private float _flashColorTo = 0.4f;

        [SerializeField]
        private float _flashDuration = 0.15f;
        
                
        [Header("Sounds")]
        [SerializeField]
        private RandomSoundData _landSound;
        
        private SpriteRenderer _spriteRenderer;
        private SpriteFlasher _spriteFlasher;
        private Tween _shakeTween;
        private bool _isInitialized = false;

        public void Init()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _spriteFlasher = new SpriteFlasher(gameObject, _spriteRenderer);
            _spriteRenderer.enabled = false;
            _shadow.enabled = false;
            
            _fallVfx.Init();
            
            _isInitialized = true;
        }

        public void AnimateFall(Action callback)
        {
            var originalPos = transform.position; 
            transform.position += Vector3.up * _fallHeight;
            _shadow.transform.localScale = Vector3.zero;
            
            _spriteRenderer.enabled = true;
            _shadow.enabled = true;
            
            var seq = DOTween.Sequence();
            seq.SetLink(gameObject, LinkBehaviour.KillOnDestroy);
            seq.Append(transform.DOMoveY(originalPos.y, _fallDuration).SetEase(Ease.InQuad));
            seq.Join(transform.DOScale(Vector3.one, _fallDuration).SetEase(Ease.InQuad));
            seq.AppendCallback(callback.Invoke);
            seq.AppendCallback(FlashWhite);
            seq.AppendCallback(PlayLandSound);
            seq.Append(transform.DOShakePosition(0.15f, new Vector3(0.125f, 0, 0)));
        }

        private void FlashWhite()
        {
            _spriteFlasher.FlashColor(Color.white, _flashColorTo, _flashDuration);
            _fallVfx.Play();
        }

        private void PlayLandSound()
        {
            SoundManager.Instance.Play(_landSound.GetRandomSound());
        }

        public void JumpOffScreen(Action callback)
        {
            var seq = DOTween.Sequence();
            seq.SetLink(gameObject, LinkBehaviour.KillOnDestroy);
            seq.Append(transform.DOJump(transform.position + Vector3.right * 20f, _jumpHeight, 1, _fallDuration)
                .SetEase(Ease.InQuad));
            seq.Join(transform.DOScale(Vector3.one, _fallDuration).SetEase(Ease.InQuad));
            seq.AppendCallback(callback.Invoke);
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