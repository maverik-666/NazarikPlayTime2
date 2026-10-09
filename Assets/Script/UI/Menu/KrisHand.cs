namespace Project
{
    using DG.Tweening;
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class KrisHand : MonoBehaviour
    {
        [SerializeField]
        private float _offset = 10;

        [SerializeField]
        private float _jumpDuration = 2;

        [SerializeField]
        private float _jumpPower = 3;

        [SerializeField]
        private float _delay = 3;
        
        private bool _isInitialized = false;

        public void Init()
        {
            DOTween.Sequence()
                .SetLink(gameObject, LinkBehaviour.KillOnDestroy)
                .AppendInterval(_delay)
                .AppendCallback(Animate);
            
            _isInitialized = true;
        }
        
        private void Animate()
        {
            GetComponent<RectTransform>().DOJumpAnchorPos(Vector3.right * _offset, _jumpPower, 1, _jumpDuration)
                .SetLink(gameObject, LinkBehaviour.KillOnDestroy)
                .SetRelative()
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
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