namespace Project
{
    using DG.Tweening;
    using LudicWorlds;
    using UnityEngine;

    public class CatSkinRenderer : MonoBehaviour
    {
        private static readonly int _levelId = Shader.PropertyToID("_Level");
        private static readonly int _saturationColorId = Shader.PropertyToID("_SaturationColor");
        private static readonly int _saturationPhaseId = Shader.PropertyToID("_SaturationPhase");
        
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
        private SingleSoundData _attackSound;

        [SerializeField]
        private float _attackSoundDelay;
        
        private Cat _unit;
        private SpriteRenderer _spriteRenderer;
        private Animator _animator;
        private StateMachine _stateMachine;
        
        private MaterialPropertyBlock _propBlock;
        private Tween _flashTween;
        
        private bool _isInitialized = false;

        public void Init(Cat unit, StateMachine stateMachine)
        {
            _unit = unit;
            _stateMachine = stateMachine;
            _animator = GetComponent<Animator>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _propBlock = new MaterialPropertyBlock();
            
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
            _attackParticle.Play();

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
            FlashColor(Color.white, _flashColorTo, _flashDuration);
        }

        
        public void FlashRed()
        {
            FlashColor(Color.red, _flashColorTo, _flashDuration);
        }
        
        private void FlashColor(Color color, float maxValue, float duration)
        {
            _spriteRenderer.GetPropertyBlock(_propBlock);
            _propBlock.SetColor(_saturationColorId, color);
            _spriteRenderer.SetPropertyBlock(_propBlock);

            _flashTween?.Kill();
            _flashTween = DOVirtual.Float(0f, maxValue, duration, SetFillPhase)
                .SetLink(gameObject, LinkBehaviour.KillOnDisable)
                .SetLoops(2, LoopType.Yoyo);

            void SetFillPhase(float phase)
            {
                _spriteRenderer.GetPropertyBlock(_propBlock);
                _propBlock.SetFloat(_saturationPhaseId, phase);
                _spriteRenderer.SetPropertyBlock(_propBlock);
            }
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