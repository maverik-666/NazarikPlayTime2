namespace Project
{
    using System;
    using DG.Tweening;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using Random = UnityEngine.Random;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(CanvasGroup))]
    [RequireComponent(typeof(RectTransform))]
    public class LevelButton : MonoBehaviour, IPointerClickHandler
    {
        [Header("Level Info")]
        [SerializeField]
        private int _level;

        [SerializeField]
        private string _title;
        
        [TextArea]
        [SerializeField]
        private string _description;
        
        [Header("Elements")]
        [SerializeField]
        private RectTransform _perl;

        [SerializeField]
        private CanvasGroup _highlightSelect;
        
        [SerializeField]
        private CanvasGroup _highlightLocked;
        
        [Header("Fly")]
        [SerializeField]
        private float _flyOffset = 20;

        [SerializeField]
        private float _flyDuration = 1;

        [SerializeField]
        private Vector2 _gotoRange = new Vector2(0, 2);

        [SerializeField]
        private float _slideOffset = 200;
        
        [Header("Sounds")]
        [SerializeField]
        private SingleSoundData _successSound;
        
        [SerializeField]
        private SingleSoundData _failureSound;
        
        private Action<LevelButton> _selectAction;
        
        private RectTransform _wrap;
        private CanvasGroup _canvasGroup;

        private Sequence _seq;
        private Tween _fadeTween;
        private Tween _lockedTween;
        
        private bool _isUnlocked;
        private bool _isInitialized = false;

        public int Level => _level;

        public string Title => _title;

        public string Description => _description;

        public void Init(Action<LevelButton> selectAction, bool isUnlocked)
        {
            _selectAction = selectAction;
            _highlightLocked.alpha = 0;
            
            _isUnlocked = isUnlocked;
            
            AnimateFly();
            _wrap = GetComponent<RectTransform>();
            _canvasGroup = GetComponent<CanvasGroup>();
            
            HideInstant();

            _isInitialized = true;
        }

        private void HideInstant()
        {
            _wrap.anchoredPosition += Vector2.up * -_slideOffset;
            _canvasGroup.alpha = 0f;
            _canvasGroup.blocksRaycasts = false;
            
        }

        public void Show()
        {
            _seq?.Kill(true);

            _canvasGroup.blocksRaycasts = true;
            
            _seq = DOTween.Sequence();
            _seq.SetLink(gameObject, LinkBehaviour.KillOnDestroy);
            _seq.Append(_wrap.DOAnchorPosY(_slideOffset, 0.5f)
                .SetRelative()
                .SetEase(Ease.OutBack));
            _seq.Join(_canvasGroup.DOFade(1, 0.3f)
                .SetEase(Ease.InOutSine));
        }

        public void Hide()
        {
            _seq?.Kill(true);
            
            _canvasGroup.blocksRaycasts = false;
            
            _seq = DOTween.Sequence();
            _seq.SetLink(gameObject, LinkBehaviour.KillOnDestroy);
            _seq.Append(_wrap.DOAnchorPosY(-_slideOffset, 0.5f)
                .SetRelative()
                .SetEase(Ease.InBack));
            _seq.Join(_canvasGroup.DOFade(0, 0.3f)
                .SetEase(Ease.InOutSine));
            
            Deselect();
        }

        private void AnimateFly()
        {
            var seq = _perl.DOAnchorPosY(_flyOffset, _flyDuration)
                .SetLink(gameObject, LinkBehaviour.KillOnDestroy)
                .SetRelative()
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
            
            seq.Goto( Random.Range(_gotoRange.x, _gotoRange.y),true);
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            if (_isUnlocked)
            {
                _selectAction(this);
            }
            else
            {
                Locked();
            }
        }

        private void Locked()
        {
            SoundManager.Instance.Play(_failureSound);
            
            _lockedTween?.Kill(true);
            _lockedTween = _highlightLocked.DOFade(1, 0.1f)
                .SetLink(gameObject, LinkBehaviour.KillOnDestroy)
                .SetEase(Ease.OutSine)
                .SetLoops(6, LoopType.Yoyo);
        }

        public void Select()
        {
            SoundManager.Instance.Play(_successSound);
            
            _fadeTween?.Kill();
            
            _fadeTween = _highlightSelect.DOFade(1, 0.3f)
                .SetLink(gameObject, LinkBehaviour.KillOnDestroy)
                .SetEase(Ease.InSine);
        }
        
        

        public void Deselect()
        {
            _fadeTween?.Kill();
            _fadeTween = _highlightSelect.DOFade(0, 0.3f)
                .SetLink(gameObject, LinkBehaviour.KillOnDestroy)
                .SetEase(Ease.OutSine);
        }

#if UNITY_EDITOR
        private void Start()
        {
            if (!_isInitialized)
            {
                Debug.LogError($"{nameof(LevelButton)} not initialized", this);
            }
        }
#endif
    }
}