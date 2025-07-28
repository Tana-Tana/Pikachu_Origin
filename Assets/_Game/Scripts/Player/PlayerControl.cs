using _Game.Scripts.Tile;
using UnityEngine;

namespace _Game.Scripts.Player
{
	public class PlayerControl : MonoBehaviour
	{
		private Tile.Tile tile1, tile2; // hai tile duoc chon de match
    
		private void Update() {
			if (Input.GetMouseButtonDown(0)) // kiem tra su kien click chuot trai
			{
				Tile.Tile tileClicked =  TileManager.Instance.GetTileClicked(Input.mousePosition);
				if (CanTileBeMatched(tileClicked)) // co tile dang duoc select, khong phai cai dang matching, khong phai chuong ngai vat
				{
					//Debug.Log(tileClicked.gameObject.GetInstanceID());
					SetStatusTileToMatch(ref tile1, ref tile2, tileClicked);
				}
			}
		}

		private bool CanTileBeMatched(Tile.Tile tileClicked)
		{
			if (tileClicked != null && tileClicked.gameObject.activeSelf && !tileClicked.IsMatching &&
			    (int)tileClicked.Data.typeTile < 1000) // co tile dang duoc select, khong phai cai dang matching, khong phai chuong ngai vat
			{
				return true;
			}

			return false;
		}

		private void SetStatusTileToMatch(ref Tile.Tile beginTile, ref Tile.Tile endTile, Tile.Tile tileClicked)
		{
			if (beginTile == null || !beginTile.gameObject.activeSelf || beginTile.IsMatching) // neu chua co o nao duoc chon
			{
				beginTile = tileClicked;
				beginTile.SetActiveSelected(); // danh dau tile1 la da duoc chon
			}
			else
			{
				if (tileClicked != beginTile)
				{
					if (tileClicked.Data.typeTile == beginTile.Data.typeTile)
					{
						endTile = tileClicked;
						endTile.SetActiveSelected(); // danh dau tile2 la da duoc chon
				
						TileManager.Instance.OnMatching(ref beginTile, ref endTile); // goi ham match hai tile
					}
					else
					{
						beginTile.SetDeActiveSelected();
						beginTile = tileClicked;
						beginTile.SetActiveSelected(); // danh dau tile1 la da duoc chon
					}
				}
			}
		}
	}
}
