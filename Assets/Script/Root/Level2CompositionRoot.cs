namespace Project
{
    using System.Collections.Generic;
    using UnityEngine;

    public class Level2CompositionRoot : Context
    {
        private LoadingScreen _loadingScreen = null;
        private CountDownScreen _countDownScreen = null;
        
        private void Awake()
        {
            var timeSpeedHandler = GetComponentInChildren<TimeSpeedHandler>(true);
            timeSpeedHandler.Init();

            var accelerationButton = GetComponentInChildren<AccelerationButton>(true);
            accelerationButton.Init(timeSpeedHandler);
            
            var loadingScreen = GetComponentInChildren<LoadingScreen>(true);
            loadingScreen.Init();
            
            var levelLoader = GetComponentInChildren<LevelLoader>(true);
            levelLoader.Init(loadingScreen);
            
            var progress = GetComponentInChildren<Progress>(true);
            progress.Init();

            GetComponentInChildren<PauseMenu>(true).Init(timeSpeedHandler, levelLoader);
            
            var damageableRegistry = new Registry<IDamageable>(200);

            var camera = GetComponentInChildren<Camera>(true);
            var cameraMover = GetComponentInChildren<CameraMover>(true);
            cameraMover.Init();
            
            GetComponentInChildren<Clouds>(true).Init();
            
            var moneyCounter = GetComponentInChildren<MoneyCounter>(true);
            var moneyCounterView = GetComponentInChildren<MoneyCounterView>(true);

            moneyCounter.Init();
            moneyCounterView.Init(moneyCounter);
            
            var sunSpawner = GetComponentInChildren<SunSpawner>(true);
            
            sunSpawner.Init(camera, moneyCounter, moneyCounterView);
            
            var playerStructure = GetComponentInChildren<PlayerStructure>(true);
            playerStructure.Init(damageableRegistry);

            var enemyStructure = GetComponentInChildren<Level2EnemyStructure>(true);
            enemyStructure.Init(damageableRegistry);
            
            var services = new List<IService>();
            
            foreach (var service in GetComponentsInChildren<UnitSpawnService>(true))
            {
                service.Init(playerStructure.Structure, moneyCounter, damageableRegistry);
                services.Add(service);
            }

            var serviceButtonPanel = GetComponentInChildren<ServiceButtonPanel>(true);
            var serviceButtonCreator = GetComponentInChildren<ServiceButtonCreator>(true);
            
            serviceButtonPanel.Init();
            serviceButtonCreator.Init(services.ToArray(), serviceButtonPanel, progress);
            
            var gameOverMenu = GetComponentInChildren<GameOverMenu>(true);
            gameOverMenu.Init(levelLoader);
            
            var gameOverHandler = GetComponentInChildren<GameOverHandler>(true);
            gameOverHandler.Init(playerStructure, enemyStructure, cameraMover, timeSpeedHandler, serviceButtonPanel, gameOverMenu, progress);

            var countDownScreen = GetComponentInChildren<CountDownScreen>(true);
            countDownScreen.Init(timeSpeedHandler.RenewTime);
            
            timeSpeedHandler.StopTime();
            
            _loadingScreen = loadingScreen;
            _countDownScreen = countDownScreen;
        }

        private void Start()
        {
            _loadingScreen.Hide();
            _countDownScreen.Show();
        }
    }
}