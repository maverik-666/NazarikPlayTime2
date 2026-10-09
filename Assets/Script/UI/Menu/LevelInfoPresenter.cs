namespace Project
{
    using DG.Tweening;
    using TMPro;
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class LevelInfoPresenter : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _title;
        
        [SerializeField]
        private TMP_Text _description;

        [SerializeField]
        private CanvasGroup _canvasGroup;

        [SerializeField]
        private MenuButton _playLevel;
        
        [Header("Sounds")]
        [SerializeField]
        private SingleSoundData _playSound;
        
        [Header("Crash")]
        [TextArea]
        [SerializeField]
        private string _crashTitle;

        [TextArea]
        [SerializeField]
        private string _crashDescription; 
        
        private LevelLoader _levelLoader;
        private LevelButton _selected;
        
        private Sequence _fadeSeq;

        private bool _isInitialized = false;

        public void Init(LevelLoader levelLoader)
        {
            _levelLoader = levelLoader;
            
            _canvasGroup.alpha = 0;
            _canvasGroup.blocksRaycasts = false;
            
            _playLevel.Init(OnPlayClick);
            _playLevel.SetInteractable(false);
            
            _isInitialized = true;
        }

        public void SelectLevel(LevelButton levelButton)
        {
            if (_selected == levelButton)
            {
                return;
            }
            
            _selected = levelButton;
            Show();
        }

        private void Show()
        {
            _canvasGroup.blocksRaycasts = true;
            
            _playLevel.SetInteractable(true);
            _fadeSeq?.Kill();
            _fadeSeq = DOTween.Sequence();
            _fadeSeq.SetLink(gameObject, LinkBehaviour.KillOnDestroy);
            _fadeSeq.Append(_canvasGroup.DOFade(0, 0.3f));
            _fadeSeq.AppendCallback(SetSelectedInfo);
            _fadeSeq.Append(_canvasGroup.DOFade(1, 0.3f));
        }

        private void SetSelectedInfo()
        {
            _title.SetText(_selected.Title);
            _description.SetText(_selected.Description);
        }
        
        public void Hide()
        {
            _selected = null;
            _canvasGroup.blocksRaycasts = false;
            _playLevel.SetInteractable(false);
            
            _fadeSeq?.Kill();
            _fadeSeq = DOTween.Sequence();
            _fadeSeq.SetLink(gameObject, LinkBehaviour.KillOnDestroy);
            _fadeSeq.Append(_canvasGroup.DOFade(0, 0.3f));
        }

        private void OnPlayClick()
        {
            if (_selected.Level == 4 || _selected.Level == 5)
            {
                SoundManager.Instance.Play(_playSound);
                Crash();
            }
            else
            {
                MusicManager.Instance.Stop();
                SoundManager.Instance.Play(_playSound);
                _levelLoader.LoadLevel(_selected.Level);
            }
        }

        private void Crash()
        {
            _title.SetText(_crashTitle);
            _description.SetText(_crashDescription);

            DOTween.KillAll();
            DOTween.Sequence()
                .AppendInterval(0.05f)
                .AppendCallback(Application.Quit);
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