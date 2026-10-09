namespace Project
{
    using System;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Image))]
    public class Clickable : MonoBehaviour, IPointerDownHandler
    {
        private Image _image;
        private Action _clickAction;
        private bool _isInitialized = false;

        
        public void Init(Action clickAction)
        {
            _clickAction = clickAction;
            _image = GetComponent<Image>();
            _isInitialized = true;
        }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            _clickAction?.Invoke();
        }

        public void SetInteractable(bool value)
        {
            _image.raycastTarget = value;
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