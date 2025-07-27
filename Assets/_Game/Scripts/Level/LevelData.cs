using UnityEngine;

namespace _Game.Scripts.Level
{
    [System.Serializable]
    public class LevelData
    {
        [Header("Level Information")]
        [SerializeField] private ETypeLevel typeLevel = ETypeLevel.LEVEL_0; // phan biet voi cac level khac
        public ETypeLevel TypeLevel => typeLevel;
        
        [SerializeField] private string nameLevel = "Level 0"; // dung de load ten len UI
        public string NameLevel => nameLevel;
        
        [SerializeField] private float timeLevel = 0f; // thoi gian choi cua level
        public float TimeLevel => timeLevel;
        
        [Header("Level Prefab")]
        [SerializeField] private Level levelPrefab = null; // man choi
        public Level LevelPrefab => levelPrefab;
    }

    public enum ETypeLevel
    {
        LEVEL_0 = 0,
        LEVEL_1 = 1,
        LEVEL_2 = 2,
        LEVEL_3 = 3,
        LEVEL_4 = 4,
        LEVEL_5 = 5,
        LEVEL_6 = 6,
    }
}
