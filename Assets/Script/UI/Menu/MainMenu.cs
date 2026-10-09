namespace Project
{
    using DG.Tweening;
    using UnityEngine;

    public class MainMenu : MonoBehaviour
    {
        [Header("Menu Buttons")]
        [SerializeField]
        private MenuButton _logo;
        
        [SerializeField]
        private MenuButton _playButton;
        
        [SerializeField]
        private MenuButton _quitButton;

        [SerializeField]
        private MenuButton _backButton;
        
        [SerializeField]
        private float _slideOffset = 1000;
        
        [Header("Character")]
        [SerializeField] 
        private RectTransform _character;
        
        [SerializeField]
        private GameObject _kris;

        [SerializeField]
        private GameObject _piano;
        
        [SerializeField]
        private GameObject _cat;

        [Header("Level Buttons")]
        [SerializeField]
        private LevelButton[] _levelButtons;
        
        [Header("Sounds")]
        [SerializeField]
        private SingleSoundData _clickSound;

        [SerializeField]
        private RandomSoundData _playtimeSound;
        
        private Progress _progress;

        private LevelInfoPresenter _levelInfoPresenter;

        private bool _isInitialized = false;
        
        private Sequence _menuSeq;
        private Sequence _levelSeq;
        private Tween _backButtonTween;
        
        public void Init(LevelLoader levelLoader, Progress progress)
        {
            _progress = progress;

            _levelInfoPresenter = GetComponentInChildren<LevelInfoPresenter>(true);
            _levelInfoPresenter.Init(levelLoader);
            
            _logo.Init(OnClickLogo);
            _playButton.Init(OnPlayClick);
            _quitButton.Init(OnQuitClick);
            _backButton.Init(OnBackClick);
            
            DefineCharacter();
            
            HideInstant();
            ShowMenu();
            
            foreach (var levelButton in _levelButtons)
            {
                levelButton.Init(OnLevelSelect, _progress.IsLevelUnlocked(levelButton.Level));
            }
            
            _isInitialized = true;
        }

     

        private void HideInstant()
        {
            _logo.Rect.anchoredPosition += Vector2.right * -_slideOffset;
            _playButton.Rect.anchoredPosition += Vector2.right * -_slideOffset;
            _quitButton.Rect.anchoredPosition += Vector2.right * -_slideOffset;
            _character.anchoredPosition += Vector2.right * _slideOffset;

            _backButton.Rect.localScale = Vector3.zero;
            _backButton.SetInteractable(false);
        }

        private void DefineCharacter()
        {
            if (_progress.IsLevelUnlocked(2) && Random.value > 0.9f)
            {
                _cat.SetActive(true);
                _piano.SetActive(false);
                _kris.SetActive(false);
            }
            else
            {
                _cat.SetActive(false);

                if (_progress.IsLevelUnlocked(6))
                {
                    _piano.SetActive(true);
                    _kris.SetActive(false);
                }
                else
                { 
                    _piano.SetActive(false);
                    _kris.SetActive(true);
                }
            }
        }
        
        private void ShowMenu()
        {
            _logo.SetInteractable(true);
            _playButton.SetInteractable(true);
            _quitButton.SetInteractable(true);
            
            _menuSeq?.Kill(true);
            _menuSeq = DOTween.Sequence();
            
            _menuSeq.Append(_logo.Rect.DOAnchorPosX(_slideOffset, 0.5f)
                .SetRelative()
                .SetEase(Ease.OutBack));
            
            _menuSeq.Insert(0.15f, _playButton.Rect.DOAnchorPosX(_slideOffset, 0.5f)
                .SetRelative()
                .SetEase(Ease.OutBack));
            
            _menuSeq.Insert(0.3f, _quitButton.Rect.DOAnchorPosX(_slideOffset, 0.5f)
                .SetRelative()
                .SetEase(Ease.OutBack));
            
            _menuSeq.Insert(0, _character.DOAnchorPosX(-_slideOffset, 1)
                .SetRelative()
                .SetEase(Ease.OutBack));
        }

        private void HideMenu()
        {
            _logo.SetInteractable(false);
            _playButton.SetInteractable(false);
            _quitButton.SetInteractable(false);
            
            _menuSeq?.Kill(true);
            _menuSeq = DOTween.Sequence();
            
            _menuSeq.Append(_logo.Rect.DOAnchorPosX(-_slideOffset, 0.5f)
                .SetRelative()
                .SetEase(Ease.InBack));
            
            _menuSeq.Insert(0.15f, _playButton.Rect.DOAnchorPosX(-_slideOffset, 0.5f)
                .SetRelative()
                .SetEase(Ease.InBack));
            
            _menuSeq.Insert(0.3f, _quitButton.Rect.DOAnchorPosX(-_slideOffset, 0.5f)
                .SetRelative()
                .SetEase(Ease.InBack));
            
            _menuSeq.Insert(0, _character.DOAnchorPosX(_slideOffset, 1)
                .SetRelative()
                .SetEase(Ease.InBack));
        }

        private void ShowLevels()
        {
            _backButton.SetInteractable(true);
            
            _levelSeq?.Kill();
            
            _levelSeq = DOTween.Sequence();
            _levelSeq.SetLink(gameObject, LinkBehaviour.KillOnDestroy);
         
            foreach (var levelButton in _levelButtons)
            {
                _levelSeq.AppendInterval(0.05f);
                _levelSeq.AppendCallback(levelButton.Show);
            }
            
            _levelSeq.Insert(0, _backButton.Rect.DOScale(1, 0.5f).SetEase(Ease.OutBack));
        }

        private void HideLevels()
        {
            _backButton.SetInteractable(false);
            
            _levelSeq?.Kill();
            _levelSeq = DOTween.Sequence();
            _levelSeq.SetLink(gameObject, LinkBehaviour.KillOnDestroy);

            _levelSeq.AppendCallback(_levelInfoPresenter.Hide);
            
            for (int i = _levelButtons.Length - 1; i >= 0; i--)
            {
                var levelButton = _levelButtons[i];
                _levelSeq.AppendInterval(0.05f);
                _levelSeq.AppendCallback(levelButton.Hide);
            }
            
            _levelSeq.Insert(0, _backButton.Rect.DOScale(0, 0.5f).SetEase(Ease.InBack));
        }

        private void OnBackClick()
        {
            SoundManager.Instance.Play(_clickSound);
            
            var seq = DOTween.Sequence();
            seq.SetLink(gameObject, LinkBehaviour.KillOnDestroy);

            seq.AppendCallback(HideLevels);
            seq.AppendInterval(0.5f);
            seq.AppendCallback(ShowMenu);
        }

        private void OnClickLogo()
        {
            SoundManager.Instance.Play(_clickSound);
            SoundManager.Instance.Play(_playtimeSound.GetRandomSound());
        }
        
        private void OnPlayClick()
        {
            SoundManager.Instance.Play(_clickSound);
            
            var seq = DOTween.Sequence();
            seq.SetLink(gameObject, LinkBehaviour.KillOnDestroy);

            seq.AppendCallback(HideMenu);
            seq.AppendInterval(0.5f);
            seq.AppendCallback(ShowLevels);
        }

        private void OnQuitClick()
        {
            SoundManager.Instance.Play(_clickSound);
            
            Application.Quit();
        }

        private void OnLevelSelect(LevelButton selected)
        {
            SoundManager.Instance.Play(_clickSound);
            
            foreach (var levelButton in _levelButtons)
            {
                if (levelButton == selected)
                {
                    levelButton.Select();
                    _levelInfoPresenter.SelectLevel(levelButton);
                }
                else
                {
                    levelButton.Deselect();
                }
            }
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