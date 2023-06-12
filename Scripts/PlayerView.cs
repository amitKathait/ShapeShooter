using UnityEngine;

namespace ShapeShooter
{
    public class PlayerView : MonoBehaviour
    {
        [SerializeField] private float speed;
        [SerializeField] private Transform initialMissilePosition;
        [SerializeField] private GameObject missileObject;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private float offsetY;
        
        public int MissileFired = 0;
        
        //Player movement area
        private float playerWidth;
        private float playerHeight;
        private float xMin, xMax;
        private float yMin, yMax;
        
        private void Start()
        {
            InitBounds();
            InitPlayer();
            GameManager.GameManagerInstance.OnLevelChangedEvent += OnLevelComplete;
            GameManager.GameManagerInstance.OnPlayerHitEvent += OnPlayerHit;
        }
        
        //set player position to left center of screen
        private void InitPlayer()
        {
            var screenPos = new Vector3(Screen.safeArea.xMax / 4.0f, Screen.safeArea.yMax / 2.0f, 0);
            var worldPos = Camera.main.ScreenToWorldPoint(screenPos);
            worldPos.z = 0;
            transform.position = worldPos;
        }
        
        //init visible area for player
        private void InitBounds()
        {
            playerHeight = spriteRenderer.bounds.size.y / 2;
            playerWidth = spriteRenderer.bounds.size.x / 2;
            var cam = Camera.main;
            var camHeight = cam.orthographicSize;
            var camWidth = cam.orthographicSize * cam.aspect;
            
            yMin = -camHeight + playerHeight;
            yMax = camHeight - playerHeight - offsetY;
            xMin = -camWidth + playerWidth;
            xMax = camWidth - playerWidth;
        }

        private void OnPlayerHit()
        {
            InitPlayer();
        }
        
        private void Update()
        {
            CheckPlayerMovements();
            CheckPlayerBullets();
        }
        
        /// <summary>
        /// Move player and clamp position if player is moving out of visible area
        /// </summary>
        private void CheckPlayerMovements()
        {
            float posX = Input.GetAxis("Horizontal");
            float posY = Input.GetAxis("Vertical");
            Vector3 pos = new Vector2(posX, posY).normalized * (Time.deltaTime * speed);
            var xValidPos = Mathf.Clamp(transform.position.x + pos.x, xMin, xMax);
            var yValidPos = Mathf.Clamp(transform.position.y + pos.y, yMin, yMax);
            transform.position = new Vector3(xValidPos, yValidPos, 0);
        }
        
        private void CheckPlayerBullets()
        {
            if (!GameManager.GameManagerInstance.IsMultiPurposeScreenOpen && Input.GetKeyDown(KeyCode.Space))
            {
                SpawnBullet();
            }
        }
        
        private void SpawnBullet()
        {
            MissileFired++;
            var go = Instantiate(missileObject, GameManager.GameManagerInstance.MissileHolder);
            go.GetComponent<MissileView>().SetMissileDirection(false, MissileType.Straight, initialMissilePosition.position);
        }

        //On level complete reset player
        private void OnLevelComplete(int level, bool isReset)
        {
            InitPlayer();
            if (isReset)
            {
                MissileFired = 0;
            }
        }

        private void OnDestroy()
        {
            GameManager.GameManagerInstance.OnLevelChangedEvent -= OnLevelComplete;
            GameManager.GameManagerInstance.OnPlayerHitEvent -= OnPlayerHit;
        }
    }
}