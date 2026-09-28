namespace Project
{
    using DG.Tweening;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class AccelerationButton : MonoBehaviour, IPointerClickHandler
    {
        private const int AcceleratedSpeed = 2;
        
        [SerializeField]
        private Image _x1Image;
        
        [SerializeField]
        private Image _x2Image;
        
        private Tween _scaleTween;

        private bool _isAccelerated = false;
        private TimeSpeedHandler _timeSpeedHandler;
        private bool _isInitialized = false;

        public void Init(TimeSpeedHandler timeSpeedHandler)
        {
            _timeSpeedHandler = timeSpeedHandler;
            
            ApplyState();
            _isInitialized = true;
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            _isAccelerated = !_isAccelerated;
            ApplyState();
            
            _scaleTween?.Kill(true);
            _scaleTween = transform.DOScale(0.8f, 0.15f)
                .SetLink(gameObject, LinkBehaviour.KillOnDisable)
                .SetUpdate(true)
                .SetEase(Ease.OutSine)
                .SetLoops(2, LoopType.Yoyo);
        }

        private void ApplyState()
        {
            if (_isAccelerated)
            {
                _x1Image.gameObject.SetActive(false);
                _x2Image.gameObject.SetActive(true);
                _timeSpeedHandler.SetSpeed(AcceleratedSpeed);
            }
            else
            {
                _x1Image.gameObject.SetActive(true);
                _x2Image.gameObject.SetActive(false);
                _timeSpeedHandler.SetSpeed(1);
            }
        }

#if UNITY_EDITOR
        private void Start()
        {
            if (!_isInitialized)
            {
                Debug.LogError($"{nameof(AccelerationButton)} not initialized", this);
            }
        }
#endif
    }
}