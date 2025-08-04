using System.Collections;
using _Game.Scripts.Effect;
using UnityEngine;

public class Level : MonoBehaviour
{
    [SerializeField] private Tile tilePrefab; // prefab cua tile
    private readonly float tileSize = 1f; // kich thuoc cua 1 tile
    private LevelData levelData = null; // du lieu cua level hien tai
    private Tile[,] tiles = new Tile[10, 10]; // mang luu tru cac tile dang duoc su dung

    public void OnInit()
    {
        for (int i = 1; i < levelData.Rows + 1; i++)
        {
            for (int j = 1; j < levelData.Columns + 1; j++)
            {
                tiles[i, j].OnInit();
            }
        }
    }

    public void OnDespawn()
    {
        if (levelData == null || tiles == null) return;

        for (int i = 0; i < levelData.Rows + 2; i++)
        {
            for (int j = 0; j < levelData.Columns + 2; j++)
            {
                tiles[i, j].OnDespawn();
            }
        }
    }

    public void OnLoadLevel(int levelIndex)
    {
        levelData = DataManager.Instance.GetLevelData(levelIndex); // lay level data tu DataManager
        //CheckIndexMatrix();

        SpawnTileOnBoard(tilePrefab, transform, tileSize); // sinh tile tren board
        GamePlayManager.Instance.CameraController.FitCameraToBoard(Vector3.zero, new Vector3(levelData.Columns + 1, levelData.Rows + 1, 0));
        
    }

// lay o duoc click hien tai
    public Tile GetTileClicked(Vector3 mousePosition)
    {
        // ban 1 tia raycast tu vi tri chuot den cac tile
        RaycastHit2D hit = Physics2D.Raycast(GamePlayManager.Instance.CameraController.MainCamera.ScreenToWorldPoint(mousePosition),
            Vector2.zero);
        //Debug.Log(hit.collider.name);
        if (hit.collider != null) // neu co va cham voi collider
        {
            for (int i = 1; i <= levelData.Rows; ++i)
            {
                for (int j = 1; j <= levelData.Columns; ++j)
                {
                    if (tiles[i, j] != null && hit.collider.gameObject == tiles[i, j].gameObject) // kiem tra xem collider co phai la tile dang duoc click
                    {
                        return tiles[i, j]; // tra ve tile dang duoc click
                    }
                }
            }
        }

        return null;
    }

    #region Check matching to match
    public void OnMatching(ref Tile tile1, ref Tile tile2)
    {
        Vector2Int startPoint = GetCoordinateTile(ref tile1); // lay toa do cua tile1
        Vector2Int endPoint = GetCoordinateTile(ref tile2); // lay toa do cua tile2

        //Debug.Log(startPoint + " " + endPoint);
        if (startPoint == Vector2Int.zero || endPoint == Vector2Int.zero)
        {
            Debug.LogError("Tile not found in the board!");
            return;
        }

        Vector2Int pointMatchOne = Vector2Int.zero; // diem match thu nhat
        Vector2Int pointMatchTwo = Vector2Int.zero; // diem match thu hai
        if (CanMatch(startPoint, endPoint, ref pointMatchOne, ref pointMatchTwo))
        {
            if (pointMatchOne == Vector2Int.zero && pointMatchTwo == Vector2Int.zero)
            {
                EffectManager.Instance.OnDraw(2, new Vector2Int[] { startPoint, endPoint });
            }
            else if (pointMatchTwo == Vector2Int.zero && pointMatchOne != Vector2Int.zero)
            {
                EffectManager.Instance.OnDraw(3, new Vector2Int[] { startPoint, pointMatchOne, endPoint });
            }
            else
            {
                EffectManager.Instance.OnDraw(4, new Vector2Int[] { startPoint, pointMatchOne, pointMatchTwo, endPoint });
            }

            StartCoroutine(IECoroutineMatchTile(startPoint, endPoint));
        }
        else
        {
            Debug.Log("Khong the match duoc  voi nhau --> set lai mau");

            tile1.OnDeselect();
            tile1 = tile2;
            tile1.OnSelect();
            SoundManager.Instance.PlayFx(FxID.CAN_NOT_MATCH);
        }
    }

    public bool CanMatch(Vector2Int startPoint, Vector2Int endPoint, ref Vector2Int pointMatchOne, ref Vector2Int pointMatchTwo)
    {
        if ((int)tiles[startPoint.x, startPoint.y].TileData.eTypeTile >= 1000 || (int)tiles[endPoint.x, endPoint.y].TileData.eTypeTile >= 1000)
        {
            return false;
        }

        return CanTwoPointMatching(startPoint, endPoint) ||
               CheckHavePathWithTwoLine(startPoint, endPoint, ref pointMatchOne) ||
               CheckHavePathWithThreeLine(startPoint, endPoint, ref pointMatchOne, ref pointMatchTwo);
    }

    public Vector2Int GetCoordinateTile(ref Tile tile)
    {
        for (int i = 1; i <= levelData.Rows; ++i)
        {
            for (int j = 1; j <= levelData.Columns; ++j)
            {
                if (tiles[i, j] == tile) // kiem tra xem tile co phai la tile dang duoc click
                {
                    return new Vector2Int(i, j); // tra ve toa do cua tile
                }
            }
        }

        return Vector2Int.zero; // neu khong tim thay tile, tra ve toa do (0,0)
    }

    private bool CheckHavePathWithThreeLine(Vector2Int startPoint, Vector2Int endPoint,
        ref Vector2Int pointMatchOne, ref Vector2Int pointMatchTwo)
    {
        // kiem tra xung quanh diem startPoint co the noi max toi dau
        Vector2Int upCoordinate = GetCoordinateTileUpCanMatch(startPoint);
        Vector2Int downCoordinate = GetCoordinateTileDownCanMatch(startPoint);
        Vector2Int leftCoordinate = GetCoordinateTileLeftCanMatch(startPoint);
        Vector2Int rightCoordinate = GetCoordinateTileRightCanMatch(startPoint);

        // doi voi cac diem phia tren cua startPoint
        for (Vector2Int point = startPoint + new Vector2Int(1, 0); point.x <= upCoordinate.x;
            point += new Vector2Int(1, 0)) // noi theo huong phia tren
        {
            // voi moi huong se kiem tra co noi duoc tu point sang endPoint khong
            bool canMatch = CheckHavePathWithTwoLine(point, endPoint, ref pointMatchTwo);
            if (canMatch)
            {
                pointMatchOne = point;
                return true;
            }
        }

        // doi voi cac diem phia duoi cua startPoint
        for (Vector2Int point = startPoint + new Vector2Int(-1, 0);
             point.x >= downCoordinate.x;
             point += new Vector2Int(-1, 0)) // noi theo huong phia duoi
        {
            // voi moi huong se kiem tra co noi duoc tu point sang endPoint khong
            bool canMatch = CheckHavePathWithTwoLine(point, endPoint, ref pointMatchTwo);
            if (canMatch)
            {
                pointMatchOne = point;
                return true;
            }
        }

        // doi voi cac diem ben trai
        for (Vector2Int point = startPoint + new Vector2Int(0, -1);
             point.y >= leftCoordinate.y;
             point += new Vector2Int(0, -1)) // noi theo huong phia trai
        {
            // voi moi huong se kiem tra co noi duoc tu point sang endPoint khong
            bool canMatch = CheckHavePathWithTwoLine(point, endPoint, ref pointMatchTwo);
            if (canMatch)
            {
                pointMatchOne = point;
                return true;
            }
        }

        // doi voi cac diem ben phai
        for (Vector2Int point = startPoint + new Vector2Int(0, 1);
             point.y <= rightCoordinate.y;
             point += new Vector2Int(0, 1)) // noi theo huong phia phai
        {
            // voi moi huong se kiem tra co noi duoc tu point sang endPoint khong
            bool canMatch = CheckHavePathWithTwoLine(point, endPoint, ref pointMatchTwo);
            if (canMatch)
            {
                pointMatchOne = point;
                return true;
            }
        }

        return false;
    }

    private bool CheckHavePathWithTwoLine(Vector2Int startPoint, Vector2Int endPoint, ref Vector2Int pointToCheck)
    {
        // kiem tra xung quanh diem startPoint co the noi max toi dau
        Vector2Int upCoordinate = GetCoordinateTileUpCanMatch(startPoint);
        Vector2Int downCoordinate = GetCoordinateTileDownCanMatch(startPoint);
        Vector2Int leftCoordinate = GetCoordinateTileLeftCanMatch(startPoint);
        Vector2Int rightCoordinate = GetCoordinateTileRightCanMatch(startPoint);

        // kiem tra xem endPoint co noi duoc toi cac diem thuoc duong thang co gioi han tren khong
        for (Vector2Int point = startPoint + new Vector2Int(1, 0);
             point.x <= upCoordinate.x;
             point += new Vector2Int(1, 0)) // noi theo huong phia tren
        {
            if (CanTwoPointMatching(point, endPoint)) // neu co the noi duoc den endPoint
            {
                pointToCheck = new Vector2Int(point.x, point.y);
                return true;
            }
        }

        for (Vector2Int point = startPoint + new Vector2Int(-1, 0);
             point.x >= downCoordinate.x;
             point += new Vector2Int(-1, 0)) // noi theo huong phia duoi
        {
            if (CanTwoPointMatching(point, endPoint)) // neu co the noi duoc den endPoint
            {
                pointToCheck = new Vector2Int(point.x, point.y);
                return true;
            }
        }

        for (Vector2Int point = startPoint + new Vector2Int(0, -1);
             point.y >= leftCoordinate.y;
             point += new Vector2Int(0, -1)) // noi theo huong phia trai
        {
            if (CanTwoPointMatching(point, endPoint)) // neu co the noi duoc den endPoint
            {
                pointToCheck = new Vector2Int(point.x, point.y);
                return true;
            }
        }

        for (Vector2Int point = startPoint + new Vector2Int(0, 1);
             point.y <= rightCoordinate.y;
             point += new Vector2Int(0, 1)) // noi theo huong phia phai
        {
            if (CanTwoPointMatching(point, endPoint)) // neu co the noi duoc den endPoint
            {
                pointToCheck = new Vector2Int(point.x, point.y);
                return true;
            }
        }

        return false;
    }

    private bool CanTwoPointMatching(Vector2Int startPoint, Vector2Int endPoint)
    {
        //Debug.Log(startPoint + " " + endPoint);
        // kiem tra xung quanh diem startPoint co the noi max toi dau
        Vector2Int upCoordinate = GetCoordinateTileUpCanMatch(startPoint);
        Vector2Int downCoordinate = GetCoordinateTileDownCanMatch(startPoint);
        Vector2Int leftCoordinate = GetCoordinateTileLeftCanMatch(startPoint);
        Vector2Int rightCoordinate = GetCoordinateTileRightCanMatch(startPoint);

        if (upCoordinate + new Vector2Int(1, 0) == endPoint || downCoordinate + new Vector2Int(-1, 0) == endPoint
                                                            || leftCoordinate + new Vector2Int(0, -1) == endPoint ||
                                                            rightCoordinate + new Vector2Int(0, 1) == endPoint)
        {
            return true;
        }

        return false;
    }

    private IEnumerator IECoroutineMatchTile(Vector2Int startPoint, Vector2Int endPoint)
    {
        tiles[startPoint.x, startPoint.y].OnMatch();
        tiles[endPoint.x, endPoint.y].OnMatch();
        yield return new WaitForSeconds(0.5f);
        SoundManager.Instance.PlayFx(FxID.MATCH);
        EffectManager.Instance.OnEffectMatching(new Vector3(startPoint.y, startPoint.x, 0), new Vector3(endPoint.y, endPoint.x));

        // sau khi ma match xong thi remove khoi list hint
        GamePlayManager.Instance.GameEvent.RemoveTile(tiles[startPoint.x, startPoint.y], tiles[endPoint.x, endPoint.y]);
        if (GameManager.Instance.IsState(GameState.HINT)) 
        {
            if (GamePlayManager.Instance.TimeController.IsFreezeTime)
            {
                GameManager.Instance.ChangeState(GameState.FREEZE_TIME);
            }
            else
            {
                GameManager.Instance.ChangeState((GameState.GAME_PLAY));
            }
        }

        // check winGame
        if (!GamePlayManager.Instance.OnCheckWinGame()) 
        {
            if (GamePlayManager.Instance.OnCheckShuffle()) // neu shuffle thi shuffle
            {
                GameManager.Instance.ChangeState(GameState.SHUFFLE);
                BoosterManager.Instance.OnBooster(ETypeBooster.SHUFFLE);
            }
            else
            {
                // Debug.Log("Van con nuoc di! Chua can shuffle");
            }
        }
        else
        {
            GameManager.Instance.ChangeState(GameState.WIN_GAME);
            GamePlayManager.Instance.OnWinGame();
            UIManager.Instance.GetUI<CanvasGamePlay>().SetActiveOverlayUI();
        }
    }
#endregion

    #region Check 4 huong cua o hien tai de phuc vu cho viec match

    private Vector2Int GetCoordinateTileRightCanMatch(Vector2Int point)
    {
        int idx = point.y;
        for (int i = point.y + 1; i <= levelData.Columns + 1; i++) // duyet theo x ve phia ben phai cua diem
        {
            if (!tiles[point.x, i].gameObject.activeSelf) // neu con di duoc
            {
                idx = i;
            }
            else
            {
                break;
            }
        }

        return new Vector2Int(point.x, idx);
    }

    private Vector2Int GetCoordinateTileLeftCanMatch(Vector2Int point)
    {
        int idx = point.y;
        for (int i = point.y - 1; i >= 0; --i) // duyet theo cot ve phia ben trai cua diem
        {
            if (!tiles[point.x, i].gameObject.activeSelf) // neu con di duoc thi di
            {
                idx = i;
            }
            else
            {
                break;
            }
        }

        return new Vector2Int(point.x, idx);
    }

    private Vector2Int GetCoordinateTileDownCanMatch(Vector2Int point)
    {
        int idx = point.x;
        for (int i = point.x - 1; i >= 0; --i) // duyet theo hang ve phia ben duoi cua diem
        {
            if (!tiles[i, point.y].gameObject.activeSelf) // neu con duong di thi luu lai
            {
                idx = i;
            }
            else
            {
                break;
            }
        }

        return new Vector2Int(idx, point.y);
    }

    private Vector2Int GetCoordinateTileUpCanMatch(Vector2Int point)
    {
        int idx = point.x;
        for (int i = point.x + 1; i <= levelData.Rows + 1; ++i) // duyet theo hang ve phia ben tren cua diem
        {
            if (!tiles[i, point.y].gameObject.activeSelf) // neu con duong di thi luu lai
            {
                idx = i;
            }
            else
            {
                break;
            }
        }

        return new Vector2Int(idx, point.y);
    }

#endregion

    #region sinh ra cac o moi level

    private void SpawnTileOnBoard(Tile prefab, Transform parent, float cellSize) // sinh tile tren board
    {
        Vector3 position = Vector3.zero; // vi tri cua tile
        int countValuesToAssign = 0; // bien dem gia tri dung de load data cho tile
        int numberOfType = 0; // bien dem loai tile da duoc gan

        for (int i = 0; i <= levelData.Rows + 1; i++)
        {
            for (int j = 0; j <= levelData.Columns + 1; j++)
            {
                position = GetTilePosition(i, j, cellSize);
                Tile tile = Instantiate(prefab, position, Quaternion.identity, parent); // tao clone tile tai vi tri moi tinh

                if (i == 0 || i == levelData.Rows + 1 || j == 0 || j == levelData.Columns + 1) // neu la duong vien
                {
                    tile.SetTileData(new TileData(ETypeTile.ROAD, null)); // lay gia tri mac dinh
                }
                else
                {
                    numberOfType = levelData.ValuesToAssign[countValuesToAssign++]; // lay gia tri tu level data
                    tile.SetTileData(DataManager.Instance.GetTileData(numberOfType)); // lay tile data tu DataManager roi gan vao tile
                }

                tile.SetDeActiveObject();
                tile.SetName($"Tile_{i}_{j}");
                tile.SetPosInMatrix(i,j);
                tiles[i, j] = tile; // luu tile vao mang
            }
        }
    }

    private Vector3 GetTilePosition(int row, int column, float cellSize) // lay vi tri cua tile dua tren hang va cot
    {
        float x = column * cellSize;
        float y = row * cellSize;
        return new Vector3(x, y, 0f);
    }
    
#endregion

    #region Getters 

    public Tile[,] Tiles => tiles;
    public LevelData LevelData => levelData;

#endregion

#if UNITY_EDITOR
    private void CheckIndexMatrix()
    {
        if (levelData.Rows + 2 > 10 || levelData.Columns + 2 > 10)
        {
            Debug.LogError("levelIndex out of range");
        }
    }
#endif
}
