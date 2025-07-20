using _Game.Scripts.Tile;
using UnityEngine;

namespace _Game.Scripts.ScriptableObject
{
    [CreateAssetMenu(fileName = "new Tile Database", menuName = "ScriptableObjects/TileDatabase", order = 1)]
    public class TileDatabase : UnityEngine.ScriptableObject
    {
        [SerializeField] private DataTile[] tiles;
        public DataTile[] Tiles => tiles;
    }
}
