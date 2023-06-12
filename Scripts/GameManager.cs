using System;
using System.Collections;
using UnityEngine;

namespace ShapeShooter
{
    public enum MissileType
    {
        Straight,
        Homing,
        Trajectory
    }
    
    public enum ScreenType
    {
        None,
        Intro,
        LevelUp,
        GameOver,
        GamePaused,
        Congrats
    }
    
    public class GameManager : MonoBehaviour
    {
        //Creating a singleton to share info between scripts 
        public static GameManager GameManagerInstance;
        
        [SerializeField] private GameObject multiPurposeScreenPrefab;
        [SerializeField] private PlayerView playerView;
        [SerializeField] private ShapeShooterMapData shapeShooterMapData;
        
        public static ShapeShooterMapData MapData; //Scriptable object singleton object to fetch level info at any time
        
        public Transform MissileHolder;
        
        public int CurrentLevel;
        
        //Events to inform any game related changes
        /// <summary>
        /// Informs enemy has been hit
        /// </summary>
        public event Action OnEnemyHitEvent;
        
        /// <summary>
        /// informs player has been hit
        /// </summary>
        public event Action OnPlayerHitEvent;
        /// <summary>
        /// Notifies Level changes to other classes and if level has been reset to 1 by boolean
        /// </summary>
        public event Action<int, bool> OnLevelChangedEvent;
        
        /// <summary>
        /// Inform enemy to update its missile type based on HP remaining
        /// </summary>
        public event Action<MissileType> EnemyMissileUpdateEvent;
        
        public bool IsMultiPurposeScreenOpen;
        private GameObject multiPurposeScreenObject;
        public ScreenType ScreenType = ScreenType.None;
        private GameObject congratsScreenObject;
        
        /// <summary>
        /// initialise singleton object and scriptable object
        /// </summary>
        private void Awake()
        {
            shapeShooterMapData.InitMissileDataDict();
            MapData = shapeShooterMapData;
            GameManagerInstance = this;
        }

        /// <summary>
        /// At very first start of game show intro screen
        /// </summary>
        private void Start()
        {
            ShowMultiPurposeScreenWithType(ScreenType.Intro);
        }
        
        public void OnEnemyHit()
        {
            OnEnemyHitEvent?.Invoke();
        }

        /// <summary>
        /// When level is completed show level up screen show level or congrats screen based on max level setuped
        /// </summary>
        public void OnLevelCompleted()
        {
            DestroyMissile();
            if (CurrentLevel < MapData.EnemyHealthPerLevel.Count)
            {
                ShowMultiPurposeScreenWithType(ScreenType.LevelUp);
                CurrentLevel++;
                OnHealthUpdated(100.0f);
                OnLevelChangedEvent?.Invoke(CurrentLevel, false);
            }
            else
            {
                ShowScreen(ScreenType.Congrats);
            }
        }

        /// <summary>
        /// Method to handle screen with buttons
        /// </summary>
        /// <param name="type"></param>
        private void ShowScreen(ScreenType type)
        {
            DestroyMultiPurposeScreen();
            ShowMultiPurposeScreenWithType(type);
            multiPurposeScreenObject.GetComponent<MultiPurposeScreenView>().InitScreen(playerView.MissileFired,
                () =>
                {
                    DestroyMultiPurposeScreen();
                    DestroyMissile();
                    CurrentLevel = 1;
                    OnLevelChangedEvent?.Invoke(CurrentLevel, true);
                    OnHealthUpdated(100f);
                });
        }

        public void OnPlayerHit()
        {
            OnPlayerHitEvent?.Invoke();
        }

        public void OnGameOver()
        {
           ShowScreen(ScreenType.GameOver);
        }

        /// <summary>
        /// Set game paused state
        /// </summary>
        /// <param name="isPaused">is game paused</param>
        private void SetPauseState(bool isPaused)
        {
            if (isPaused)
            {
                ShowMultiPurposeScreenWithType(ScreenType.GamePaused);
            }
            else
            {
                DestroyMultiPurposeScreen();
            }
        }

        /// <summary>
        /// Whenever a screen is open pause game in the back and spawn screen
        /// </summary>
        /// <param name="screenType">screen to be spawned</param>
        private void ShowMultiPurposeScreenWithType(ScreenType screenType)
        {
            Time.timeScale =  0;
            IsMultiPurposeScreenOpen = true;
            ScreenType = screenType;
            multiPurposeScreenObject = Instantiate(multiPurposeScreenPrefab);
        }

        /// <summary>
        /// Destroys MultiPurpose screen and resume game behind
        /// </summary>
        private void DestroyMultiPurposeScreen()
        {
            Destroy(multiPurposeScreenObject);
            multiPurposeScreenObject = null;
            ScreenType = ScreenType.None;
            Time.timeScale =  1;
            StartCoroutine(DelayToAcceptPlayerClicks());
        }

        /// <summary>
        /// Waits for 1 frame to accept input from player when multi popup is closed
        /// </summary>
        /// <returns></returns>
        private IEnumerator DelayToAcceptPlayerClicks()
        {
            yield return null;
            IsMultiPurposeScreenOpen = false;
        }

        /// <summary>
        /// Destroy all missile presents in screen
        /// </summary>
        private void DestroyMissile()
        {
            foreach (Transform child in MissileHolder)
            {
                Destroy(child.gameObject);
            }
        }
        
        private void Update()
        {
            //input checks
            bool canCheckForButtonClicks = ScreenType != ScreenType.None && ScreenType != ScreenType.GameOver && ScreenType != ScreenType.Congrats;
            if (canCheckForButtonClicks)
            {
                if (ScreenType == ScreenType.GamePaused && Input.GetKeyDown(KeyCode.Escape))
                {
                    SetPauseState(!IsMultiPurposeScreenOpen);
                }
                else if (Input.GetKeyDown(KeyCode.Space))
                {
                    DestroyMultiPurposeScreen();
                }
            }
        }

        public Vector3 GetPlayerPosition()
        {
            return playerView.transform.position;
        }
        
        /// <summary>
        /// Called whenever enemy is hit by a missile and decide enemy next Missile type
        /// </summary>
        /// <param name="health"></param>
        public void OnHealthUpdated(float health)
        {
            var type = MapData.GetMissileByType(health, CurrentLevel);
            EnemyMissileUpdateEvent?.Invoke(type);
        }
    }
}

