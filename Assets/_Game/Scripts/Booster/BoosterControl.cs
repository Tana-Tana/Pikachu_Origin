using _Game.Extensions.DP;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Game.Scripts.Booster
{
    public class BoosterControl : Singleton<BoosterControl>
    {
        public void ShuffleArray(int rows, int columns, Tile.Tile[,] tiles)
        {
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
        
        public void RemoveTile(Tile.Tile tile1, Tile.Tile tile2)
        {
            
        }
        
        public void AddTime(float time)
        {
            
        }
    }
}
