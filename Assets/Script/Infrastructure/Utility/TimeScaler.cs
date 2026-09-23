namespace Project
{
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class TimeScaler : MonoBehaviour
    {
#if UNITY_EDITOR
        [SerializeField]
        private bool _reset = false;

        [SerializeField]
        [Range(0f, 5f)]
        private float _timeScale = 1f;

        private void OnValidate()
        {
            Time.timeScale = _timeScale;

            if (_reset)
            {
                _reset = false;
                _timeScale = 1f;
                Time.timeScale = _timeScale;
            }
        }
#endif
    }
}