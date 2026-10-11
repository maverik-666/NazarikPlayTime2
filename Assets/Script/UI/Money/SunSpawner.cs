namespace Project
{
    using System.Collections.Generic;
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class SunSpawner : MonoBehaviour
    {
        [SerializeField]
        private Sun _sunPf;

        [SerializeField]
        private Vector2 _fallHeightRange;

        [SerializeField]
        private int _moneyPerSun = 5;
        
        [SerializeField]
        private Vector2 _spawnIntervalRange = new Vector2(10, 15);
        
        [SerializeField]
        private Vector2 _spawnBoxExtents = new Vector2(6, 2);

        [Header("Editor")]
        [SerializeField]
        private bool _autoPickup = true;
        
        private Camera _camera;
        private MoneyCounter _moneyCounter;
        private MoneyCounterView _moneyCounterView;

        private Timer _timer;
        private readonly Queue<Sun> _pool = new Queue<Sun>();
        private bool _isInitialized = false;

        public void Init(Camera camera, MoneyCounter moneyCounter, MoneyCounterView moneyCounterView)
        {
            _camera = camera;
            _moneyCounter = moneyCounter;
            _moneyCounterView = moneyCounterView;
            
            PrepareSuns();
            
            _timer = new Timer();
            _isInitialized = true;
        }


        private void PrepareSuns()
        {
            for (int i = 0; i < 5; i++)
            {
                var sun = CreateSun();
                sun.gameObject.SetActive(false);
                _pool.Enqueue(sun);
            }
        }

        private Sun CreateSun()
        {
            var sun = Instantiate(_sunPf, transform); 
            sun.Init(_camera, OnPickup, ReturnToPool);
            return sun;
        }

        public void Update()
        {
            _timer.Tick(Time.deltaTime);
            
            if (_timer.IsStopped)
            {
                SpawnSun();
                _timer.Start(Random.Range(_spawnIntervalRange.x, _spawnIntervalRange.y));
            }
        }

        private void SpawnSun()
        {
            var offset = Random.insideUnitSphere;
            offset.Scale(_spawnBoxExtents);
            var position = transform.position + offset;
            float fallHeight = Random.Range(_fallHeightRange.x, _fallHeightRange.y);

            Sun sun;
            if (_pool.Count > 0)
            {
                sun = _pool.Dequeue();
            }
            else
            {
                sun = CreateSun();
            }
            
            sun.StartFlyFrom(position, fallHeight);
            sun.gameObject.SetActive(true);

#if UNITY_EDITOR
            if (_autoPickup)
            {
                sun.OnClick();
            }
#endif
        }

        private void OnPickup(Sun sun)
        {
            _moneyCounter.Add(_moneyPerSun);
            sun.MoveTo(_moneyCounterView.IconCenter);
        }

        private void ReturnToPool(Sun sun)
        {
            sun.gameObject.SetActive(false);
            _pool.Enqueue(sun);
        }

#if UNITY_EDITOR
        private void Start()
        {
            if (!_isInitialized)
            {
                Debug.LogError($"{GetType().Name} not initialized", this);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.position, _spawnBoxExtents * 2);
        }
#endif
    }
}