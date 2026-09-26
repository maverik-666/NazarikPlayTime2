namespace Project
{
    using System;
    using DG.Tweening;
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class PauseMenu : MonoBehaviour
    {
        [Header("Elements")]
        [SerializeField]
        private RectTransform _wrap;
        
        [SerializeField]
        private RectTransform _could;
        
        [SerializeField] 
        private CanvasGroup _canvasGroup;

        [Header("Buttons")]
        
        [SerializeField]
        private PauseMenuButton _pauseButton;
        
        [SerializeField]
        private PauseMenuButton _menuButton;
        
        [SerializeField]
        private PauseMenuButton _restartButton;

        [SerializeField]
        private PauseMenuButton _playButton;
        
        [Header("Fly")]
        [SerializeField]
        private float _flyOffset = 20;

        [SerializeField]
        private float _flyDuration = 1;

        [SerializeField]
        private float _slideOffset = 200;

        private LevelLoader _levelLoader;
        private Sequence _seq;
        
        private bool _isInitialized = false;

        public void Init(LevelLoader levelLoader)
        {
            _levelLoader = levelLoader;
            
            _pauseButton.Init(OnPauseClick);
            _menuButton.Init(OnMenuClick);
            _restartButton.Init(OnRestartClick);
            _playButton.Init(OnPlayClick);
            
            HideInstant();
            AnimateFly();
            
            _isInitialized = true;
        }
        
        private void OnPauseClick()
        {
            Show();
        }
        
        private void OnPlayClick()
        {
            Hide();
        }

        private void OnMenuClick()
        {
            _levelLoader.LoadMainMenu();
        }

        private void OnRestartClick()
        {
            _levelLoader.LoadCurrent();
        }
        
        private void HideInstant()
        {
            _wrap.anchoredPosition += Vector2.up * -_slideOffset;
            _canvasGroup.alpha = 0f;
            _canvasGroup.blocksRaycasts = false;
        }

        private void Show()
        {
            StopTime();
            _canvasGroup.blocksRaycasts = true;
            
            _seq?.Kill(true);
            
            _seq = DOTween.Sequence();
            _seq.SetLink(gameObject, LinkBehaviour.KillOnDestroy);
            _seq.SetUpdate(true);
            _seq.Append(_wrap.DOAnchorPosY(_slideOffset, 0.5f)
                .SetRelative()
                .SetEase(Ease.OutBack));
            _seq.Join(_canvasGroup.DOFade(1, 0.3f)
                .SetEase(Ease.InOutSine));
            
        }

        private void Hide()
        {
            RenewTime();
            _canvasGroup.blocksRaycasts = false;
            
            _seq?.Kill(true);
            
            _seq = DOTween.Sequence();
            _seq.SetLink(gameObject, LinkBehaviour.KillOnDestroy);
            _seq.SetUpdate(true);
            _seq.Append(_wrap.DOAnchorPosY(-_slideOffset, 0.5f)
                .SetRelative()
                .SetEase(Ease.OutCubic));
            _seq.Join(_canvasGroup.DOFade(0, 0.15f)
                .SetEase(Ease.InOutSine));
        }

        private void AnimateFly()
        {
            _could.DOAnchorPosY(_flyOffset, _flyDuration)
                .SetLink(gameObject, LinkBehaviour.KillOnDestroy)
                .SetUpdate(true)
                .SetRelative()
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
        }

        private void StopTime()
        {
            Time.timeScale = 0;
        }
        
        private void RenewTime()
        {
            Time.timeScale = 1;
        }
        
#if UNITY_EDITOR
        private void Start()
        {
            if (!_isInitialized)
            {
                Debug.LogError($"{nameof(PauseMenu)} not initialized", this);
            }
        }
#endif
    }
}