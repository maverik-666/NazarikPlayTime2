namespace Project
{
    using DG.Tweening;
    using LudicWorlds;
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class Golem : MonoBehaviour, IIdler, IMover, IAttacker, IUnit
    {
        [SerializeField]
        private float _moveSpeed = 1;
        
        [SerializeField]
        private int _attack = 15;
        
        [SerializeField]
        private float _attackCooldown = 5;

        [SerializeField]
        private float _attackPrepareDuration = 5;
        
        [SerializeField]
        private float _attackDistance = 2;
        
        [SerializeField]
        private float _visionDistance = 1;
        
        [SerializeField]
        private int _heathPoints = 50;

        [SerializeField]
        private int[] _knockPoints = { 0 };
        
        private Health _health;
        private Team _team;
        private Team _enemyTeam;
        private float _direction = 1;
        
        private StateMachine _stateMachine;

        private bool _canNotTakeDamage = false;
        private bool _isInitialized = false;

        public Vector2 Position => transform.position;
        public Health Health => _health;
        
        public Team Team => _team;
        public Team EnemyTeam => _enemyTeam;

        public bool CanNotTakeDamage => _canNotTakeDamage;

        public float MoveSpeed => _moveSpeed;

        public int Attack => _attack;

        public float AttackCooldown => _attackCooldown;

        public float VisionDistance => _visionDistance;

        public float Direction => _direction;
        

        public void Init(Team team, Team enemyTeam, float direction, DamageableRegistry damageableRegistry, GolemRemainsFactory golemRemainsFactory)
        {
            _team = team;
            _enemyTeam = enemyTeam;
            SetDirection(direction);

            _health = new Health(_heathPoints);
            
            _stateMachine = GetComponentInChildren<StateMachine>();

            var golemDieState = GetComponentInChildren<GolemDieState>();
            golemDieState.Init("GolemDie", this, damageableRegistry, golemRemainsFactory, _stateMachine);
            _stateMachine.AddState(golemDieState);
            
            var attackState = GetComponentInChildren<GolemAttackState>();
            attackState.Init("Attack", this, this, 0.5f, _attackDistance, "GolemDie", damageableRegistry, _stateMachine);
            _stateMachine.AddState(attackState);
            
            var prepareAttack = GetComponentInChildren<PrepareAttackState>();
            prepareAttack.Init("PrepareAttack", _attackPrepareDuration, attackState, _stateMachine);
            _stateMachine.AddState(prepareAttack);
            
            var idleState = GetComponentInChildren<IdleState>();
            idleState.Init("Idle", this, damageableRegistry, prepareAttack, "Move", _stateMachine);
            _stateMachine.AddState(idleState);
            
            var moveState = GetComponentInChildren<MoveForwardState>();
            moveState.Init("Move", this, damageableRegistry, prepareAttack, "Idle", _stateMachine);
            _stateMachine.AddState(moveState);

            var takeDamageState = GetComponentInChildren<TakeDamageState>();
            takeDamageState.Init("TakeDamage", this, "Die", "Idle", _stateMachine);
            _stateMachine.AddState(takeDamageState);

            var dieState = GetComponentInChildren<DieState>();
            dieState.Init("Die", this, damageableRegistry, _stateMachine);
            _stateMachine.AddState(dieState);
            
            GetComponentInChildren<GolemSkinRenderer>().Init(this, _stateMachine);
            
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
                Debug.LogError($"{nameof(Golem)} not initialized", this);
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