namespace Project
{
    using DG.Tweening;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class ServiceButton : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _text;
        
        [SerializeField]
        private Slider _slider;
        
        [SerializeField]
        private Button _button;

        [SerializeField]
        private Image _frame;
        
        [SerializeField]
        private CanvasGroup _frameLit;
        
        private IService _service; 
        
        private Tween _jumpTween;
        private Tween _litTween;

        private bool _isInitialized = false;

        public void Init(IService service)
        {
            _service = service;
            _service.StateChanged += UpdateState;
            _frameLit.alpha = 0;
            UpdateState();

            _text.text = $"{_service.Price}";
            
            _button.onClick.AddListener(OnClick);
            _isInitialized = true;
        }

        private void OnClick()
        {
            if (_service.IsAvailable)
            {
                _service.Execute();
                
                _jumpTween?.Kill(true);
                _jumpTween = transform.DOMoveY(10, 0.3f)
                    .SetRelative()
                    .SetLink(gameObject, LinkBehaviour.KillOnDisable)
                    .SetEase(Ease.OutCubic)
                    .SetLoops(2, LoopType.Yoyo);
            }
            else
            {
                _jumpTween?.Kill(true);
                _jumpTween = transform.DOShakePosition(0.3f, Vector2.right * 2)
                    .SetLink(gameObject, LinkBehaviour.KillOnDisable)
                    .SetEase(Ease.OutCubic);
            }
        }

        private void Update()
        {
            _slider.value = _service.CurrentCooldown / _service.Cooldown;
        }
        
        private void UpdateState()
        {
            _litTween?.Kill();
            _litTween = _frameLit.DOFade(_service.IsAvailable ? 1 : 0, 0.1f)
                .SetLink(gameObject, LinkBehaviour.KillOnDisable)
                .SetEase(Ease.InSine)
                .SetLoops(_service.IsAvailable ? 3 : 1, LoopType.Yoyo);
        }
        
        private void OnDestroy()
        {
            _service.StateChanged -= UpdateState;
            _button.onClick.RemoveListener(OnClick);
        }
        
#if UNITY_EDITOR
        private void Start()
        {
            if (!_isInitialized)
            {
                Debug.LogError($"{nameof(ServiceButton)} not initialized", this);
            }
        }
#endif
    }
}