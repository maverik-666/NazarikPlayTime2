namespace Project
{
    using System;
    using DG.Tweening;
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class CountDownScreen : MonoBehaviour
    {
        [SerializeField]
        private CanvasGroup _root;
        
        [Header("Texts")]
        [SerializeField]
        private CanvasGroup[] _numbers;
        
        [SerializeField]
        private float _interval = 0.45f;

        [SerializeField]
        private SingleSoundData _countSound;

        [SerializeField]
        private float _pitchStep = 0.15f;
        
        private Action _afterAction;
        private bool _isInitialized = false;

        public void Init(Action afterAction)
        {
            _afterAction = afterAction;
            _root.blocksRaycasts = false;
            _isInitialized = true;
        }

        public void Show()
        {
            _root.blocksRaycasts = true;
            
            var seq = DOTween.Sequence();
            seq.SetLink(gameObject, LinkBehaviour.KillOnDestroy);
            seq.SetUpdate(true);

            seq.AppendInterval(0.5f);
            
            for (int i = 0; i < _numbers.Length; i++)
            {
                int a = i; 
                var number = _numbers[i];
                number.transform.localScale = Vector3.one * 3;
                number.alpha = 0;

                var numSeq = DOTween.Sequence()
                    .AppendCallback(() => SoundManager.Instance.Play(_countSound, 1 + _pitchStep * a))
                    .Append(number.DOFade(1, 0.3f))
                    .Join(number.transform.DOScale(1, 0.3f).SetEase(Ease.InCubic));

                if (i == _numbers.Length - 1)
                {
                    numSeq.AppendCallback(AfterCountDown);
                }
                
                numSeq.Append(number.DOFade(0, 0.15f));
                
                seq.Insert(i * _interval + 0.5f, numSeq);
            }
        }

        private void AfterCountDown()
        {
            _root.blocksRaycasts = false;
            _afterAction?.Invoke();
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