namespace Project
{
    using DG.Tweening;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using Random = UnityEngine.Random;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class MenuCat : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] 
        private Vector2 _intervalRange = new Vector2(1, 3);

        [SerializeField]
        private CanvasGroup _cat;

        [SerializeField]
        private CanvasGroup _face;
        
        private Timer _timer;
        private bool _isInitialized = false;

        public void Init()
        {
            _timer = new Timer();

            _face.alpha = 0;
            _cat.alpha = 1;
            
            StartTimer();
            _isInitialized = true;
        }

        private void StartTimer()
        {
            float interval = Random.Range(_intervalRange.x, _intervalRange.y);
            _timer.Start(interval);
        }

        private void Update()
        {
            _timer.Tick(Time.deltaTime);
            
            if (_timer.IsStopped)
            {
                Blink();
            }
        }

        private void Blink()
        {
            _face.alpha = 1;
            _cat.alpha = 0;
            
            StartTimer();
            
            var seq = DOTween.Sequence();
            seq.AppendInterval(0.05f);
            seq.AppendCallback(RemoveBlink);
        }

        private void RemoveBlink()
        {
            _face.alpha = 0;
            _cat.alpha = 1;

        }

        public void OnPointerClick(PointerEventData eventData)
        {
            // TODO add sound
        }
#if UNITY_EDITOR
        private void Start()
        {
            if (!_isInitialized)
            {
                Debug.LogError($"{nameof(Cat)} not initialized", this);
            }
        }
#endif
       
    }
}