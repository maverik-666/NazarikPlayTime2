namespace Project
{
    using LudicWorlds;
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class Cat : MonoBehaviour, IIdler, IMover, IAttacker, IUnit
    {
        [SerializeField]
        private float _moveSpeed = 1;
        
        [SerializeField]
        private int _attack = 15;

        [SerializeField]
        private float _attackCooldown = 5;

        [SerializeField]
        private float _visionDistance = 1;
        
        [SerializeField]
        private int _heathPoints = 50;

        [SerializeField]
        private int[] _knockPoints = { 0 };
        
        [SerializeField]
        private DamageableProperty _properties = DamageableProperty.None;
        
        private Health _health;
        private Team _team;
        private Team _enemyTeam;
        private float _direction = 1;

        private CatSkinRenderer _skinRenderer;
        private StateMachine _stateMachine;

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

        public float MoveSpeed => _moveSpeed;

        public int Attack => _attack;

        public float AttackCooldown => _attackCooldown;

        public float VisionDistance => _visionDistance;

        public float Direction => _direction;
        

        public void Init(Team team, Team enemyTeam, float direction, DamageableRegistry damageableRegistry)
        {
            _team = team;
            _enemyTeam = enemyTeam;
            SetDirection(direction);
            
            _health = new Health(_heathPoints);
            
            _stateMachine = GetComponentInChildren<StateMachine>();

            var attackState = GetComponentInChildren<SingleAttackState>();
            attackState.Init("Attack", this, 0.15f, _visionDistance, "Idle", damageableRegistry, DamageableProperty.None, _stateMachine);
            _stateMachine.AddState(attackState);

            var idleState = GetComponentInChildren<IdleState>();
            idleState.Init("Idle", this, damageableRegistry, attackState, "Move", DamageableProperty.None, _stateMachine);
            _stateMachine.AddState(idleState);

            var moveState = GetComponentInChildren<MoveForwardState>();
            moveState.Init("Move", this, damageableRegistry, attackState, "Idle", DamageableProperty.None, _stateMachine);
            _stateMachine.AddState(moveState);

            var takeDamageState = GetComponentInChildren<TakeDamageState>();
            takeDamageState.Init("TakeDamage", this, "Die", "Idle", _stateMachine);
            _stateMachine.AddState(takeDamageState);

            var dieState = GetComponentInChildren<DieState>();
            dieState.Init("Die", this, damageableRegistry, _stateMachine);
            _stateMachine.AddState(dieState);
            
            _skinRenderer = GetComponentInChildren<CatSkinRenderer>();
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
            _stateMachine.SetState("Die");
        }

        public void PickedUp()
        {
            _canNotTakeDamage = true;
            _stateMachine.enabled = false;
            _stateMachine.SetState("Idle");
        }

        public void Dispose()
        {
            Destroy(gameObject);
        }
        
#if UNITY_EDITOR
        private void Start()
        {
            if (!_isInitialized)
            {
                Debug.LogError($"{nameof(Cat)} not initialized", this);
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