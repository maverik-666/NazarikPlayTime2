namespace Project
{
    using System;
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
        
        private bool _hasTarget;
        private Action _grabAction;
        private Action _consumeAction;
        private Transform _target;
        private bool _isGrabbed;
        private bool _isInitialized = false;

        public void Init()
        {
            _isInitialized = true;
        }
        
        public void StartMoveTo(Transform target, Action grabAction, Action consumeAction)
        {
            _target = target;
            _grabAction = grabAction;
            _consumeAction = consumeAction; 
            
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
                }
            }
            
            _tentacleEnd.transform.up = _source.transform.position - transform.position;
            
            _line.SetPosition(0,_source.position);
            _line.SetPosition(1, _tentacleEnd.transform.position);
        }
        
        private void MoveToSource()
        {
            _tentacleEnd.position = Vector3.Lerp(_tentacleEnd.position, _source.position, _speed * Time.deltaTime);

            if ((_source.position - _tentacleEnd.position).sqrMagnitude < 0.05f)
            {
                Consume();
            }
        }

        private void MoveToTarget()
        {
            _tentacleEnd.position = Vector3.Lerp(_tentacleEnd.position, _target.position, _speed * Time.deltaTime);

            if ((_target.position - _tentacleEnd.position).sqrMagnitude < 0.05f)
            {
                Grab();
            }
        }
        
        private void Grab()
        {
            _tentacleEnd.position = _target.position;
            _target.SetParent(_tentacleEnd.parent);
            
            _grabAction?.Invoke();
            _isGrabbed = true;
        }
        
        private void Consume()
        {
            _tentacleEnd.position = _source.position;
            _consumeAction?.Invoke();
            
            Discard();
        }
        
        private void Discard()
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
                Debug.LogError($"{nameof(Tentacle)} not initialized", this);
            }
        }
#endif
    }
}