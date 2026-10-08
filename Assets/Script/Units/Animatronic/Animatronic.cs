namespace Project
{
    using DG.Tweening;
    using LudicWorlds;
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class Animatronic : MonoBehaviour, IIdler, IUnit
    {
        [SerializeField]
        private float _waitDuration = 30;
        
        [SerializeField]
        private int _heathPoints = 50;

        [SerializeField]
        private float _summonDuration;
        
        [SerializeField]
        private DamageableProperty _properties = DamageableProperty.None;
        
        private Health _health;
        private Team _team;
        private Team _enemyTeam;
        private float _direction = 1;
        
        private StateMachine _stateMachine;
        private AnimatronicSkinRenderer _skinRenderer;

        private bool _isDepleted = false;
        private bool _canNotTakeDamage = false;
        private bool _isInitialized = false;

        public Vector2 Position => transform.position;
        public float Radius => 0;
        public Health Health => _health;
        
        public Team Team => _team;
        public Team EnemyTeam => _enemyTeam;
        
        public DamageableType Type => DamageableType.Structure;
        public DamageableProperty Properties => _properties;

        public bool CanNotTakeDamage => _canNotTakeDamage;

        public float VisionDistance => 0;

        public float Direction => _direction;

        public bool IsDepleted => _isDepleted;

        public void Init(Team team, Team enemyTeam, float direction, DamageableRegistry damageableRegistry, GhostFactory ghostFactory)
        {
            _team = team;
            _enemyTeam = enemyTeam;
            SetDirection(direction);

            _health = new Health(_heathPoints);
            
            _stateMachine = GetComponentInChildren<StateMachine>();

            var attackState = GetComponentInChildren<AnimatronicSummonState>();
            attackState.Init("Attack", this, _summonDuration, ghostFactory, "Idle", _stateMachine);
            _stateMachine.AddState(attackState);
            
            var prepareAttack = GetComponentInChildren<PrepareAttackState>(); 
            prepareAttack.Init("PrepareAttack", _waitDuration, attackState, _stateMachine);
            _stateMachine.AddState(prepareAttack);

            var idle = GetComponentInChildren<LockedState>();
            idle.Init("Idle", this, _stateMachine);
            _stateMachine.AddState(idle);
            
            var takeDamageState = GetComponentInChildren<TakeDamageState>();
            takeDamageState.Init("TakeDamage", this, "Die", "Idle", _stateMachine);
            _stateMachine.AddState(takeDamageState);

            var dieState = GetComponentInChildren<AnimatronicDieState>();
            dieState.Init("Die", this, damageableRegistry, ghostFactory, _stateMachine);
            _stateMachine.AddState(dieState);

            _skinRenderer = GetComponentInChildren<AnimatronicSkinRenderer>();
            _skinRenderer.Init(this, _stateMachine);
            
            _stateMachine.SetState("PrepareAttack");
            
            _isInitialized = true;
        }
        
        public void SetInvincible(bool value)
        {
            _canNotTakeDamage = value;
        }

        public void SetDepleted()
        {
            _isDepleted = true;
        }
        
        public void TakeDamage(int damage)
        {
            if (CanNotTakeDamage)
            {
                return;
            }

            Health.TakeDamage(damage);
            _skinRenderer.FlashRed();

            if (Health.IsDead)
            {
                _stateMachine.SetState("TakeDamage");
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
                Debug.LogError($"{nameof(Peashooter)} not initialized", this);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
        }
#endif
    }
}