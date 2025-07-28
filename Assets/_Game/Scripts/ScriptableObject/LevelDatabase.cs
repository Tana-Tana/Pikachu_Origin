using _Game.Scripts.Level;
using UnityEngine;

namespace _Game.Scripts.ScriptableObject
{
    [CreateAssetMenu(fileName = "New LevelDatabase", menuName = "ScriptableObjects/LevelDatabase")]
    public class LevelDatabase : UnityEngine.ScriptableObject
    {
        [SerializeField] private LevelData[] levelData = null; // Danh sách dữ liệu các level
        public LevelData[] LevelData => levelData;
    }
}
