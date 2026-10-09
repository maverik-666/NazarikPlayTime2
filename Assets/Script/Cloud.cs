namespace Project
{
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class Cloud : MonoBehaviour
    {
        [SerializeField]
        private float _speed = 0.2f;

        private Transform _end;
        
        private Transform _start; 
        
        private bool _isInitialized = false;

        public void Init(Transform start, Transform end)
        {
            _start = start;
            _end = end;
            
            _isInitialized = true;
        }

        private void Update()
        {
            transform.position += Vector3.left * (_speed * Time.deltaTime);

            if (transform.position.x < _end.position.x)
            {
                var pos = transform.position;
                pos.x = _start.position.x;
                transform.position = pos;
            }
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