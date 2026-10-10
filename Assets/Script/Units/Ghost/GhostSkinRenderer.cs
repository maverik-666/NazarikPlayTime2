namespace Project
{
    using DG.Tweening;
    using LudicWorlds;
    using UnityEngine;

    public class GhostSkinRenderer : MonoBehaviour
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

        [Header("Shaders")]
        [SerializeField]
        private float _flashColorTo = 0.4f;

        [SerializeField]
        private float _flashDuration = 0.15f;
        
        [Header("Sounds")]
        [SerializeField]
        private RandomSoundData _deathSound;
        
        private Ghost _unit;
        private SpriteRenderer _spriteRenderer;
        private Animator _animator;
        private StateMachine _stateMachine;
        
        private MaterialPropertyBlock _propBlock;
        private Tween _flashTween;
        private Sequence _flySeq;
        
        private bool _isInitialized = false;

        public void Init(Ghost unit, StateMachine stateMachine)
        {
            _unit = unit;
            _stateMachine = stateMachine;
            _animator = GetComponent<Animator>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _propBlock = new MaterialPropertyBlock();
            
            _stateMachine.StateEntered += OnStateEntered;
            
            _flySeq = DOTween.Sequence();
            _flySeq.SetLink(gameObject, LinkBehaviour.KillOnDestroy);
            _flySeq.SetLoops(-1, LoopType.Yoyo);

            _flySeq.Append(_spriteRenderer.transform.DOMoveY(-0.2f, 2)
                .SetRelative()
                .SetEase(Ease.InOutSine));
            _flySeq.Join(_shadow.transform.DOScale(0.8f, 2)
                .SetRelative()
                .SetEase(Ease.InOutSine));
            
            transform.localScale = Vector3.zero;
            transform.DOScale(1, 0.5f)
                .SetLink(gameObject, LinkBehaviour.KillOnDisable)
                .SetEase(Ease.OutSine);

            
            _isInitialized = true;
        }

        private void OnStateEntered(IState state)
        {
            if (state.ID == "Idle" || state.ID == "Attack" || state.ID == "Die")
            {
                _animator.Play("Idle");
            }
            else if (state.ID == "Move")
            {
                _animator.Play("Move");
            } 
            
            if (state.ID == "Attack")
            {
                Attack();
            }
            else if (state.ID == "Die")
            {
                Die();
            }
        }

        private void Attack()
        {
            FlashWhite();
            _attackParticle.Play();
        }

        private void Die()
        {
            _spriteRenderer.enabled = false;
            _shadow.enabled = false;
            _dieParticle.Play();
            
            SoundManager.Instance.Play(_deathSound.GetRandomSound());
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