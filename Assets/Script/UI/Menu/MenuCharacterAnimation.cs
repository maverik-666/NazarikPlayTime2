namespace Project
{
    using DG.Tweening;
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class MenuCharacterAnimation : MonoBehaviour
    {
        [SerializeField]
        private float _scale = 1.1f;
        
        [SerializeField]
        private float _duration = 2;

        private bool _isInitialized = false;
        
        public void Init()
        {
            transform.DOScale(_scale, _duration)
                .SetLink(gameObject, LinkBehaviour.KillOnDestroy)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
            
            _isInitialized = true;
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