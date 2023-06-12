using TMPro;
using UnityEngine;

namespace ShapeShooter
{
    public class PayerLifeView : MonoBehaviour
    {
        private const string LIFE_FORMAT = "x {0}";

        [SerializeField] private TextMeshProUGUI lifeLeftText;
        [SerializeField] private int totalLife;

        private int remainingLife ;
        
        private void Start()
        {
            remainingLife = totalLife;
            GameManager.GameManagerInstance.OnPlayerHitEvent += OnPlayerHit;
            GameManager.GameManagerInstance.OnLevelChangedEvent += OnLevelChanged;
        }

        /// <summary>
        /// Resets or update player HP based on level
        /// </summary>
        /// <param name="level"></param>
        /// <param name="isReset"></param>
        private void OnLevelChanged(int level, bool isReset)
        {
            if (isReset)
            {
                remainingLife = totalLife;
            }
            SetText();
        }

        /// <summary>
        /// Whenever player has been hit decides to updated text or game is over
        /// </summary>
        private void OnPlayerHit()
        {
            if (--remainingLife <= 0)
            {
                SetText();
                GameManager.GameManagerInstance.OnGameOver();
            }
            else
            {
                SetText();
            }
        }

        private void SetText()
        {
            lifeLeftText.SetText(string.Format(LIFE_FORMAT, remainingLife < 0 ? 0 : remainingLife));
        }

        private void OnDestroy()
        {
            GameManager.GameManagerInstance.OnPlayerHitEvent -= OnPlayerHit;
        }
    }
}