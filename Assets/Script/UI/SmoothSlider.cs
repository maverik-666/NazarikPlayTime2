using UnityEngine;
using UnityEngine.UI;

namespace Project
{

    [RequireComponent(typeof(Slider))]
    public class SmoothSlider : MonoBehaviour
    {
        [SerializeField] 
        private float _lerp = 0.1f;
        [SerializeField] 
        private float _clamp = 0.001f;
     
        [SerializeField] 
        private bool _doMoveTowards;
        
        private float _currentPosition;

        private Slider _slider;
        
        private void Awake()
        {
            _slider = GetComponent<Slider>();
        }

        public void SetPosition(float position)
        {
            _currentPosition = position;
            enabled = true;
        }

        public void SetForcedPosition(float position)
        {
             _slider.value = position;
            _currentPosition = position;
            enabled = false;
        }

        private void Update()
        {
            if (_doMoveTowards)
            {
                _slider.value = Mathf.MoveTowards( _slider.value, _currentPosition, _lerp * Time.unscaledDeltaTime);
            }
            else
            {
                _slider.value = Mathf.Lerp( _slider.value, _currentPosition, _lerp);
            }

            if (Mathf.Abs( _slider.value - _currentPosition) < _clamp)
            {
                _slider.value = _currentPosition;
                enabled = false;
            }
        }

    }
}