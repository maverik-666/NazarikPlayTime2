namespace Project
{
    using System;
    using DG.Tweening;
    using UnityEngine;

    public class Sun : MonoBehaviour
    {
        [SerializeField] 
        private Transform _aura;

        [SerializeField]
        private CanvasGroup _canvasGroup;
        
        [SerializeField]
        private float _rotationSpeed = 20;

        [SerializeField]
        private float _fallSpeed = 1;
        
        [SerializeField]
        private float _moveToTargetSpeed = 1;

        [SerializeField]
        private SingleSoundData _clickSound;
        
        private float _initialYPosition = 0;
        private float _fallHeight = 0;
        private State _state;
        
        private Tween _fadeTween = null;
        private bool _isShown = false;

        private Camera _camera;
        private Transform _uiTarget;
        private float _initialDistance;
        
        private Action<Sun> _clickAction = null;
        private Clickable _clickable = null;
        
        private bool _isInitialized = false;

        private enum State
        {
            Falling, MovingToTarget, None
        }
        
        public void Init(Camera camera, Action<Sun> clickAction, float fallHeight)
        { 
            _clickAction = clickAction;
            
            _initialYPosition = transform.position.y;
            _fallHeight = fallHeight;

            _camera = camera;
            GetComponentInChildren<Canvas>().worldCamera = camera;
            _clickable = GetComponentInChildren<Clickable>(true);
            _clickable.Init(OnClick);

            _canvasGroup.alpha = 0;
            
            Show();

            _state = State.Falling;
            _isInitialized = true;
        }

        public void OnClick()
        {
            _clickable.SetInteractable(false);
            _state = State.None;
            
            _fadeTween?.Kill();
            _canvasGroup.alpha = 1;
            
            SoundManager.Instance.Play(_clickSound);
            
            _clickAction?.Invoke(this);
        }
        
        private void Update()
        {
            _aura.Rotate(Vector3.forward, _rotationSpeed * Time.deltaTime);

            if (_state == State.Falling)
            {
               Fall();
            }
            else if (_state == State.MovingToTarget)
            { 
                MoveToTarget();
            }
        }

        private void Fall()
        {
            transform.Translate(Vector3.down * (_fallSpeed * Time.deltaTime));
            
            if (_isShown && _initialYPosition - transform.position.y > _fallHeight)
            {
                Hide();
            }
        }

        private void MoveToTarget()
        {
            var targetPosition = CalcWorldPosition(_uiTarget.position);
            float distance = Vector3.Distance(transform.position, targetPosition);

            if (distance < 0.1f)
            {
                _state = State.None;
                _canvasGroup.DOFade(0,0.15f)
                    .SetLink(gameObject, LinkBehaviour.KillOnDisable)
                    .OnComplete(Dispose);
                
                return;
            }
            
            var direction = (targetPosition - transform.position).normalized;
            float t = Mathf.InverseLerp(-1, _initialDistance, distance);

            transform.Translate(direction * (_moveToTargetSpeed * Time.deltaTime * t));
        }

        private Vector3 CalcWorldPosition(Vector3 screenPos)
        {
            screenPos.z = -_camera.transform.position.z;
            return _camera.ScreenToWorldPoint(screenPos);
        }

        private void Show()
        {
            _isShown = true;
            _clickable.SetInteractable(true);
            _fadeTween = _canvasGroup.DOFade(1, 0.5f)
                .SetLink(gameObject, LinkBehaviour.KillOnDisable);
        }

        public void Hide()
        {
            _isShown = false; 
            _fadeTween = _canvasGroup.DOFade(0, 0.5f)
                .SetLink(gameObject, LinkBehaviour.KillOnDisable)
                .OnComplete(Dispose);
        }

        public void MoveTo(Transform uiTarget)
        {
            _state = State.MovingToTarget;
            _uiTarget = uiTarget;
            
            var targetPosition = CalcWorldPosition(_uiTarget.position);
            _initialDistance = Vector3.Distance(transform.position, targetPosition);
        }

        private void Dispose()
        {
            _clickable.SetInteractable(false);
            Destroy(gameObject);
        }
        
#if UNITY_EDITOR
        private void Start()
        {
            if (!_isInitialized)
            {
                Debug.LogError($"{nameof(Sun)} not initialized", this);
            }
        }
#endif
    }
}