namespace Project
{
    using DG.Tweening;
    using LudicWorlds;
    using UnityEngine;

    public class KrisSkinRenderer : MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer _shadow;

        [SerializeField]
        private ParticleSystem _healParticle;

        [SerializeField]
        private ParticleSystem _itemParticle;
        
        [SerializeField]
        private ParticleSystem _dieParticle;

        [SerializeField]
        private float _tiltRotation = 30f;
        
        [Header("Shaders")]
        [SerializeField]
        private float _flashColorTo = 0.4f;

        [SerializeField]
        private float _flashDuration = 0.15f;

        [Header("Sounds")]
        [SerializeField]
        private SingleSoundData _plantSound;

        [SerializeField]
        private RandomSoundData _attackSound;
        
        [SerializeField]
        private RandomSoundData _deathSound;
        
        private Kris _unit;
        private Animator _animator;
        private StateMachine _stateMachine;
        private SpriteRenderer _spriteRenderer;
        private SpriteFlasher _spriteFlasher;
        private Tween _shakeTween;
        
        private bool _isInitialized = false;

        public void Init(Kris unit, StateMachine stateMachine)
        {
            _unit = unit;
            _stateMachine = stateMachine;
            _animator = GetComponent<Animator>();
            
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _spriteFlasher = new SpriteFlasher(gameObject, _spriteRenderer);
            
            _spriteRenderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            _shadow.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            
            _stateMachine.StateEntered += OnStateEntered;
            _isInitialized = true;
        }

        private void OnStateEntered(IState state)
        {
            _animator.Play(state.ID);
            
            if (state.ID == "Heal")
            {
                Heal();
            }
            else if (state.ID == "Die")
            {
                Die();
            }
            else if (state.ID == "Defend")
            {
                Defend();
            }
            else if (state.ID == "TakeDamage")
            {
                TakeDamage();
            }
        }
        

        private void Heal()
        {
            FlashWhite();
            SoundManager.Instance.Play(_attackSound.GetRandomSound());
            
            _healParticle.Play(true);
            _itemParticle.Play(true);
        }
        
        private void Die()
        {
            _spriteRenderer.enabled = false;
            _shadow.enabled = false;
            _dieParticle.Play();
            SoundManager.Instance.Play(_deathSound.GetRandomSound());
        }

        private void TakeDamage()
        {
            transform.rotation = Quaternion.Euler(0f, 0f, _tiltRotation * _unit.Direction); 
            
            FlashRed();
        }

        private void Defend()
        {
            _shakeTween?.Kill(true); 
            _shakeTween = transform.DOShakePosition(0.15f, new Vector3(0.125f, 0, 0), 10, 90, false)
                .SetLink(gameObject, LinkBehaviour.KillOnDisable);
            _spriteFlasher.FlashColor(Color.red, _flashColorTo, _flashDuration);
            
            FlashRed();
        }

        private void FlashWhite()
        {
            _spriteFlasher.FlashColor(Color.white, _flashColorTo, _flashDuration);
        }
        
        public void FlashRed()
        {
            _spriteFlasher.FlashColor(Color.red, _flashColorTo, _flashDuration);
        }

        private void OnDestroy()
        {
            _stateMachine.StateEntered -= OnStateEntered;
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