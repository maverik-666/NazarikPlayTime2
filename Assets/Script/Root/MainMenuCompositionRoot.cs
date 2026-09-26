namespace Project
{ 
    public class MainMenuCompositionRoot : Context
    {
        private void Awake()
        {
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
        }
    }
}