namespace Project
{
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(LineRenderer))]
    public class LineOrderInLayer : MonoBehaviour
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

        private LineRenderer _trail = null;
        private readonly int _factor = 1000;

        private float Y => _baseValueOn ? _baseValueOn.position.y : transform.position.y;

#if UNITY_EDITOR
        private void Reset()
        {
            if (_baseValueOn == null)
            {
                _baseValueOn = transform.parent;
            }
        }
#endif

        private void Start()
        {
            _trail = GetComponent<LineRenderer>();
        }

        private void Update()
        {
            _layerId = _trail.sortingLayerName;
            _trail.sortingOrder = -(int)(Y * _factor) + _addedValue;
        }
    }
}