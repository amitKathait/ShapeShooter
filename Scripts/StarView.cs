using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ShapeShooter
{
    public class StarView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private float scaleSpeed;
        [SerializeField] private float movementSpeed;
        [SerializeField] private float maxScale;
        [SerializeField] private float startDelay;
        
        private Vector3 originalPos;
        private Coroutine coroutine;
        private float xMin, xMax;
        private float yMin, yMax;
        
        private void Start()
        {
            InitBounds();
            GameManager.GameManagerInstance.OnLevelChangedEvent += OnLevelChanged;
            originalPos = transform.position;
            coroutine = StartCoroutine(ScaleStar());
        }
        
        /// <summary>
        /// Initialised visible range in screen
        /// </summary>
        private void InitBounds()
        {
            var cam = Camera.main;
            var camHeight = cam.orthographicSize;
            var camWidth = cam.orthographicSize * cam.aspect;

            yMin = -camHeight;
            yMax = camHeight ;
            xMax = camWidth;
            xMin = -camWidth;
        }

        /// <summary>
        /// Scales star continuesly
        /// </summary>
        /// <returns></returns>
        private IEnumerator ScaleStar()
        {
            yield return null;
            yield return new WaitForSeconds(startDelay);
            while (true)
            {
                Vector3 scale = new Vector3((Mathf.Sin(Time.time * startDelay)*maxScale) + maxScale, (Mathf.Sin(Time.time * startDelay)*maxScale) + maxScale, 0);
                transform.localScale = scale;
                transform.Translate(Vector3.left * (Time.deltaTime * movementSpeed));
                yield return null;
            }
        }

        //On level changes reset start to intitial position
        private void OnLevelChanged(int level, bool isReset)
        {
            transform.position = originalPos;
        }
        
        //Once all movement is done check if star is within visible area if not reset its position
        private void LateUpdate()
        {
            var pos = transform.position;
            if (pos.x > xMax || pos.x < xMin || pos.y > yMax || pos.y < yMin)
            {
                ResetStar();
            }
        }

        //Reset star position to right most
        private void ResetStar()
        {
            var yPos = Random.Range(yMin, yMax - 1);
            transform.position = new Vector3(xMax - 1, yPos, 0);
        }

        private void OnDestroy()
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
                coroutine = null;
            }
            GameManager.GameManagerInstance.OnLevelChangedEvent -= OnLevelChanged;
        }
    }
}