namespace Project
{
    using System;
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class PlayerStructure : MonoBehaviour
    {
        private Structure _structure;
        private bool _isInitialized = false;

        public Structure Structure => _structure;

        public void Init(DamageableRegistry damageableRegistry)
        {
            _structure = GetComponent<Structure>();
            _structure.Init();
            damageableRegistry.Add(_structure);

            _isInitialized = true;
        }

#if UNITY_EDITOR
        private void Start()
        {
            if (!_isInitialized)
            {
                Debug.LogError($"{nameof(PlayerStructure)} not initialized", this);
            }
        }
#endif
    }
}