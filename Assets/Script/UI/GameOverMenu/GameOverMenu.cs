namespace Project
{
    using System;
    using DG.Tweening;
    using TMPro;
    using UnityEngine;
    using Random = UnityEngine.Random;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class GameOverMenu : MonoBehaviour
    {
        [Header("Texts")]
        [SerializeField]
        private string[] _winTitles;
        
        [TextArea]
        [SerializeField]
        private string[] _winDescriptions;
        
        [SerializeField]
        private string[] _loseTitles;
        
        [TextArea]
        [SerializeField]
        private string[] _loseDescriptions;
        
        [SerializeField]
        private TMP_Text _titleText;
        
        [SerializeField]
        private TMP_Text _descriptionText;

        [Header("Elements")]
        [SerializeField]
        private CanvasGroup _rootCanvasGroup;
        
        [SerializeField]
        private CanvasGroup _titleGroup;
        
        [SerializeField]
        private CanvasGroup _descriptionGroup;
        
        [SerializeField]
        private CanvasGroup _menuGroup;
        
        [SerializeField]
        private CanvasGroup _winCanvasGroup;
        
        [SerializeField]
        private CanvasGroup _loseCanvasGroup;
        
        [SerializeField]
        private CanvasGroup _unlock1CanvasGroup;
        
        [SerializeField]
        private CanvasGroup _unlock2CanvasGroup;

        [SerializeField]
        private RectTransform _loseRect;
        
        [SerializeField]
        private RectTransform _winRect;
        
        [Header("Buttons")]
        [SerializeField]
        private PauseMenuButton _continueButton;
        
        [SerializeField]
        private PauseMenuButton _restartButton;
        
        [SerializeField]
        private PauseMenuButton _quitButton;
        
        [Header("Sounds")]
        [SerializeField]
        private SingleSoundData _clickSound;

        [SerializeField]
        private SingleSoundData _winSound;
        
        [SerializeField]
        private RandomSoundData _winPhraseSound;

        [SerializeField]
        private float _winPhraseDelay = 3;
        
        [SerializeField]
        private SingleSoundData _loseSound;
        
        [SerializeField]
        private RandomSoundData _losePhraseSound;

        [SerializeField]
        private float _losePhraseDelay = 3;
        
        private LevelLoader _levelLoader;
        
        private bool _isInitialized = false;

        public void Init(LevelLoader levelLoader)
        {
            _levelLoader = levelLoader;
            _continueButton.Init(OnContinueClick);
            _restartButton.Init(OnRestartClick);
            _quitButton.Init(OnQuitClick);

            _rootCanvasGroup.blocksRaycasts = false;
            _rootCanvasGroup.alpha = 0;
            
            _titleGroup.alpha = 0;
            _descriptionGroup.alpha = 0;
            _menuGroup.alpha = 0;

            _loseRect.anchoredPosition += Vector2.up * 400;
            _winRect.anchoredPosition += Vector2.up * -400;
            
            _isInitialized = true;
        }

        private void OnQuitClick()
        {
            SoundManager.Instance.Play(_clickSound);

            _levelLoader.LoadMainMenu();
        }

        private void OnRestartClick()
        {
            SoundManager.Instance.Play(_clickSound);

            _levelLoader.LoadCurrent();
        }

        private void OnContinueClick()
        {
            SoundManager.Instance.Play(_clickSound);

            _levelLoader.LoadMainMenu();
        }

        public void ShowEnemyWin()
        {
            _titleText.text = _loseTitles[Random.Range(0, _loseTitles.Length)];
            _descriptionText.text = _loseDescriptions[Random.Range(0, _loseDescriptions.Length)];

            _winCanvasGroup.alpha = 0;
            _winCanvasGroup.blocksRaycasts = false;
            _loseCanvasGroup.alpha = 1;
            _loseCanvasGroup.blocksRaycasts = true;
            
            MusicManager.Instance.Stop();
            
            SoundManager.Instance.Play(_loseSound);
            DOTween.Sequence()
                .SetLink(gameObject, LinkBehaviour.KillOnDisable)
                .SetUpdate(true)
                .AppendInterval(_losePhraseDelay)
                .AppendCallback(PlayLosePhrase);
            
            DOTween.Sequence()
                .SetLink(gameObject, LinkBehaviour.KillOnDestroy)
                .SetUpdate(true)
                .AppendInterval(0.5f)
                .AppendCallback(Show)
                .Insert(0.25f, _loseRect.DOAnchorPosY(0, 1).SetEase(Ease.OutSine));
        }
        
        private void PlayLosePhrase()
        {
            SoundManager.Instance.Play(_losePhraseSound.GetRandomSound());
        }
        
        public void ShowPlayerWin()
        {
            _titleText.text = _winTitles[Random.Range(0, _winTitles.Length)];
            _descriptionText.text = _winDescriptions[Random.Range(0, _winDescriptions.Length)];

            _winCanvasGroup.alpha = 1;
            _winCanvasGroup.blocksRaycasts = true;
            _loseCanvasGroup.alpha = 0;
            _loseCanvasGroup.blocksRaycasts = false;
            
            Flash(_unlock1CanvasGroup);
            Flash(_unlock2CanvasGroup);
            
            MusicManager.Instance.Stop();
            SoundManager.Instance.Play(_winSound);
            DOTween.Sequence()
                .SetLink(gameObject, LinkBehaviour.KillOnDisable)
                .SetUpdate(true)
                .AppendInterval(_winPhraseDelay)
                .AppendCallback(PlayWinPhrase);
            
            DOTween.Sequence()
                .SetLink(gameObject, LinkBehaviour.KillOnDestroy)
                .SetUpdate(true)
                .AppendInterval(0.5f)
                .AppendCallback(Show)
                .Insert(0.25f, _winRect.DOAnchorPosY(0, 1).SetEase(Ease.OutSine));
        }

        private void PlayWinPhrase()
        {
            SoundManager.Instance.Play(_winPhraseSound.GetRandomSound());
        }

        private void Flash(CanvasGroup canvasGroup)
        {
            canvasGroup.alpha = 0;
            canvasGroup.DOFade(1, 0.3f)
                .SetLink(gameObject, LinkBehaviour.KillOnDestroy)
                .SetUpdate(true)
                .SetEase(Ease.InSine)
                .SetLoops(-1, LoopType.Yoyo);
        }
        

        private void Show()
        {
            _rootCanvasGroup.blocksRaycasts = true;
            _rootCanvasGroup.alpha = 1;
            
            _titleGroup.transform.localScale = Vector3.one * 3;
            _menuGroup.transform.localScale = Vector3.one * 1.5f;
            _descriptionGroup.transform.localScale = Vector3.one * 3;
            
            var seq = DOTween.Sequence();
            seq.SetLink(gameObject, LinkBehaviour.KillOnDestroy);
            seq.SetUpdate(true);
            
            seq.Insert(0, DOTween.Sequence()
                .Append(_titleGroup.DOFade(1, 0.5f))
                .Join(_titleGroup.transform.DOScale(1, 0.5f).SetEase(Ease.InCubic))
                .Append(_titleGroup.GetComponent<RectTransform>().DOShakeAnchorPos(0.15f, new Vector3(15f, 0, 0))));
            
            seq.Insert(0.15f, DOTween.Sequence()
                
                .Join(_menuGroup.transform.DOScale(1, 0.5f).SetEase(Ease.InCubic))
                .Append(_menuGroup.GetComponent<RectTransform>().DOShakeAnchorPos(0.15f, new Vector3(15f, 0, 0))))
                .Insert(0.35f, _menuGroup.DOFade(1, 0.15f));
            seq.Insert(0.3f, DOTween.Sequence()
                .Append(_descriptionGroup.DOFade(1, 0.5f))
                .Join(_descriptionGroup.transform.DOScale(1, 0.5f).SetEase(Ease.InCubic))
                .Append(_descriptionGroup.GetComponent<RectTransform>().DOShakeAnchorPos(0.15f, new Vector3(15f, 0, 0))));
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