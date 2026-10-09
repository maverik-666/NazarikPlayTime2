namespace Project
{
    using DG.Tweening;
    using LudicWorlds;
    using UnityEngine;

    public class AnimatronicSkinRenderer : MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer _shadow;
        
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
        private RandomSoundData _deathSound;
        
        private Animatronic _unit;
        private Animator _animator;
        private StateMachine _stateMachine;
        private SpriteRenderer _spriteRenderer;
        private SpriteFlasher _spriteFlasher;
        
        private bool _isInitialized = false;

        public void Init(Animatronic unit, StateMachine stateMachine)
        {
            _unit = unit;
            _stateMachine = stateMachine;
            _animator = GetComponent<Animator>();
            
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _spriteFlasher = new SpriteFlasher(gameObject, _spriteRenderer);
            
            _stateMachine.StateEntered += OnStateEntered;

            _spriteRenderer.transform.position += Vector3.up * 0.25f;
            _spriteRenderer.transform.DOMoveY(-0.25f, 0.5f)
                .SetLink(gameObject, LinkBehaviour.KillOnDestroy)
                .SetRelative()
                .SetEase(Ease.OutCubic);
            
            _shadow.transform.localScale = Vector3.zero;
            _shadow.transform.DOScale(1, 0.5f)
                .SetLink(gameObject, LinkBehaviour.KillOnDisable)
                .SetEase(Ease.OutSine);
            
            _isInitialized = true;
        }

        private void OnStateEntered(IState state)
        {
            if (state.ID == "Idle" || state.ID == "PrepareAttack")
            {
                if (!_animator.GetCurrentAnimatorStateInfo(0).IsName(state.ID))
                {
                    _animator.Play(state.ID);
                }

                Idle();
                return;
            }
            
            if (state.ID == "Attack")
            {
                Attack();
                _animator.Play(state.ID);
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

            transform.DOShakePosition(1, Vector3.right * 0.3f)
                .SetDelay(2)
                .SetLink(gameObject, LinkBehaviour.KillOnDestroy);
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