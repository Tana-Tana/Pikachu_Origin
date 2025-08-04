using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Tile tile1, tile2 = null; // hai tile duoc chon de match

    private void Update() {
        if (Input.GetMouseButtonDown(0) && (GameManager.Instance.IsState(GameState.GAME_PLAY) || GameManager.Instance.IsState(GameState.FREEZE_TIME))) // kiem tra su kien click chuot trai va co dang o trong gamePlay hay khong
        {
            Tile tileClicked = LevelManager.Instance.Level.GetTileClicked(Input.mousePosition);
            if (CanTileBeMatched(tileClicked)) // co tile dang duoc select, khong phai cai dang matching, khong phai chuong ngai vat
            {
                SoundManager.Instance.PlayFx(FxID.SELECTION_ITEM);
                SetStatusTileToMatch(ref tile1, ref tile2, tileClicked);
            }
        }
    }

    private bool CanTileBeMatched(Tile tileClicked)
    {
        if (tileClicked != null && tileClicked.gameObject.activeSelf && !tileClicked.IsMatching &&
            (int)tileClicked.TileData.eTypeTile < 1000) // co tile dang duoc select, khong phai cai dang matching, khong phai chuong ngai vat
        {
            return true;
        }

        return false;
    }

    private void SetStatusTileToMatch(ref Tile beginTile, ref Tile endTile, Tile tileClicked)
    {
        if (beginTile == null || !beginTile.gameObject.activeSelf || beginTile.IsMatching) // neu chua co o nao duoc chon hoac da co nhung trong qua trinh matching
        {
            beginTile = tileClicked;
            beginTile.OnSelect(); // danh dau tile1 la da duoc chon
        }
        else
        {
            if (tileClicked != beginTile)
            {
                if (tileClicked.TileData.eTypeTile == beginTile.TileData.eTypeTile)
                {
                    endTile = tileClicked;
                    endTile.OnSelect(); // danh dau tile2 la da duoc chon

                    LevelManager.Instance.Level.OnMatching(ref beginTile, ref endTile); // goi ham match hai tile
                }
                else
                {
                    beginTile.OnDeselect();
                    beginTile = tileClicked;
                    beginTile.OnSelect(); // danh dau tile1 la da duoc chon
                }
            }
        }
    }
}