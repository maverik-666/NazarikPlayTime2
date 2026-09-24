namespace Project
{
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteOrderInLayer : MonoBehaviour
    {
#pragma warning disable IDE0044
        [SerializeField]
        private Transform _baseValueOn = null;

        [SerializeField]
        private int _addedValue = 0;

        [Header("Debug")]
        [SerializeField]
        private string _layerId = string.Empty;
#pragma warning restore IDE0044

        private SpriteRenderer _sprite = null;
        private readonly int _factor = 1000;

        private float Y => _baseValueOn ? _baseValueOn.position.y : transform.position.y;
        
        private void Start()
        {
            _sprite = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            _layerId = _sprite.sortingLayerName;
            _sprite.sortingOrder = -(int)(Y * _factor) + _addedValue;
        }
        
#if UNITY_EDITOR
        private void Reset()
        {
            if (_baseValueOn == null)
            {
                _baseValueOn = transform.parent;
            }
        }
#endif
    }
}