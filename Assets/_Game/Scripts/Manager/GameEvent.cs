using System.Collections.Generic;
using _Game.Extensions.DP;
using _Game.Scripts.Booster;
using _Game.Scripts.Tile;
using UnityEngine;

namespace _Game.Scripts.Manager
{
    public class GameEvent : Singleton<GameEvent>
    {
        private Dictionary<TypeTile,List<Tile.Tile>> tilesInCurrentLevel; // luu cac tile theo typeTile trong level hien tai

        public void OnInit(int rows, int cols, Tile.Tile[,] tilesActive)
        {
            tilesInCurrentLevel = new Dictionary<TypeTile, List<Tile.Tile>>();
            
            for (int i = 1; i <= rows; ++ i)
            {
                for (int j = 1; j<= cols; ++j)
                {
                    if (tilesInCurrentLevel.ContainsKey(tilesActive[i, j].Data.typeTile)) // neu da ton tai typeTile trong dictionary
                    {
                        tilesInCurrentLevel[tilesActive[i, j].Data.typeTile].Add(tilesActive[i, j]);
                    }
                    else if((int)tilesActive[i,j].Data.typeTile < 1000) // neu chua ton tai typeTile trong dictionary va khac vat can
                    {
                        tilesInCurrentLevel.Add(tilesActive[i, j].Data.typeTile, new List<Tile.Tile> { tilesActive[i, j]});
                    }
                }
            }
            
            //Debug.Log(tilesInCurrentLevel.Count + " typeTile in current level");
        }

        public void RemoveTile(Tile.Tile tile1, Tile.Tile tile2)
        {
            //Debug.Log(tile1 == null);
            //Debug.Log(tile2 == null);
            foreach (var tiles in tilesInCurrentLevel)
            {
                if (tiles.Key == tile1.Data.typeTile)
                {
                    tilesInCurrentLevel[tiles.Key].Remove(tile1);
                    tilesInCurrentLevel[tiles.Key].Remove(tile2);
                    break;
                }
            }
        }

        public bool CheckNeedShuffle()
        {
            Vector2Int pointMatchOne = Vector2Int.zero; // diem match thu nhat
            Vector2Int pointMatchTwo = Vector2Int.zero; // diem match thu hai
            foreach(var tiles in tilesInCurrentLevel)
            {
                for (int i = 0; i < tiles.Value.Count; i++) // lay tung phan tu thuoc dictionary
                {
                    for (int j = i + 1; j < tiles.Value.Count; j++) // lay cac phan tu con lai de so sanh
                    {
                        if (TileManager.Instance.CanMatch(new Vector2Int(tiles.Value[i].X, tiles.Value[i].Y), new Vector2Int(tiles.Value[j].X, tiles.Value[j].Y), ref pointMatchOne, ref pointMatchTwo))
                        {
                            Debug.Log("Con nuoc di o tile: " + tiles.Value[i].name + " va tile: " + tiles.Value[j].name);
                            BoosterControl.Instance.SetHint(tiles.Value[i], tiles.Value[j]);
                            return false; // neu co 2 tile co the match duoc thi khong can shuffle
                        }
                    }
                }
            }
            return true;
        }

        public bool CheckWinGame()
        {
            foreach (var tiles in tilesInCurrentLevel)
            {
                if (tiles.Value.Count > 0) // neu co tile nao con ton tai trong level
                {
                    return false; // chua thang
                }
            }

            return true;
        }

        public void OnTimeUp()
        {
            Debug.Log("OnTimeUp --> LOSE");
            GameManager.Instance.ChangeState(GameState.LOSE);
        }
    }
}
