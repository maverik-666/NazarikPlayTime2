namespace Project
{
    using DG.Tweening;
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class Level2EnemyStructure : MonoBehaviour, IEnemyStructure
    {
        [SerializeField]
        private float _peaShooterSpawnDelay;
        
        [SerializeField]
        private Transform _peaShooterSpawnPoint;
        
        private Structure _structure;
        private PeashooterFactory _peashooterFactory;
        private bool _isInitialized = false;

        public Structure Structure => _structure;

        public void Init(Registry<IDamageable> damageableRegistry)
        {
            _structure = GetComponent<Structure>();
            _structure.Init();
            damageableRegistry.Add(_structure);
            
            _peashooterFactory = GetComponentInChildren<PeashooterFactory>();
            _peashooterFactory.Init(damageableRegistry);
            
            var unitSpawner = GetComponentInChildren<TimedUnitSpawner>(true);
            var unitFactory = unitSpawner.GetComponentInChildren<IUnitFactory>(true);
            
            unitFactory.Init(damageableRegistry);
            unitSpawner.Init(_structure.Team, _structure.EnemyTeam, _structure.Direction, unitFactory);
            
            DOTween.Sequence()
                .SetLink(gameObject, LinkBehaviour.KillOnDestroy)
                .AppendInterval(_peaShooterSpawnDelay)
                .AppendCallback(CreatePeaShooter);
            
            _isInitialized = true;
        }

        private void CreatePeaShooter()
        {
            _peashooterFactory.Create(_peaShooterSpawnPoint.position, Structure.Team, _structure.EnemyTeam, _structure.Direction);
        }

#if UNITY_EDITOR
        private void Start()
        {
            if (!_isInitialized)
            {
                Debug.LogError($"{nameof(Level2EnemyStructure)} not initialized", this);
            }
        }
#endif
    }
}