namespace Project
{
    using DG.Tweening;
    using LudicWorlds;
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class Kris : MonoBehaviour, IUnit
    {
        [SerializeField]
        private float _poseDuration;
        
        [SerializeField]
        private float _healCooldown = 5;
        
        [SerializeField]
        private int _heathPoints = 1000;

        [SerializeField]
        private DamageableProperty _properties = DamageableProperty.None;

        private Health _health;
        private Team _team;
        private Team _enemyTeam;
        private float _direction = 1;
        
        private StateMachine _stateMachine;
        private KrisSkinRenderer _skinRenderer;

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
        
        public float Direction => _direction;
        

        public void Init(Team team, Team enemyTeam, float direction, DamageableRegistry damageableRegistry, DarkTransition darkTransition)
        {
            _team = team;
            _enemyTeam = enemyTeam;
            SetDirection(direction);

            _health = new Health(_heathPoints);
            
            _stateMachine = GetComponentInChildren<StateMachine>();

            var poseState = GetComponentInChildren<WaitState>();
            poseState.Init("Pose", 5, "Idle", _stateMachine);
            _stateMachine.AddState(poseState);
            
            var idleState = GetComponentInChildren<WaitForHealState>();
            idleState.Init("Idle", this, _healCooldown, "Heal", _stateMachine);
            _stateMachine.AddState(idleState);
            
            var healState = GetComponentInChildren<HealState>();
            healState.Init("Heal", this, "Idle", _health.Max, 1, _stateMachine);
            _stateMachine.AddState(healState);
            
            var takeDamageState = GetComponentInChildren<TakeDamageState>();
            takeDamageState.Init("TakeDamage", this, "Die", "Idle", _stateMachine);
            _stateMachine.AddState(takeDamageState);

            var defendState = GetComponentInChildren<DefendState>();
            defendState.Init("Defend", this,"Idle", 1, _stateMachine);
            _stateMachine.AddState(defendState);
            
            var dieState = GetComponentInChildren<DieState>();
            dieState.Init("Die", this, damageableRegistry, _stateMachine);
            _stateMachine.AddState(dieState);

            _skinRenderer = GetComponentInChildren<KrisSkinRenderer>();
            _skinRenderer.Init(this, _stateMachine);
            
            _stateMachine.SetState("Pose");
            
            darkTransition.Play();
            
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
            
            Health.TakeDamage(damage);
            _skinRenderer.FlashRed();

            if (_health.IsDead)
            {
                _stateMachine.SetState("TakeDamage");
            }
            else
            { 
                _stateMachine.SetState("Defend");
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
       _stateMachine.SetState("Die");}

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
#endif
    }
}