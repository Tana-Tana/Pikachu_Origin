using _Game.Extensions.DP;
using _Game.Scripts.Tile;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Game.Scripts.Booster
{
    public class BoosterControl : Singleton<BoosterControl>
    {
        private int rows;
        private int columns;
        private Tile.Tile[,] tiles;
        
        [ContextMenu("Shuffle Array")]
        public void ShuffleArray()
        {
            this.rows = TileManager.Instance.Rows;
            this.columns = TileManager.Instance.Columns;
            this.tiles = TileManager.Instance.Tiles;
            
            for (int i = 1; i <= rows; ++i)
            {
                for (int j = 1; j <= columns; ++j)
                {
                    int randomRow = Random.Range(1, rows + 1);
                    int randomColumn = Random.Range(1, columns + 1);
                    // Hoán đổi vị trí của tileActives[i, j] với tileActives[randomRow, randomColumn]
                    (tiles[i,j].TF.position, tiles[randomRow, randomColumn].TF.position) = (tiles[randomRow,randomColumn].TF.position,tiles[i,j].TF.position);
                    (tiles[i, j], tiles[randomRow, randomColumn]) = (tiles[randomRow, randomColumn], tiles[i, j]);
                }
            }
            
            Debug.Log("Shuffle complete!");
            for (int i = 1; i <= rows; ++i)
            {
                for (int j = 1; j <= columns; ++j)
                {
                    tiles[i,j].SetName($"Tile_{i}_{j}"); // dat ten cho tile de de dang quan ly
                    tiles[i,j].SetLocationInMatrix(i,j);
                }
            }
        }
        
        [ContextMenu("Remove Tile")]
        public void RemoveTile()
        {
            
        }
        
        [ContextMenu("Add Time")]
        public void AddTime()
        {
            
        }

        [ContextMenu("Freeze Time")]
        public void FreezeTime()
        {
            
        }
    }
}
