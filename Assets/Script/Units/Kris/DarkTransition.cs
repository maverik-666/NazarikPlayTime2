namespace Project
{
    using System;
    using DG.Tweening;
    using UnityEngine;

    public class DarkTransition : MonoBehaviour
    {
        [SerializeField]
        private Transform _darkness;

        [SerializeField]
        private Transform _smoke;

        [Header("Darkness Float")]
        [SerializeField]
        private float _floatOffset = 0.25f;
        [SerializeField]
        private float _floatDuration = 2;
        
        [Header("Darkness Move")]
        [SerializeField]
        private Transform _startPos;
        
        [SerializeField]
        private Transform _endPos;

        [SerializeField]
        private float _moveDuration = 3;

        [SerializeField]
        private Ease _ease = Ease.OutCubic;

        private bool _isInitialized = false;

        public void Init()
        {
            _darkness.position = _startPos.position;

            var seq = DOTween.Sequence();
            seq.SetLink(gameObject, LinkBehaviour.KillOnDestroy);
            seq.Append(_smoke.DOLocalMoveY(_floatOffset, _floatDuration)
                .SetEase(Ease.InOutSine));
            seq.Append(_smoke.DOLocalMoveY(0, _floatDuration/2)
                .SetEase(Ease.InOutSine));
            seq.SetLoops(-1, LoopType.Restart);
          
            _isInitialized = true;
        }
        
        public void Play()
        {
            _darkness.DOMove(_endPos.position, _moveDuration)
                .SetLink(gameObject, LinkBehaviour.KillOnDestroy)
                .SetEase(_ease);
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