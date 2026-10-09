namespace Project
{
    using DG.Tweening;
    using LudicWorlds;
    using UnityEngine;

    public class GolemSkinRenderer : MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer _shadow;
        
        [SerializeField]
        private ParticleSystem _attackParticle;

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
        
        [SerializeField]
        private RandomSoundData _attackSound;

        [SerializeField]
        private float _attackSoundDelay;
        
        private Golem _unit;
        private Animator _animator;
        private StateMachine _stateMachine;
        private SpriteRenderer _spriteRenderer;
        private SpriteFlasher _spriteFlasher;
        

        private bool _isInitialized = false;

        public void Init(Golem unit, StateMachine stateMachine)
        {
            _unit = unit;
            _stateMachine = stateMachine;
            _animator = GetComponent<Animator>();
            
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _spriteFlasher = new SpriteFlasher(gameObject, _spriteRenderer);
            
            _stateMachine.StateEntered += OnStateEntered;
            _isInitialized = true;
        }

        private void OnStateEntered(IState state)
        {
            if (state.ID == "GolemDie")
            {
                GolemDie();
                return;
            } 
            
            if (state.ID == "Idle" || state.ID == "PrepareAttack")
            { 
                _animator.Play("Idle");

                Idle();
                return;
            }
            
            _animator.Play(state.ID);
            
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
            _attackParticle.Play();
        }


        private void GolemDie()
        {
            _spriteRenderer.enabled = false;
            _shadow.enabled = false;
            
            DOTween.Sequence()
                .SetLink(gameObject, LinkBehaviour.KillOnDisable)
                .AppendInterval(_attackSoundDelay)
                .AppendCallback(PlayAttackSound);
        }

        private void PlayAttackSound()
        {
            SoundManager.Instance.Play(_attackSound.GetRandomSound());
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