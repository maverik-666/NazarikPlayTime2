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
            _levelLoader.LoadLevel(_selected.Level);
        }
        

#if UNITY_EDITOR
        private void Start()
        {
            if (!_isInitialized)
            {
                Debug.LogError($"{nameof(LevelInfoPresenter)} not initialized", this);
            }
        }
#endif
    }
}