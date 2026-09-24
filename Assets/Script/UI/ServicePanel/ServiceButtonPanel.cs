namespace Project
{
    using System.Collections.Generic;
    using DG.Tweening;
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class ServiceButtonPanel : MonoBehaviour
    {
        [Header("Slide animation")]
        [SerializeField]
        private RectTransform _panel;

        [SerializeField]
        private RectTransform _shownPoint;
        
        [SerializeField]
        private RectTransform _hiddenPoint;

        [SerializeField]
        private float _slideDuration = 0.5f;
        
        [Header("Slots")]
        [SerializeField]
        private ButtonSlot[] _buttonSlots;
        
        private List<ServiceButton> _buttons;
        private MoneyCounter _moneyCounter;
        
        private PanelHoverZone _panelHoverZone; 
        private Tween _slideTween;
        private bool _isInitialized = false;
        
        public Transform NextSlot => _buttonSlots[_buttons.Count].transform;

        public void Init()
        {
            _panelHoverZone = GetComponentInChildren<PanelHoverZone>(true);
            _panelHoverZone.Init(this);
            
            _buttons = new List<ServiceButton>();
            
            _isInitialized = true;
        }

        public void AddButton(ServiceButton serviceButton)
        {
            serviceButton.transform.SetParent(_buttonSlots[_buttons.Count].transform);
            serviceButton.transform.localPosition = Vector3.zero;
            
            _buttons.Add(serviceButton);
        }

        public void Show()
        {
            _slideTween?.Kill();
            _slideTween = _panel.DOAnchorPosY(_shownPoint.anchoredPosition.y, _slideDuration)
                .SetLink(gameObject, LinkBehaviour.KillOnDisable)
                .SetEase(Ease.OutBack);
        }

        public void Hide()
        {
            _slideTween?.Kill();
            _slideTween = _panel.DOAnchorPosY(_hiddenPoint.anchoredPosition.y, _slideDuration)
                .SetLink(gameObject, LinkBehaviour.KillOnDisable)
                .SetEase(Ease.OutBack);       
        }

#if UNITY_EDITOR
        private void Start()
        {
            if (!_isInitialized)
            {
                Debug.LogError($"{nameof(ServiceButtonPanel)} not initialized", this);
            }
        }
#endif
    }
}