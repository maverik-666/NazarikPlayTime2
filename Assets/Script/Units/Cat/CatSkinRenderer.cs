namespace Project
{
    using DG.Tweening;
    using LudicWorlds;
    using UnityEngine;

    public class CatSkinRenderer : MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer _shadow;
        
        [SerializeField]
        private VFXSource _attackVfx;

        [SerializeField]
        private VFXSource _dieVfx;

        [SerializeField]
        private float _tiltRotation = 30f;
        
        [Header("Shaders")]
        [SerializeField]
        private float _flashColorTo = 0.4f;

        [SerializeField]
        private float _flashDuration = 0.15f;

        [Header("Sounds")]
        [SerializeField]
        private RandomSoundData _deathSound;
        
        [SerializeField]
        private SingleSoundData _attackSound;

        [SerializeField]
        private float _attackSoundDelay;
        
        private Cat _unit;
        private SpriteRenderer _spriteRenderer;
        private Animator _animator;
        private StateMachine _stateMachine;

        private SpriteFlasher _spriteFlasher;
        private Tween _flashTween;
        
        private bool _isInitialized = false;

        public void Init(Cat unit, StateMachine stateMachine)
        {
            _unit = unit;
            _stateMachine = stateMachine;
            _animator = GetComponent<Animator>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _spriteFlasher = new SpriteFlasher(gameObject, _spriteRenderer);
            
            _attackVfx.Init();
            _dieVfx.Init();
            
            _stateMachine.StateEntered += OnStateEntered;
            _isInitialized = true;
        }

        private void OnStateEntered(IState state)
        {
            _animator.Play(state.ID);
            
            if (state.ID == "Idle")
            {
                Idle();
            }
            if (state.ID == "Attack")
            {
                Attack();
            }
            else if (state.ID == "Die")
            {
                Die();
            }
            else if (state.ID == "TakeDamage")
            {
                TakeDamage();
            }
        }

        private void Idle()
        {
            transform.rotation = Quaternion.identity;
        } 

        private void Attack()
        {
            FlashWhite();
            _attackVfx.Play();

            DOTween.Sequence()
                .SetLink(gameObject, LinkBehaviour.KillOnDisable)
                .AppendInterval(_attackSoundDelay)
                .AppendCallback(PlayAttackSound);
        }

        private void PlayAttackSound()
        {
            SoundManager.Instance.Play(_attackSound, _attackSound.GetRandomPitch());
        }

        private void Die()
        {
            _spriteRenderer.enabled = false;
            _shadow.enabled = false;
            _dieVfx.Play();
            
            SoundManager.Instance.Play(_deathSound.GetRandomSound());
        }

        private void TakeDamage()
        {
            transform.rotation = Quaternion.Euler(0f, 0f, _tiltRotation * _unit.Direction); 
            
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