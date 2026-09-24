namespace Project
{
    using System.Collections.Generic;
    using UnityEngine;

    public class CompositionRoot : Context
    {
        private void Awake()
        {
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

            var enemyStructure = GetComponentInChildren<EnemyStructure>(true);
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
            serviceButtonCreator.Init(services.ToArray(), serviceButtonPanel);
        }
    }
}