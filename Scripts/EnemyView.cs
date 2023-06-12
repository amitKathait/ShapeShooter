using UnityEngine;

namespace ShapeShooter
{
    public class EnemyView : MonoBehaviour
    {
        [SerializeField] private float speed;
        [SerializeField] private GameObject missileObject;
        [SerializeField] private Transform initialMissilePosition;
        
        [Header("Enemy Movement")]
        [SerializeField] private float posAxOffset;
        [SerializeField] private float posAyOffset;
        [SerializeField] private float posBxOffset;
        [SerializeField] private float posByOffset;
        
        [Header("Auto fire missile")]
        [SerializeField] private float delayToSpawnMissile;
        [SerializeField] private float repeatRate;

        private Vector3 positionA;
        private Vector3 positionB;
        private MissileType currentMissileType = MissileType.Straight;
        
        private void Start()
        {
            GameManager.GameManagerInstance.EnemyMissileUpdateEvent += OnEnemyMissileUpdateEvent;
            SetPositionForMovements();
            InvokeRepeating("SpawnMissile", delayToSpawnMissile, repeatRate); //spawning missile at every interval of repeatRate
        }

        /// <summary>
        /// Change missile type whenever health is updated
        /// </summary>
        /// <param name="missileType">missile type</param>
        private void OnEnemyMissileUpdateEvent(MissileType missileType)
        {
            currentMissileType = missileType;
        }

        public void SpawnMissile()
        {
            var go = Instantiate(missileObject, GameManager.GameManagerInstance.MissileHolder);
            go.GetComponent<MissileView>().SetMissileDirection(true, currentMissileType, initialMissilePosition.position);
        }

        private void Update()
        {
            MoveEnemy();
        }

        /// <summary>
        /// Calculating enemy movement points 
        /// </summary>
        private void SetPositionForMovements()
        {
            var screenCords = Screen.safeArea;
            var posAInScreenSpace = new Vector3(screenCords.xMax- posAxOffset, screenCords.yMax - posAyOffset, 0);
            var posBInScreenSpace = new Vector3(screenCords.xMax - posBxOffset, screenCords.yMin - posByOffset, 0);
            positionA = Camera.main.ScreenToWorldPoint(posAInScreenSpace);
            positionA.z = 0f;
            positionB = Camera.main.ScreenToWorldPoint(posBInScreenSpace);
            positionB.z = 0f;
        }

        private void MoveEnemy()
        {
            transform.position = Vector3.Lerp (positionA, positionB, Mathf.PingPong(Time.time*speed, 1.0f));
        }

        private void OnDestroy()
        {
            GameManager.GameManagerInstance.EnemyMissileUpdateEvent -= OnEnemyMissileUpdateEvent;
        }
    }
}