using UnityEngine;

namespace ShapeShooter
{
    public class MissileView : MonoBehaviour
    {
        [SerializeField] private float speed;
        [SerializeField] private SpriteRenderer spriteRenderer;
        
        private Vector3 movementVector;
        private MissileType missileType;
        private float cycle; 
        public float amplitude = 2f; 
        public float frequency = 4f;
        private Vector3 posA;
        private Vector3 posB;
        public float trajectoryAmplitude;
        public float trajectoryFrequency;
        private float missileWidth;
        private float missileHeight;
        private float xMin, xMax;
        private float yMin, yMax;

        private void Start()
        {
            InitBounds();
        }

        /// <summary>
        /// Initialise Missile
        /// </summary>
        /// <param name="isEnemyMissile">is missile fired by enemy or player</param>
        /// <param name="missileType">missile type</param>
        /// <param name="position">spawn position</param>
        public void SetMissileDirection(bool isEnemyMissile, MissileType missileType, Vector3 position)
        {
            transform.position = position;
            this.missileType = missileType;
            if (missileType == MissileType.Homing)
            {
                gameObject.tag = MissileType.Homing.ToString();
                speed /= 2;
            }
           
            spriteRenderer.sprite = GameManager.MapData.GetSpriteByType(missileType); //Setup missile sprite by missile type
            posA = transform.position;
            posB = GameManager.GameManagerInstance.GetPlayerPosition();
            var normalised = (posB - posA).normalized;
            movementVector = isEnemyMissile ? normalised : Vector3.right;
        }

        //initialised visible area
        private void InitBounds()
        {
            missileHeight = spriteRenderer.bounds.size.y / 2;
            missileWidth = spriteRenderer.bounds.size.x / 2;
            var cam = Camera.main;
            var camHeight = cam.orthographicSize;
            var camWidth = cam.orthographicSize * cam.aspect;
            
            yMin = -camHeight - missileHeight; //sprite height and width has been substracted or added to prevent 
            yMax = camHeight + missileHeight;
            xMin = -camWidth - missileWidth;
            xMax = camWidth + missileWidth;
        }

        private void Update()
        {
            MoveBullet();
        }

        private void MoveBullet()
        {
            if (missileType == MissileType.Straight )
            {
                transform.Translate(movementVector * (speed * Time.deltaTime));
            }
            else if (missileType == MissileType.Homing)
            {
                var playerPos = GameManager.GameManagerInstance.GetPlayerPosition();
                movementVector = (playerPos - posA).normalized;
                MoveInSinWaveWithParam(frequency, amplitude, movementVector);
            }
            else if (missileType == MissileType.Trajectory)
            {
                MoveInSinWaveWithParam(trajectoryFrequency, trajectoryAmplitude, movementVector);
            }
        }

        /// <summary>
        /// Move object along sine wave
        /// </summary>
        /// <param name="frequency">number of wave frequency</param>
        /// <param name="amplitude">height of wave</param>
        /// <param name="movementVector">direction </param>
        private void MoveInSinWaveWithParam(float frequency, float amplitude, Vector3 movementVector)
        {
            cycle += Time.deltaTime * frequency;
            transform.position = posA + Vector3.up * (amplitude * Mathf.Sin(cycle));
            posA += movementVector*(Time.deltaTime * speed);
        }
        
        private void OnCollisionEnter2D(Collision2D col)
        {
            if (col.gameObject.tag.Equals("Player"))
            {
                GameManager.GameManagerInstance.OnPlayerHit();
                Destroy(gameObject);
            }
            else if (col.gameObject.tag.Equals("Enemy"))
            {
                GameManager.GameManagerInstance.OnEnemyHit();
                Destroy(gameObject);
            }
            else if (col.gameObject.tag.Equals(MissileType.Homing.ToString()))
            {
                Destroy(col.gameObject);
                Destroy(gameObject);
            }
        }

        //once all movement is done check if object is out of visible area
        private void LateUpdate()
        {
            var pos = transform.position;
            if (pos.x > xMax || pos.x < xMin || pos.y > yMax || pos.y < yMin)
            {
                Destroy(gameObject);
            }
        }
    }
}