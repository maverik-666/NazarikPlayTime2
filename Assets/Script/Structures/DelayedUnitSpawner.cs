namespace Project
{
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class DelayedUnitSpawner : MonoBehaviour
    {
        [SerializeField]
        private float _startDelay = 10f;

        [SerializeField]
        private Vector2 _yOffsetRange = new Vector2(-0.25f, 0.25f);

        private IUnitFactory _unitFactory;

        private Team _team = Team.Blue;

        private Team _enemyTeam = Team.Blue;

        private float _direction = 1;

        private Timer _timer;

        private bool _hasSpawned = false;
        private bool _isInitialized = false;

        public void Init(Team team, Team enemyTeam, float direction, IUnitFactory unitFactory)
        {
            _team = team;
            _enemyTeam = enemyTeam;
            _direction = direction;

            _unitFactory = unitFactory;
            
            _timer = new Timer();
            _timer.Start(_startDelay);
            
            _isInitialized = true;
        }

        private void Update()
        {
            _timer.Tick(Time.deltaTime);
            
            if (_timer.IsStopped && !_hasSpawned)
            {
                _hasSpawned = true; 
                Spawn(); 
            }
        }

        private void Spawn()
        {
            var pos = transform.position + Vector3.up * Random.Range(_yOffsetRange.x, _yOffsetRange.y);
            _unitFactory.Create(pos, _team, _enemyTeam, _direction);
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
            Gizmos.DrawLine(transform.position + Vector3.up * _yOffsetRange.x,
                transform.position + Vector3.up * _yOffsetRange.y);
        }
#endif
    }
}