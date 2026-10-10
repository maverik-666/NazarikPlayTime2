namespace Project
{
    using UnityEngine;

    public class MainMenuCompositionRoot : Context
    {
        [SerializeField]
        private SingleSoundData _music;

        [SerializeField]
        private SingleSoundData _endGameMusic;
        
        private LoadingScreen _loadingScreen;
        private Progress _progress;
        
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
            _progress = progress;
            
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

            if (_progress.IsLevelUnlocked(5) && !_progress.IsLevelUnlocked(6))
            {
                MusicManager.Instance.PlayMusic(_endGameMusic.AudioClip, _endGameMusic.Volume);
            }
            else
            {
                MusicManager.Instance.PlayMusic(_music.AudioClip, _music.Volume);
            }
        }
    }
}