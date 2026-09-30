namespace Project
{
    using System;
    using DG.Tweening;
    using UnityEngine;
    using UnityEngine.EventSystems;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class MenuButton : MonoBehaviour, IPointerClickHandler
    {
        private RectTransform _rectTransform;
        private Action _clickAction;
        private Tween _scaleTween;

        private bool _isInteractable = true;
        private bool _isInitialized = false;
        
        public RectTransform Rect => _rectTransform;
        
        public void Init(Action clickAction)
        {
            _rectTransform = GetComponent<RectTransform>();
            _clickAction = clickAction;
            _isInitialized = true;
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            if (!_isInteractable || (_scaleTween != null && _scaleTween.IsPlaying()))
            {
                return;
            }
            
            _scaleTween?.Kill(true);

            _clickAction.Invoke();
            _scaleTween = transform.DOScale(0.8f, 0.075f)
                .SetLink(gameObject, LinkBehaviour.KillOnDisable)
                .SetEase(Ease.OutSine)
                .SetLoops(2, LoopType.Yoyo);
        }

        public void SetInteractable(bool isInteractable)
        {
            _isInteractable = isInteractable;
        }

#if UNITY_EDITOR
        private void Start()
        {
            if (!_isInitialized)
            {
                Debug.LogError($"{nameof(MenuButton)} not initialized", this);
            }
        }
#endif
    }
}