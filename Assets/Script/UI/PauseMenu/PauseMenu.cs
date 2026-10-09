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
        
        [Header("Sounds")]
        [SerializeField]
        private SingleSoundData _clickSound;
        
        private TimeSpeedHandler _timeSpeedHandler;
        private LevelLoader _levelLoader;
        private Sequence _seq;
        
        private bool _isInitialized = false;

        public void Init(TimeSpeedHandler timeSpeedHandler, LevelLoader levelLoader)
        {
            _timeSpeedHandler = timeSpeedHandler;
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
            SoundManager.Instance.Play(_clickSound);
            MusicManager.Instance.Pause();
            Show();
        }
        
        private void OnPlayClick()
        {
            SoundManager.Instance.Play(_clickSound);
            MusicManager.Instance.Resume();
            
            Hide();
        }

        private void OnMenuClick()
        {
            SoundManager.Instance.Play(_clickSound);
            
            _levelLoader.LoadMainMenu();
        }

        private void OnRestartClick()
        {
            SoundManager.Instance.Play(_clickSound);
            
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
            _timeSpeedHandler.StopTime();
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
            _timeSpeedHandler.RenewTime();
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