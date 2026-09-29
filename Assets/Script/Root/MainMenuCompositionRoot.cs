namespace Project
{
    using Game.Scripts;

    public class MainMenuCompositionRoot : Context
    {
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
        }

        private void Start()
        {
            _loadingScreen.Hide();
        }
    }
}