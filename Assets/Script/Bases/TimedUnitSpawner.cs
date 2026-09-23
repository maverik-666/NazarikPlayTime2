namespace Project
{
    using System;
    using DG.Tweening;
    using UnityEngine;
    using Random = UnityEngine.Random;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class TimedUnitSpawner : MonoBehaviour
    {
        [SerializeField]
        private Team _team = Team.Blue;

        [SerializeField]
        private Team _enemyTeam = Team.Blue;
        
        [SerializeField]
        private float _direction = 1;
        
        [SerializeField]
        private float _startDelay = 10f;

        [SerializeField]
        private float _interval = 10f;

        [SerializeField]
        private float _spawnCount = 1;

        [SerializeField]
        private float _singleInterval = 0.5f;

        [SerializeField]
        private Vector2 _yOffsetRange = new Vector2(-0.25f, 0.25f);

        private float _currentCooldown;
        
        private UnitFactory _unitFactory;
        private bool _isInitialized = false;

        public void Init(UnitFactory unitFactory)
        {
            _unitFactory = unitFactory;
            _currentCooldown = _startDelay;
            _isInitialized = true;
        }

        private void Update()
        {
            _currentCooldown -= Time.deltaTime;

            if (_currentCooldown <= 0f)
            {
                Spawn();
                _currentCooldown = _interval;
            }
        }

        private void Spawn()
        {
            var seq = DOTween.Sequence();
            seq.SetLink(gameObject, LinkBehaviour.KillOnDisable);
            
            for (int i = 0; i < _spawnCount; i++)
            {
                seq.AppendCallback(SpawnSingle); 
                seq.AppendInterval(_singleInterval);
            }
        }

        private void SpawnSingle()
        {
            var pos = transform.position + Vector3.up * Random.Range(_yOffsetRange.x, _yOffsetRange.y);
            _unitFactory.Create(pos, _team, _enemyTeam, _direction);
        }

#if UNITY_EDITOR
        private void Start()
        {
            if (!_isInitialized)
            {
                Debug.LogError($"{nameof(TimedUnitSpawner)} not initialized", this);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position + Vector3.up * _yOffsetRange.x, transform.position + Vector3.up * _yOffsetRange.y);
        }
#endif
    }
}