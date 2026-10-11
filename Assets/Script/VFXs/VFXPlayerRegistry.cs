namespace Project
{
    using System.Collections.Generic;
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class VFXPlayerRegistry : MonoBehaviour
    {
        private Dictionary<VFXId, OneTimeVFXPlayer> _vfxPlayers;
        private bool _isInitialized = false;

        public void Init()
        {
            _vfxPlayers = new Dictionary<VFXId, OneTimeVFXPlayer>();
     
            foreach (var vfxPlayer in GetComponentsInChildren<OneTimeVFXPlayer>(true))
            {
                vfxPlayer.Init();
                _vfxPlayers.Add(vfxPlayer.VFXId, vfxPlayer);
            }
            
            _isInitialized = true;
        }

        public OneTimeVFXPlayer GetPlayerBy(VFXId vfxId)
        {
            return _vfxPlayers[vfxId];
        }

#if UNITY_EDITOR
        private void Start()
        {
            if (!_isInitialized)
            {
                Debug.LogError($"{nameof(VFXPlayerRegistry)} not initialized", this);
            }
        }
#endif
    }
}