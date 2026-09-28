namespace Project
{
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class Level1EnemyStructure : MonoBehaviour, IEnemyStructure
    {
        private Structure _structure;
        private bool _isInitialized = false;

        public Structure Structure => _structure;

        public void Init(DamageableRegistry damageableRegistry)
        {
            _structure = GetComponent<Structure>();
            _structure.Init();
            damageableRegistry.Add(_structure);
            
            var unitFactory = GetComponentInChildren<IUnitFactory>(true);
            unitFactory.Init(damageableRegistry);

            var unitSpawner = GetComponentInChildren<TimedUnitSpawner>(true);
            unitSpawner.Init(_structure.Team, _structure.EnemyTeam, _structure.Direction, unitFactory);
            
            _isInitialized = true;
        }

#if UNITY_EDITOR
        private void Start()
        {
            if (!_isInitialized)
            {
                Debug.LogError($"{nameof(Level1EnemyStructure)} not initialized", this);
            }
        }
#endif
    }
}