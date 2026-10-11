namespace Project
{
    using DG.Tweening;
    using LudicWorlds;
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class Peashooter : MonoBehaviour, IIdler, IAttacker, IUnit
    {
        [SerializeField]
        private int _attack = 15;
        
        [SerializeField]
        private float _attackCooldown = 5;
        
        [SerializeField]
        private float _visionDistance = 1;
        
        [SerializeField]
        private int _heathPoints = 50;

        [SerializeField]
        private int[] _knockPoints;
        
        [SerializeField]
        private DamageableProperty _properties = DamageableProperty.Normal;

        private Health _health;
        private Team _team;
        private Team _enemyTeam;
        private float _direction = 1;
        
        private StateMachine _stateMachine;
        private PeashooterSkinRenderer _skinRenderer;

        private bool _canNotTakeDamage = false;
        private bool _isInitialized = false;

        public Vector2 Position => transform.position;
        public float Radius => 0;
        public Health Health => _health;
        
        public Team Team => _team;
        public Team EnemyTeam => _enemyTeam;
        public DamageableType Type => DamageableType.Unit; 
        public DamageableProperty Properties => _properties;

        public bool CanNotTakeDamage => _canNotTakeDamage;
        
        public int Attack => _attack;

        public float AttackCooldown => _attackCooldown;

        public float VisionDistance => _visionDistance;

        public float Direction => _direction;
        

        public void Init(Team team, Team enemyTeam, float direction, DamageableRegistry damageableRegistry, ProjectileFactory projectileFactory)
        {
            _team = team;
            _enemyTeam = enemyTeam;
            SetDirection(direction);

            _health = new Health(_heathPoints);
            
            _stateMachine = GetComponentInChildren<StateMachine>();
            
            var attackState = GetComponentInChildren<ShootAttackState>();
            attackState.Init("Attack", this, 0.2f, "Idle", projectileFactory, _stateMachine);
            _stateMachine.AddState(attackState);
            
            var prepareAttack = GetComponentInChildren<WaitForAttackState>();
            prepareAttack.Init("PrepareAttack", attackState, _stateMachine);
            _stateMachine.AddState(prepareAttack);
            
            var idleState = GetComponentInChildren<StationaryIdleState>();
            idleState.Init("Idle", this, damageableRegistry, prepareAttack, DamageableProperty.Normal, _stateMachine);
            _stateMachine.AddState(idleState);
            
            var takeDamageState = GetComponentInChildren<TakeDamageState>();
            takeDamageState.Init("TakeDamage", this, "Die", "Idle", _stateMachine);
            _stateMachine.AddState(takeDamageState);

            var dieState = GetComponentInChildren<DieState>();
            dieState.Init("Die", this, damageableRegistry, _stateMachine);
            _stateMachine.AddState(dieState);

            _skinRenderer = GetComponentInChildren<PeashooterSkinRenderer>();
            _skinRenderer.Init(this, _stateMachine);
            
            _stateMachine.SetState("Idle");
            
            _isInitialized = true;
        }
        
        public void SetInvincible(bool value)
        {
            _canNotTakeDamage = value;
        }

        public void TakeDamage(int damage)
        {
            if (CanNotTakeDamage)
            {
                return;
            }

            int lastHealth = Health.Current;
            
            Health.TakeDamage(damage);
            _skinRenderer.FlashRed();

            if (Health.IsDead)
            {
                _stateMachine.SetState("TakeDamage");
                return;
            }
            
            foreach (int knockPoint in _knockPoints)
            {
                if (lastHealth > knockPoint && Health.Current <= knockPoint)
                { 
                    _stateMachine.SetState("TakeDamage");
                    break;
                }
            }
        }
        
        public void SetDirection(float direction)
        {
            _direction = Mathf.Sign(direction);
            transform.localScale = new Vector3(_direction, 1, 1);
        }
        
        public void InstantDeath() 
        { 
            _health.SetCurrentHealth(0); 
            _stateMachine.SetState("Die");
        }

        public void PickedUp()
        {
            _stateMachine.enabled = false;
            _stateMachine.SetState("Idle");
        }

        public void Dispose()
        {
            var seq = DOTween.Sequence();
            seq.SetLink(gameObject, LinkBehaviour.KillOnDestroy);
            seq.AppendInterval(2);
            seq.AppendCallback(DestroySelf);
        }

        private void DestroySelf()
        {
            Destroy(gameObject);
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
            Gizmos.color = Color.red;
            
            Gizmos.DrawLine(transform.position, transform.position + Vector3.right * (_direction * _visionDistance));
        }
#endif
    }
}