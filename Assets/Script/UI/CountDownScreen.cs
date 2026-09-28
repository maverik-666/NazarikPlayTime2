namespace Project
{
    using System;
    using DG.Tweening;
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class CountDownScreen : MonoBehaviour
    {
        [Header("Texts")]
        [SerializeField]
        private CanvasGroup[] _numbers;

        [SerializeField]
        private float _interval = 0.45f; 
        
        private Action _afterAction;
        private bool _isInitialized = false;

        public void Init(Action afterAction)
        {
            _afterAction = afterAction;
            _isInitialized = true;
        }

        public void Show()
        {
            var seq = DOTween.Sequence();
            seq.SetLink(gameObject, LinkBehaviour.KillOnDestroy);
            seq.SetUpdate(true);

            seq.AppendInterval(0.5f);
            
            for (int i = 0; i < _numbers.Length; i++)
            {
                var number = _numbers[i];
                number.transform.localScale = Vector3.one * 3;
                number.alpha = 0;
                seq.Insert(i * _interval + 0.5f, DOTween.Sequence()
                    .Append(number.DOFade(1, 0.3f))
                    .Join(number.transform.DOScale(1, 0.3f).SetEase(Ease.InCubic))
                    .Append(number.DOFade(0, 0.15f)));
            }

            seq.OnComplete(_afterAction.Invoke);
        }

#if UNITY_EDITOR
        private void Start()
        {
            if (!_isInitialized)
            {
                Debug.LogError($"{nameof(CountDownScreen)} not initialized", this);
            }
        }
#endif
    }
}