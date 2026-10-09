namespace Project
{
    using System;
    using DG.Tweening;
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class Tentacle : MonoBehaviour
    {
        [SerializeField]
        private Transform _source;
        
        [SerializeField]
        private LineRenderer _line;
        
        [SerializeField]
        private Transform _tentacleEnd;
        
        [SerializeField]
        private float _speed;

        [SerializeField]
        private Vector3 _targetOffset;

        private SpriteRenderer _spriteRenderer;
        
        private bool _hasTarget;
        private Action _grabAction;
        private Action _consumeAction;
        private Transform _target;
        private bool _isGrabbed;
        private bool _isInitialized = false;

        public void Init()
        {
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>(true);
            _spriteRenderer.enabled = false;
            _isInitialized = true;
        }
        
        public void StartMoveTo(Transform target, Action grabAction, Action consumeAction)
        {
            _target = target;
            _grabAction = grabAction;
            _consumeAction = consumeAction; 
            
            _spriteRenderer.enabled = true;
            _tentacleEnd.position = _source.position;
            
            _hasTarget = true;
        }

        private void Update()
        {
            if (_hasTarget)
            {
                if (!_isGrabbed)
                {
                    MoveToTarget();
                }
                else
                { 
                    MoveToSource();
                    MoveTargetToSource();
                }
            } 
            else
            {
                MoveToSource();
            }
            
            if((_source.position - _tentacleEnd.position).sqrMagnitude > 0.001f)
            {
                _tentacleEnd.LookAt(_source, Vector3.up); 
                _tentacleEnd.Rotate(Vector3.up, 90, Space.Self);
            }
            
            _line.SetPosition(0,_source.position);
            _line.SetPosition(1, _tentacleEnd.transform.position);
        }

        private void MoveToSource()
        {
            _tentacleEnd.position = Vector3.Lerp(_tentacleEnd.position, _source.position, _speed * Time.deltaTime);
        }
        
        private void MoveTargetToSource()
        {
            _target.position = Vector3.Lerp(_target.position, _source.position, _speed * Time.deltaTime);
            
            if ((_source.position - _target.position).sqrMagnitude < 0.001f)
            {
                Consume();
            }
        }

        private void MoveToTarget()
        {
            var targetPosition = _target.position + _targetOffset;
            _tentacleEnd.position = Vector3.Lerp(_tentacleEnd.position, targetPosition, _speed * Time.deltaTime);

            if ((targetPosition - _tentacleEnd.position).sqrMagnitude < 0.001f)
            {
                Grab();
            }
        }
        
        private void Grab()
        {
            _tentacleEnd.position = _target.position + _targetOffset;
            
            _grabAction?.Invoke();
            _isGrabbed = true;
        }
        
        private void Consume()
        {
            _consumeAction?.Invoke();
            
            Discard();
        }
        
        public void Discard()
        {
            _hasTarget = false;
            _target = null;
            _grabAction = null;
            _consumeAction = null;
            _isGrabbed = false;
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