namespace Project
{
    using DG.Tweening;
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class Olmek : MonoBehaviour, IDamageable
    {
        [SerializeField]
        private float _radius;
        [SerializeField]
        private int _heathPoints = 10000;

        [SerializeField]
        private float _attackDistance = 2;

        [SerializeField]
        private int _attack = 250;

        [SerializeField]
        private float _jumpOffScreenDelay = 3;
        
        private Team _team = Team.Red;
        
        private Health _health;

        private OlmekSkinRenderer _skinRenderer;
        private DamageableRegistry _damageableRegistry;
        private CameraMover _cameraMover;
        
        private bool _isInitialized = false;

        public Vector2 Position => transform.position;
        public float Radius => _radius;
        
        public Health Health => _health;
        
        public Team Team => _team;
        
        public DamageableType Type => DamageableType.Structure;

        public bool CanNotTakeDamage => true;
        
        public void Init(Team team, CameraMover cameraMover, DamageableRegistry damageableRegistry)
        {
            _team = team;
            _health = new Health(_heathPoints);
            _skinRenderer = GetComponentInChildren<OlmekSkinRenderer>();

            _damageableRegistry = damageableRegistry;
            _cameraMover = cameraMover;
            
            _skinRenderer.Init();
            
            _skinRenderer.AnimateFall(AfterFall);
            
            _isInitialized = true;
        }

        private void AfterFall()
        {
            Attack();
            
            _cameraMover.Shake(0.3f, 10);
            _damageableRegistry.Add(this);
            DOTween.Sequence()
                .SetLink(gameObject, LinkBehaviour.KillOnDestroy)
                .AppendInterval(_jumpOffScreenDelay)
                .AppendCallback(JumpOffScreen);
        }

        private void JumpOffScreen()
        {
            _damageableRegistry.Remove(this);
            _skinRenderer.JumpOffScreen(DestroySelf);
        }
        
        private void DestroySelf()
        {
            Destroy(gameObject);
        }
        
        private void Attack()
        {
            for (int i = _damageableRegistry.Items.Count - 1; i >= 0; i--)
            {
                var damageable = _damageableRegistry.Items[i];
                if (IsAttackable(damageable))
                {
                    damageable.TakeDamage(_attack);
                }
            }
        }
        
        private bool IsAttackable(IDamageable damageable)
        {
            float distance = Mathf.Abs(damageable.Position.x - Position.x);
            
            return !damageable.IsDead 
                   && !damageable.CanNotTakeDamage
                   && distance <= _attackDistance;
        }
        
        public void SetInvincible(bool value)
        {
        }
        
        public void TakeDamage(int damage) { }
        
        public void InstantDeath() {}

#if UNITY_EDITOR
        private void Start()
        {
            if (!_isInitialized)
            {
                Debug.LogError($"{nameof(Olmek)} not initialized", this);
            }
        }
#endif
      
    }
}