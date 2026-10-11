namespace Project
{
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class VFXSource : MonoBehaviour
    {
        [SerializeField]
        private VFXId _vfxId;

        private OneTimeVFXPlayer _vfxPlayer;
        private bool _isInitialized = false;

        public void Init()
        {
            _vfxPlayer = this.Resolve<VFXPlayerRegistry>().GetPlayerBy(_vfxId);
            _isInitialized = true;
        }

        public void Play()
        {
            _vfxPlayer.Play(transform.position, transform.lossyScale.x);
        }

#if UNITY_EDITOR
        private void Start()
        {
            if (!_isInitialized)
            {
                Debug.LogError($"{nameof(VFXSource)} not initialized", this);
            }
        }
#endif
    }
}