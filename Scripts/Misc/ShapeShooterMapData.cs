using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ShapeShooter
{
    /// <summary>
    /// This Class hold data regarding all the level details and gives flexibility to increase or decrease number of levels
    /// or enemy Hp
    /// </summary>
    [CreateAssetMenu (fileName = "ShapeShooterMapData", menuName = "ScriptableObjects/ShapeShooterMapData", order = 1)]
    public class ShapeShooterMapData : ScriptableObject
    {
        public List<MissileTypeToLevelMap> missileTypeToLevelMaps;
        public List<MissileTextureMap> missileTextureMaps;
        public List<int> EnemyHealthPerLevel;
        
        private Dictionary<int, List<MissileTypeData>> levelDataDict;
        private Dictionary<MissileType, Sprite> missileTypeSpriteDict;
        
        public void InitMissileDataDict()
        {
            if (levelDataDict == null)
            {
                levelDataDict = new Dictionary<int, List<MissileTypeData>>();
            }
            else
            {
                levelDataDict.Clear();
            }

            levelDataDict = missileTypeToLevelMaps.ToDictionary(x => x.Level,
                x => x.MissileTypeDatas);

            if (missileTypeSpriteDict == null)
            {
                missileTypeSpriteDict = new Dictionary<MissileType, Sprite>();
            }
            else
            {
                missileTypeSpriteDict.Clear();
            }

            missileTypeSpriteDict = missileTextureMaps.ToDictionary(x => x.MissileType, x => x.MissileSprite);
        }
        
        public MissileType GetMissileByType(float health, int CurrentLevel)
        {
            if (levelDataDict != null && levelDataDict.ContainsKey(CurrentLevel))
            {
                List<MissileTypeData> map = levelDataDict[CurrentLevel];
                for (int i = map.Count -1; i >=0 ; i--)
                {
                    if (map[i].HealthThreshold >= health)
                    {
                        return map[i].MissileType;
                    }
                }
                return MissileType.Straight;
            }
            return MissileType.Straight;
        }

        public Sprite GetSpriteByType(MissileType type)
        {
            if (missileTypeSpriteDict.ContainsKey(type))
            {
                return missileTypeSpriteDict[type];
            }

            return null;
        }

        public int GetEnemyHealthByLevel(int level)
        {
            if ((level-1) < EnemyHealthPerLevel.Count)
            {
                return EnemyHealthPerLevel[level - 1];
            }

            return EnemyHealthPerLevel[EnemyHealthPerLevel.Count - 1];
        }
    }
    
    [Serializable]
    public class MissileTypeData
    {
        public MissileType MissileType;
        public float HealthThreshold;
    }

    [Serializable]
    public class MissileTypeToLevelMap
    {
        public int Level;
        public List<MissileTypeData> MissileTypeDatas;
    }
        
    [Serializable]
    public class MissileTextureMap
    {
        public MissileType MissileType;
        public Sprite MissileSprite;
    }
}