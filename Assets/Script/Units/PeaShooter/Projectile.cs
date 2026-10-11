namespace Project
{
    using System;
    using DG.Tweening;
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class Projectile : MonoBehaviour
    {
        [SerializeField]
        private float _speed = 1;

        [SerializeField]
        private int _attack;

        [SerializeField]
        private float _hitDistance = 0.1f;

        [SerializeField]
        private float _maxTravelDistance = 30f;

        [SerializeField]
        private ParticleSystem _hitParticle;

        [SerializeField]
        private SpriteRenderer _spriteRenderer;

        [Header("Sounds")]
        [SerializeField]
        private RandomSoundData _hitSound;

        private Team _enemyTeam;
        private float _direction;
        private float _traveledDistance;
        private Action<Projectile> _returnToPool;
        
        private DamageableRegistry _damageableRegistry;
        private Vector2 _lastPosition;
        
        private bool _isDepleted = false;
        private bool _isInitialized = false;

        public Vector2 Position => transform.position;
        

        public void Init( DamageableRegistry damageableRegistry, Action<Projectile> returnToPool)
        {
            _damageableRegistry = damageableRegistry;
            _returnToPool = returnToPool;
            
            _isInitialized = true;
        }

        public void StartFly(Team enemyTeam, float direction)
        {
            _enemyTeam = enemyTeam;
            _direction = direction;
            
            _isDepleted = false;
            _traveledDistance = 0f;
            _spriteRenderer.enabled = true;

            _lastPosition = Position;
        }

        public void Update()
        {
            if (_isDepleted)
            {
                return;
            }

            if (_traveledDistance >= _maxTravelDistance)
            {
                _returnToPool(this);
                return;
            }

            if (TryFindTarget(out var target))
            {
                target.TakeDamage(_attack);
                Hit();
            }
            else
            {
                Move();
            }
        }
        
        private bool TryFindTarget(out IDamageable target)
        {
            target = null;
            bool foundAny = false;
            float closestDist = float.MaxValue;
            
            foreach (var damageable in _damageableRegistry.Items)
            {
                if(IsAttackable(damageable, out float distance))
                    if (closestDist > distance)
                    {
                        target = damageable;
                        foundAny = true;
                        closestDist = distance;
                    }
            }
            
            return foundAny;
        }

        private bool IsAttackable(IDamageable damageable, out float distance)
        {
            float damageablePosX = damageable.Position.x;
            float delta = damageablePosX - _lastPosition.x;
            float deltaCurrent = damageablePosX - Position.x;
            
            bool isFlyThrough = delta * deltaCurrent < 0;
            
            distance = Mathf.Abs(delta) + damageable.Radius * -Mathf.Sign(delta);
            
            return !damageable.IsDead 
                   && damageable.Team == _enemyTeam 
                   && (distance <= _hitDistance || isFlyThrough);
        }

        private void Hit()
        {
            _isDepleted = true;
            _spriteRenderer.enabled = false;
            _hitParticle.Play();

            SoundManager.Instance.Play(_hitSound.GetRandomSound());

            var seq = DOTween.Sequence();
            seq.SetLink(gameObject, LinkBehaviour.KillOnDisable);
            seq.AppendInterval(2);
            seq.AppendCallback(() => _returnToPool(this));
        }

        private void Move()
        {
            _lastPosition = Position;
            
            float delta = _direction * _speed * Time.deltaTime;
            transform.position += Vector3.right * delta;
            _traveledDistance += Mathf.Abs(delta);
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