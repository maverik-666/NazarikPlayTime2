namespace Project
{
    using System;
    using DG.Tweening;
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class LoadingScreen : MonoBehaviour
    {
        private CanvasGroup _canvasGroup;

        private Tween _fadeTween;
        private bool _isInitialized = false;

        public void Init()
        {
            _canvasGroup = GetComponentInChildren<CanvasGroup>();
            _isInitialized = true;
            
            _canvasGroup.alpha = 1;
            _canvasGroup.blocksRaycasts = true;
            Hide();
        }

        public void Show(Action callback)
        {
            _canvasGroup.blocksRaycasts = true;
            
            _fadeTween?.Kill();
            _fadeTween = _canvasGroup.DOFade(1, 0.3f)
                .SetEase(Ease.OutSine)
                .SetUpdate(true)
                .OnComplete(callback.Invoke);
        }

        public void Hide()
        {
            _canvasGroup.blocksRaycasts = false;
            
            _fadeTween?.Kill();
            _fadeTween = _canvasGroup.DOFade(0, 0.3f)
                .SetEase(Ease.OutSine);
        }

#if UNITY_EDITOR
        private void Start()
        {
            if (!_isInitialized)
            {
                Debug.LogError($"{nameof(LoadingScreen)} not initialized", this);
            }
        }
#endif
    }
}