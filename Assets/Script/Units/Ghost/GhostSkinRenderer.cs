namespace Project
{
    using DG.Tweening;
    using LudicWorlds;
    using UnityEngine;

    public class GhostSkinRenderer : MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer _shadow;
        
        [SerializeField]
        private VFXSource _attackVfx;

        [SerializeField]
        private VFXSource _dieVfx;

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

        private SpriteFlasher _spriteFlasher;
        private Tween _flashTween;
        private Sequence _flySeq;
        
        private bool _isInitialized = false;

        public void Init(Ghost unit, StateMachine stateMachine)
        {
            _unit = unit;
            _stateMachine = stateMachine;
            _animator = GetComponent<Animator>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _spriteFlasher = new SpriteFlasher(gameObject, _spriteRenderer);
            
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

            _attackVfx.Init();
            _dieVfx.Init();
            
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
            _attackVfx.Play();
        }

        private void Die()
        {
            _spriteRenderer.enabled = false;
            _shadow.enabled = false;
            _dieVfx.Play();
            
            SoundManager.Instance.Play(_deathSound.GetRandomSound());
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