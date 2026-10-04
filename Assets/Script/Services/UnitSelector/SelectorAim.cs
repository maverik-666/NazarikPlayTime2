namespace Project
{
    using DG.Tweening;
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(SpriteRenderer))]
    public class SelectorAim : MonoBehaviour
    { 
        [SerializeField]
        private float _lerpT = 0.5f;
        
        private Transform _target = null;

        private SpriteRenderer _spriteRenderer;
        private bool _isShown = false;

        private Tween _fadeTween = null;
        private bool _isInitialized = false;

        public void Init()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _spriteRenderer.SetAlpha(0);
            
            _isInitialized = true;
        }

        public void Update()
        {
            if (!_isShown)
            {
                return;
            }
            
            transform.position = Vector3.Lerp(transform.position, _target.position, _lerpT * Time.deltaTime);
            
            if ((transform.position - _target.position).sqrMagnitude < 0.02f)
            {
                transform.position = _target.position;
            }
        }

        public void Show(Transform target)
        {
            _target = target;
            transform.position = _target.position;
            _isShown = true;

            _fadeTween?.Kill();
            _fadeTween = _spriteRenderer.DOFade(1, 0.5f)
                .SetLink(gameObject, LinkBehaviour.KillOnDestroy)
                .SetEase(Ease.OutSine);
        }
        
        public void Hide()
        {
            _fadeTween?.Kill();
            _fadeTween = _spriteRenderer.DOFade(0, 0.5f)
                .SetLink(gameObject, LinkBehaviour.KillOnDestroy)
                .SetEase(Ease.OutSine)
                .OnComplete(AfterHide);
        }

        private void AfterHide()
        {
            _isShown = false;
            _target = null;
        }

        public void SetTarget(Transform target)
        {
            _target = target;
        }

        public void SetSprite(Sprite sprite)
        {
            _spriteRenderer.sprite = sprite;
        }

#if UNITY_EDITOR
        private void Start()
        {
            if (!_isInitialized)
            {
                Debug.LogError($"{nameof(SelectorAim)} not initialized", this);
            }
        }
#endif
    }
}