namespace Project
{
    using UnityEngine;
    using UnityEngine.EventSystems;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class PanelHoverZone : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private ServiceButtonPanel _serviceButtonPanel;
        private bool _isInitialized = false;

        public void Init(ServiceButtonPanel serviceButtonPanel)
        {
            _serviceButtonPanel = serviceButtonPanel;
            _isInitialized = true;
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            _serviceButtonPanel.Show();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _serviceButtonPanel.Hide();
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