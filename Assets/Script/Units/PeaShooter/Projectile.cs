namespace Project
{
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
        private ParticleSystem _hitParticle;
        
        [SerializeField]
        private SpriteRenderer _spriteRenderer;
        
        private Team _enemyTeam;
        private float _direction;

        private DamageableRegistry _damageableRegistry;

        private bool _isDepleted = false;
        private bool _isInitialized = false;
        
        public Vector2 Position => transform.position;

        public void Init(Team enemyTeam, float direction, DamageableRegistry damageableRegistry)
        {
            _enemyTeam = enemyTeam;
            _direction = direction;
            
            _damageableRegistry = damageableRegistry;
            
            _isInitialized = true;
        }

        public void Update()
        {
            if (_isDepleted)
            {
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
            float delta = damageable.Position.x - Position.x;

            distance = Mathf.Abs(delta) + damageable.Radius * -Mathf.Sign(delta);
            
            return !damageable.IsDead 
                   && damageable.Team == _enemyTeam 
                   && distance <= _hitDistance;
        }

        private void Hit()
        {
            _isDepleted = true;
            _spriteRenderer.enabled = false;
            _hitParticle.Play();
            
            var seq = DOTween.Sequence();
            seq.SetLink(gameObject, LinkBehaviour.KillOnDestroy);
            seq.AppendInterval(2);
            seq.AppendCallback(DestroySelf);
        }
        
        private void DestroySelf()
        {
            Destroy(gameObject);
        }

        private void Move()
        {
            transform.position += Vector3.right * (_direction * _speed * Time.deltaTime);
        }

#if UNITY_EDITOR
        private void Start()
        {
            if (!_isInitialized)
            {
                Debug.LogError($"{nameof(Projectile)} not initialized", this);
            }
        }
#endif
    }
}