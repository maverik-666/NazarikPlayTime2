namespace Project
{
    using System;
    using DG.Tweening;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class AccelerationButton : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField]
        private AccelerationMode[] _modes;
        
        [Header("Sounds")]
        [SerializeField]
        private SingleSoundData _clickSound;
        
        private Tween _scaleTween;

        private int _currentMode = 0;
        private int _clickCounter = 1;
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
            _currentMode = _clickCounter++ % 3;
            
            ApplyState();
           
            SoundManager.Instance.Play(_clickSound);
            
            _scaleTween?.Kill(true);
            _scaleTween = transform.DOScale(0.8f, 0.15f)
                .SetLink(gameObject, LinkBehaviour.KillOnDisable)
                .SetUpdate(true)
                .SetEase(Ease.OutSine)
                .SetLoops(2, LoopType.Yoyo);
        }

        private void ApplyState()
        {
            for (int i = 0; i < _modes.Length; i++)
            {
                var mode = _modes[i];

                if (i == _currentMode)
                { 
                    mode.Image.gameObject.SetActive(true);
                    _timeSpeedHandler.SetSpeed(mode.Acceleration);
                }
                else
                { 
                    mode.Image.gameObject.SetActive(false);
                }
            }
        }

        [Serializable]
        private class AccelerationMode
        {
            [SerializeField]
            private Image _image;

            [SerializeField]
            private float _acceleration = 1;

            public Image Image => _image;

            public float Acceleration => _acceleration;
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