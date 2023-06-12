using System.Collections.Generic;
using UnityEngine;

namespace ShapeShooter
{
    public class HealthManager : MonoBehaviour
    {
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private GameObject healthObject;

        private List<HealthView> healthViews = new List<HealthView>();
        private float healthPercent;
        private int currentLevel;
        private int hitCount;
        
        private void Start()
        {
            GameManager.GameManagerInstance.OnLevelChangedEvent += OnLevelChanged; //subscribe to necessary event
            GameManager.GameManagerInstance.OnEnemyHitEvent += OnEnemyHit;
            OnLevelChanged(1, true); //Initially level will be 1
        }

        /// <summary>
        /// Decides HP level of the enemy 
        /// </summary>
        /// <param name="level">current level</param>
        /// <param name="isReset">is new game</param>
        private void OnLevelChanged(int level, bool isReset)
        {
            currentLevel = level;
            hitCount = 0;
            var count = ResetHealth(level);
            var healthCount = GameManager.MapData.GetEnemyHealthByLevel(level);
            for (int i = count; i < healthCount; i++) //Spawn new health if in case not already preset
            {
                var go = Instantiate(healthObject, spawnPoint);
                var comp = go.GetComponent<HealthView>();
                healthViews.Add(comp);
            }
        }

        /// <summary>
        /// Reset the health of enemy based on object pooling
        /// </summary>
        /// <param name="level">current level</param>
        /// <returns></returns>
        private int ResetHealth(int level)
        {
            var currentHealthCount = healthViews.Count;
            var requiredHealthCount = GameManager.MapData.GetEnemyHealthByLevel(level);
            var finalCount = requiredHealthCount > currentHealthCount ? currentHealthCount : requiredHealthCount;
            if (currentHealthCount > 0)
            {
                var extra = currentHealthCount - requiredHealthCount; //if health preset is more then required then destroy unnecessary health
                if (extra > 0)
                {
                    for (int i = 0; i < extra; i++)
                    {
                        var view = healthViews[i];
                        healthViews.Remove(view);
                        Destroy(view.gameObject);
                    }
                }
                for (int i = 0; i < finalCount; i++) //Reset remaining health 
                {
                    healthViews[i].ResetHealth();
                }
            }

            return finalCount;
        }

        /// <summary>
        /// Whenever enemy is hit this method decide the enemy hp and notifies GameManager if level is completed
        /// or requires to change enemy missile type
        /// </summary>
        private void OnEnemyHit()
        {
            if (healthViews.Count > 0 && hitCount < healthViews.Count)
            {
                var healthView = healthViews[hitCount];
                healthView.StartFading();
                var healthRemaining = ((float)(healthViews.Count - hitCount - 1) / healthViews.Count) * 100f;
                GameManager.GameManagerInstance.OnHealthUpdated(healthRemaining);
            }
            hitCount++;
            if (hitCount == healthViews.Count)
            {
                GameManager.GameManagerInstance.OnLevelCompleted();
            }
        }

        private void OnDestroy()
        {
            GameManager.GameManagerInstance.OnLevelChangedEvent -= OnLevelChanged;
            GameManager.GameManagerInstance.OnEnemyHitEvent -= OnEnemyHit;
        }
    }
}