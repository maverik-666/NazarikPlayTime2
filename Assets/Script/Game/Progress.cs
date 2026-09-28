namespace Project
{
    using UnityEngine;


    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class Progress : MonoBehaviour
    {
        private bool _isInitialized = false;

        public void Init()
        {
            _isInitialized = true;
        }

        public bool IsUnlocked(Unlockable unlockable)
        {
            return unlockable == Unlockable.None || PlayerPrefs.HasKey(unlockable.ToString());
        }

        public void Unlock(Unlockable unlockable)
        {
            PlayerPrefs.SetString(unlockable.ToString(), 1.ToString());
        }

        public void UnlockLevel(int level)
        {
            PlayerPrefs.SetString($"Level{level}", 1.ToString());
        }
        
        public bool IsLevelUnlocked(int level)
        {
            if (level == 1)
            {
                return true;
            }
            
            return PlayerPrefs.HasKey($"Level{level}");
        }
        
#if UNITY_EDITOR
        private void Start()
        {
            if (!_isInitialized)
            {
                Debug.LogError($"{nameof(Progress)} not initialized", this);
            }
        }
#endif
    }
}