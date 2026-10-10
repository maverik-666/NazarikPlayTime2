namespace Project
{
    using DG.Tweening;
    using LudicWorlds;
    using UnityEngine;

    public class RotSkinRenderer : MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer _shadow;
        
        [SerializeField]
        private float _tiltRotation = 30f;

        [SerializeField]
        private ParticleSystem _spawnVFX;
        
        [Header("Shaders")]
        [SerializeField]
        private float _flashColorTo = 0.4f;

        [SerializeField]
        private float _flashDuration = 0.15f;

        [Header("Sounds")]
        [SerializeField]
        private RandomSoundData _spawnSound;
        
        private Rot _unit;
        private Animator _animator;
        private StateMachine _stateMachine;
        private SpriteRenderer _spriteRenderer;
        private SpriteFlasher _spriteFlasher;
        
        private bool _isInitialized = false;

        public void Init(Rot unit, StateMachine stateMachine)
        {
            _unit = unit;
            _stateMachine = stateMachine;
            _animator = GetComponent<Animator>();
            
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _spriteFlasher = new SpriteFlasher(gameObject, _spriteRenderer);
            
            transform.localScale = Vector3.zero;
            transform.DOScale(1, 0.5f)
                .SetLink(gameObject, LinkBehaviour.KillOnDisable)
                .SetEase(Ease.OutSine);

            _spawnVFX.Play();
            this.Resolve<CameraMover>().Shake(0.5f, 1f);
            
            SoundManager.Instance.Play(_spawnSound.GetRandomSound());
            
            _stateMachine.StateEntered += OnStateEntered;
            _isInitialized = true;
        }

        private void OnStateEntered(IState state)
        {
            if (state.ID == "Move")
            {
                _animator.Play("Move");
                return;
            }
            
            _animator.Play("Idle");
            
            if (state.ID == "TakeDamage")
            {
                TakeDamage();
            }
            else if (state.ID == "Attack")
            {
                Attack();
            }
            else
            {
                Idle();
            }
        }

        private void Idle()
        {
            transform.rotation = Quaternion.identity;
        } 

        private void Attack()
        {
            FlashWhite();
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

        
        private void FlashRed()
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