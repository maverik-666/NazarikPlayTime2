namespace Project
{
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class Level4EnemyStructure : MonoBehaviour, IEnemyStructure
    {
        private Structure _structure;
        private PeashooterFactory _peashooterFactory;
        private RotFactory _rotFactory;
        
        private bool _isInitialized = false;

        public Structure Structure => _structure;

        public void Init(DamageableRegistry damageableRegistry, UnitRegistry unitRegistry)
        {
            _structure = GetComponent<Structure>();
            _structure.Init();
            damageableRegistry.Add(_structure);

            foreach (var unitSpawner in GetComponentsInChildren<TimedUnitSpawner>(false))
            {
                var unitFactory = unitSpawner.GetComponentInChildren<IUnitFactory>(true);

                unitFactory.Init(damageableRegistry, unitRegistry);
                unitSpawner.Init(_structure.Team, _structure.EnemyTeam, _structure.Direction, unitFactory);
            }
            
            foreach (var unitSpawner in GetComponentsInChildren<DelayedUnitSpawner>(false))
            {
                var unitFactory = unitSpawner.GetComponentInChildren<IUnitFactory>(true);

                unitFactory.Init(damageableRegistry, unitRegistry);
                unitSpawner.Init(_structure.Team, _structure.EnemyTeam, _structure.Direction, unitFactory);
            }
            
            _isInitialized = true;
        }

#if UNITY_EDITOR
        private void Start()
        {
            if (!_isInitialized)
            {
                Debug.LogError($"{nameof(Level4EnemyStructure)} not initialized", this);
            }
        }
#endif
    }
}