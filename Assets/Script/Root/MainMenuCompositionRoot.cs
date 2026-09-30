namespace Project
{
    using UnityEngine;

    public class MainMenuCompositionRoot : Context
    {
        [SerializeField]
        private SingleSoundData _music;
        
        private LoadingScreen _loadingScreen;
        
        private void Awake()
        {
            GetComponentInChildren<MusicManager>(true).Init();
            GetComponentInChildren<SoundManager>(true).Init();
            
            var loadingScreen = GetComponentInChildren<LoadingScreen>(true);
            loadingScreen.Init();
            
            var levelLoader = GetComponentInChildren<LevelLoader>(true);
            levelLoader.Init(loadingScreen);
            
            var progress = GetComponentInChildren<Progress>(true);
            progress.Init();
            
            GetComponentInChildren<MainMenu>(true).Init(levelLoader, progress);

            foreach (var krisHand in GetComponentsInChildren<KrisHand>(true))
            {
                krisHand.Init();
            }
            
            foreach (var character in GetComponentsInChildren<MenuCharacterAnimation>(true))
            {
                character.Init();
            }
            
            GetComponentInChildren<MenuCat>(true).Init();

            _loadingScreen = loadingScreen;
            
            
            GetComponentInChildren<TimeSpeedHandler>().Init();
        }

        private void Start()
        {
            _loadingScreen.Hide();
            
            MusicManager.Instance.PlayMusic(_music.AudioClip, _music.Volume);
        }
    }
}