namespace Project
{
    using UnityEngine;
    using UnityEngine.SceneManagement;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class LevelLoader : MonoBehaviour
    {
        private const string MainMenuName = "MainMenu";

        private LoadingScreen _loadingScreen;
        private int _levelToLoad;
        private bool _isInitialized = false;

        public void Init(LoadingScreen loadingScreen)
        {
            _loadingScreen = loadingScreen;
            _isInitialized = true;
        }

        public void LoadCurrent()
        { 
            _loadingScreen.Show(RestartCurrentAfterAnimation);
        }

        private void RestartCurrentAfterAnimation()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void LoadMainMenu()
        {
            _loadingScreen.Show(LoadMainMenuAfterAnimation);
        }

        private void LoadMainMenuAfterAnimation()
        {
            SceneManager.LoadScene(MainMenuName);
        }

        public void LoadLevel(int level)
        {
            _levelToLoad = level;
            _loadingScreen.Show(LoadLevelAfterAnimation);
        }
        
        private void LoadLevelAfterAnimation()
        {
            SceneManager.LoadScene(_levelToLoad);
        }
        
#if UNITY_EDITOR
        private void Start()
        {
            if (!_isInitialized)
            {
                Debug.LogError($"{nameof(LevelLoader)} not initialized", this);
            }
        }
#endif
    }
}