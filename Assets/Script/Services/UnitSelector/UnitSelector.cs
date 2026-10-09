namespace Project
{
    using System;
    using DG.Tweening;
    using UnityEngine;
    using UnityEngine.InputSystem;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class UnitSelector : MonoBehaviour
    {
        [SerializeField]
        private CanvasGroup _canvasGroup;
        
        private SelectorAim _aim;
        
        private DamageableRegistry _damageableRegistry; 
        private Camera _camera;

        private Transform _target;
        
        private Action<IDamageable> _requestCallback;
        private Team _targetTeam;
        private Structure _structure;
        private DamageableType _allowedTypes; 
        private bool _hasRequest = false;

        private Tween _fadeTween = null;

        private IDamageable _currentTarget;
        private bool _isInitialized = false;

        public void Init(Camera camera, DamageableRegistry damageableRegistry)
        {
            _camera = camera;
            _damageableRegistry = damageableRegistry;
            _aim = GetComponentInChildren<SelectorAim>(true);
            _aim.Init();

            _canvasGroup.blocksRaycasts = false;
            _canvasGroup.alpha = 0;
            
            _isInitialized = true;
        }
        
        public void RequestSelection(Structure structure, Team targetTeam, DamageableType allowedTypes, Sprite aimSprite, Action<IDamageable> callback)
        {
            _structure = structure;
            _targetTeam = targetTeam;
            _allowedTypes = allowedTypes;
            _aim.SetSprite(aimSprite);

            if (!TryGetClosestToMouse(out _currentTarget))
            {
                return;
            }
            
            _aim.Show(_currentTarget.transform);

            _fadeTween?.Kill();
            _fadeTween = _canvasGroup.DOFade(1, 0.5f)
                .SetLink(gameObject, LinkBehaviour.KillOnDestroy)
                .SetEase(Ease.OutSine);
            
            _requestCallback = callback;
            _hasRequest = true;
                
        }
        
        private bool TryGetClosestToMouse(out IDamageable target)
        {
            var mouseWorldPosition = (Vector2) _camera.ScreenToWorldPoint(Mouse.current.position.ReadValue());

            return TryFindClosestTargetTo(mouseWorldPosition, out target);
        }
        
        private bool TryFindClosestTargetTo(Vector2 position, out IDamageable target)
        {
            target = null;
            bool foundAny = false;
            float closestDist = float.MaxValue;
            
            foreach (var damageable in _damageableRegistry.Items)
            { 
                if (damageable.Team != _targetTeam || !_allowedTypes.HasFlag(damageable.Type))
                    continue;

                float distance = Vector2.Distance(damageable.Position, position);
                if (closestDist > distance)
                {
                    target = damageable;
                    foundAny = true;
                    closestDist = distance;
                }
            }
            
            return foundAny;
        }


        private void Update()
        {
            if (!_hasRequest)
            {
                return;
            }

            if (!TryGetClosestToMouse(out _currentTarget))
            {
                Hide();
                return;
            }
            
            _aim.SetTarget(_currentTarget.transform);

            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                Select();
            }
        }

        private void Select()
        {
            _requestCallback?.Invoke(_currentTarget);
            Hide();
        }

        private void Hide()
        {
            _hasRequest = false;
            _aim.Hide();
            
            _fadeTween?.Kill();
            _fadeTween = _canvasGroup.DOFade(0, 0.5f)
                .SetLink(gameObject, LinkBehaviour.KillOnDestroy)
                .SetEase(Ease.OutSine);
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