namespace Project
{
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class GameOverHandler : MonoBehaviour
    {
        [SerializeField]
        private Unlockable _winUnlockable;
        
        [SerializeField]
        private int _winUnlockableLevel = 0;
        
        private Structure _playerStructure;
        private Structure _enemyStructure;

        private CameraMover _cameraMover;
        private TimeSpeedHandler _timeSpeedHandler;
        private ServiceButtonPanel _serviceButtonPanel;
        private GameOverMenu _gameOverMenu;
        private Progress _progress;
        
        private bool _isInitialized = false;

        public void Init(
            PlayerStructure playerStructure,
            IEnemyStructure enemyStructure,
            CameraMover cameraMover,
            TimeSpeedHandler timeSpeedHandler,
            ServiceButtonPanel serviceButtonPanel,
            GameOverMenu gameOverMenu,
            Progress progress) 
        {
            _playerStructure = playerStructure.Structure;
            _enemyStructure = enemyStructure.Structure;

            _cameraMover = cameraMover;
            _timeSpeedHandler = timeSpeedHandler;
            _serviceButtonPanel = serviceButtonPanel;
            _gameOverMenu = gameOverMenu;
            _progress = progress;

            _playerStructure.Death += EnemyWin;
            _enemyStructure.Death += PlayerWin;

            _isInitialized = true;
        }
        
        /*
        private void OnGUI()
        {
            if (GUI.Button(new Rect(0,0, 100, 50), "PlayerWin"))
            {
                PlayerWin();
            }
            
            if (GUI.Button(new Rect(0,50, 100, 50), "EnemyWin"))
            { 
                EnemyWin();
            }
        }
        */


        private void EnemyWin()
        {
            _timeSpeedHandler.StopTime();
            _cameraMover.LockAt(_playerStructure.Position, 2);
            _serviceButtonPanel.Hide();
            
            _gameOverMenu.ShowEnemyWin();
        }

        private void PlayerWin()
        {
            _timeSpeedHandler.StopTime();
            _cameraMover.LockAt(_enemyStructure.Position, 2);
            _serviceButtonPanel.Hide();
            
            _progress.Unlock(_winUnlockable);
            _progress.UnlockLevel(_winUnlockableLevel);

            _gameOverMenu.ShowPlayerWin();
        }

#if UNITY_EDITOR
        private void Start()
        {
            if (!_isInitialized)
            {
                Debug.LogError($"{nameof(GameOverHandler)} not initialized", this);
            }
        }
    }
#endif
}