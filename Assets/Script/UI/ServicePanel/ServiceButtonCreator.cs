namespace Project
{
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class ServiceButtonCreator : MonoBehaviour
    {
        private IService[] _services;
        
        private ServiceButtonPanel _serviceButtonPanel = null;
        private bool _isInitialized = false;

        public void Init(IService[] services, ServiceButtonPanel serviceButtonPanel, Progress progress)
        { 
            _services = services;
            _serviceButtonPanel = serviceButtonPanel;
            
            foreach (var service in _services)
            {
                if (progress.IsUnlocked(service.Unlockable))
                {
                    CreateButtonFor(service);
                }
            }
            
            _isInitialized = true;
        }

        private void CreateButtonFor(IService service)
        {
            var button = Instantiate(service.ServiceButtonPf, _serviceButtonPanel.NextSlot);
            button.Init(service);
            _serviceButtonPanel.AddButton(button);
        }

#if UNITY_EDITOR
        private void Start()
        {
            if (!_isInitialized)
            {
                Debug.LogError($"{nameof(ServiceButtonCreator)} not initialized", this);
            }
        }
#endif
    }
}