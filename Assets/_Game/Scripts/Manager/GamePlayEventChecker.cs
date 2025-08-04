using System.Collections.Generic;
using _Game.Extensions.DP;
using UnityEngine;


public class GamePlayEventChecker : MonoBehaviour
{
    private Dictionary<ETypeTile, List<Tile>> tilesInCurrentLevel = new(); // luu cac tile theo typeTile trong level hien tai

    public void OnInit()
    {
        tilesInCurrentLevel.Clear();

        for (int i = 1; i <= LevelManager.Instance.GetRows; ++i)
        {
            for (int j = 1; j <= LevelManager.Instance.GetColumns; ++j)
            {
                if (!LevelManager.Instance.GetTiles[i, j].IsDone) // neu nhung o do chua duoc hoan thanh
                {
                    if (tilesInCurrentLevel.ContainsKey(LevelManager.Instance.GetTiles[i, j].GetTypeData)) // neu da ton tai typeTile trong dictionary
                    {
                        tilesInCurrentLevel[LevelManager.Instance.GetTiles[i, j].GetTypeData].Add(LevelManager.Instance.GetTiles[i, j]);
                    }
                    else if ((int)LevelManager.Instance.GetTiles[i, j].GetTypeData < 1000) // neu chua ton tai typeTile trong dictionary va khac vat can
                    {
                        tilesInCurrentLevel.Add(LevelManager.Instance.GetTiles[i, j].GetTypeData, new List<Tile> { LevelManager.Instance.GetTiles[i, j] });
                    }
                }
            }
        }

        //Debug.Log(tilesInCurrentLevel.Count + " typeTile in current level");
    }

    public void RemoveTile(Tile tile1, Tile tile2)
    {
        //Debug.Log(tile1 == null);
        //Debug.Log(tile2 == null);
        foreach (var tiles in tilesInCurrentLevel)
        {
            if (tiles.Key == tile1.TileData.eTypeTile)
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
        Vector2Int coord1 = Vector2Int.zero;
        Vector2Int coord2 = Vector2Int.zero;
        foreach (var tiles in tilesInCurrentLevel)
        {
            for (int i = 0; i < tiles.Value.Count; i++) // lay tung phan tu thuoc dictionary
            {
                for (int j = i + 1; j < tiles.Value.Count; j++) // lay cac phan tu con lai de so sanh
                {
                    coord1.x = tiles.Value[i].X;
                    coord1.y = tiles.Value[i].Y;
                    coord2.x = tiles.Value[j].X;
                    coord2.y = tiles.Value[j].Y;

                    if (LevelManager.Instance.Level.CanMatch(coord1, coord2, ref pointMatchOne, ref pointMatchTwo))
                    {
                        Debug.Log("Con nuoc di o tile: " + tiles.Value[i].name + " va tile: " + tiles.Value[j].name);
                        BoosterManager.Instance.SetHint(tiles.Value[i], tiles.Value[j]);
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
}
