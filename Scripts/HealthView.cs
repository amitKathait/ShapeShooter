using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ShapeShooter
{
    public class HealthView : MonoBehaviour
    {
        [SerializeField] private Image image;
        [SerializeField] private float fadeSpeed;

        private Coroutine coroutine;
        public void StartFading()
        {
            coroutine = StartCoroutine(FadeImage());
        }

        /// <summary>
        /// fade alpha of image slowly whenever enemy is been hit
        /// </summary>
        /// <returns></returns>
        private IEnumerator FadeImage()
        {
            var alpha = image.color.a;
            var target = 0.0f;
            for (int i = 0; i < 3; i++) //blink thrice
            {
                for (var t = 0.0f; t < 1.0f; t += Time.deltaTime / fadeSpeed) //until we reached close to targeted alpha
                {
                    var newColor = new Color(image.color.r, image.color.g, image.color.b, Mathf.Lerp(alpha, target, t));
                    image.color = newColor;
                    yield return null;
                }
                (alpha, target) = (target, alpha);
            }

        }

        /// <summary>
        /// Reset alpha of image
        /// </summary>
        public void ResetHealth()
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
                coroutine = null;
            }
            image.color =  new Color(image.color.r, image.color.g, image.color.b, 1.0f);;
        }
    }
}