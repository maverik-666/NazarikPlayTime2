namespace Project
{
    using UnityEngine;

    public class CompositionRoot : Context
    {
        private void Awake()
        {
            var damageableRegistry = new Registry<IDamageable>(200);

            var cameraMover = GetComponentInChildren<CameraMover>();
            cameraMover.Init();
            
            var unitFactory = GetComponentInChildren<UnitFactory>(true);
            unitFactory.Init(damageableRegistry);

            GetComponentInChildren<Clouds>(true).Init();
            
            foreach (var timedUnitSpawner in GetComponentsInChildren<TimedUnitSpawner>(true))
            {
                timedUnitSpawner.Init(unitFactory);
            }
            
            foreach (var structure in GetComponentsInChildren<Structure>(true))
            {
                structure.Init();
                damageableRegistry.Add(structure);
            }
        }
    }
}