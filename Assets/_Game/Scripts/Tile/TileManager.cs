using System.Collections;
using _Game.Extensions;
using _Game.Extensions.DP;
using _Game.Scripts.Booster;
using _Game.Scripts.Camera;
using _Game.Scripts.Effect;
using _Game.Scripts.LineRender;
using _Game.Scripts.Manager;
using _Game.Scripts.ScriptableObject;
using _Game.Scripts.Timer;
using UnityEngine;

namespace _Game.Scripts.Tile
{
    public class TileManager : Singleton<TileManager>
    {
        
        [Header("Data")] [SerializeField] private TileDatabase tileDatabase; // database chua thong tin ve tile

        [Header("Board Settings")] [SerializeField]
        private Transform boardTransform; // transform cua board noi ma tile se duoc spawn

        [SerializeField] private Tile tilePrefab; // prefab cua tile

        private int rows = 4; // so hang cua board
        public int Rows => rows;

        private int columns = 4; // so cot cua board
        public int Columns => columns;

        private float tileSize = 1f; // kich thuoc cua tile

        [Header("Line")] [SerializeField] private GameObject lineParent;

        private Tile[,] tiles; // mang luu tru cac tile dang duoc su dung
        public Tile[,] Tiles => tiles;
        
        public void OnInit() // khoi tao board voi so hang va cot
        {
            // random hang cot
            int cnt = 0;
            while (true)
            {
                rows = (int)Random.Range(4, 11);
                columns = (int)Random.Range(4, 8);

                if ((rows * columns) % 2 == 0) break;
                
                ++cnt;
                if (cnt > 10)
                {
                    rows = 10;
                    columns = 7;
                    break;
                }
            }
            
            tiles = new Tile[rows + 2,
                columns + 2]; // khoi tao mang tileActives voi kich thuoc lon hon so hang va cot de tranh bi out of index khi truy cap

            SpawnTileOnBoard(tilePrefab, boardTransform, tileSize); // spawn tile tren board

            GiveDataToBoard(); // dua du lieu vao board
            
            GameEvent.Instance.OnInit(rows, columns, tiles); // khoi tao su kien ban dau cho game
            
            CameraControl.Instance.FitCameraToBoard(tiles[1, 1].TF.position,
                tiles[rows, columns].TF.position); // canh chinh camera theo board

            BoosterControl.Instance.Shuffle(); // shuffle lan dau
            GameEvent.Instance.CheckNeedShuffle(); // kiem tra xem co can shuffle hay khong
        }

        public void OnDespawn()
        {
            if (tiles != null)
            {
                foreach (var tile in tiles)
                {
                    if (tile != null)
                    {
                        Destroy(tile.gameObject);
                    }
                }
            }
        }

        private void GiveDataToBoard()
        {
            if (columns % 2 == 0)
            {
                for (int i = 1; i <= rows; i++) // đưa dữ liệu theo dạng cặp đôi
                {
                    for (int j = 1; j <= columns; j += 2)
                    {
                        // lay tile tu database
                        DataTile dataTile =
                            Utilities.TakeRandom(tileDatabase.Tiles); // lay ngau nhien 1 tile tu database

                        tiles[i, j].OnInit();
                        tiles[i, j].SetData(dataTile); // dua du lieu vao tile

                        tiles[i, j + 1].OnInit();
                        tiles[i, j + 1].SetData(dataTile); // dua du lieu vao tile ke tiep
                    }
                }
            }
            else
            {
                for (int i = 1; i <= columns; i++) // đưa dữ liệu theo dạng cặp đôi
                {
                    for (int j = 1; j <= rows; j += 2)
                    {
                        // lay tile tu database
                        DataTile dataTile =
                            Utilities.TakeRandom(tileDatabase.Tiles); // lay ngau nhien 1 tile tu database
                        
                        tiles[j, i].OnInit();
                        tiles[j, i].SetData(dataTile); // dua du lieu vao tile
                        
                        tiles[j + 1, i].OnInit();
                        tiles[j + 1, i].SetData(dataTile); // dua du lieu vao tile ke tiep
                    }
                }
            }
        }

        private void SpawnTileOnBoard(Tile prefab, Transform parent, float cellSize)
        {
            // spawn tile tren board
            for (int x = 0; x <= rows + 1; x++)
            {
                for (int y = 0; y <= columns + 1; y++)
                {
                    Vector3 position = GetTilePosition(x, y, cellSize);
                    Tile tile = Instantiate(prefab, position, Quaternion.identity, parent);

                    tile.DeActiveGameObject();
                    tile.SetName($"Tile_{x}_{y}");
                    tile.SetData(new DataTile(TypeTile.ROAD, null));
                    tiles[x, y] = tile; // luu tile vao mang

                    tiles[x, y].SetLocationInMatrix(x, y); // luu toa do cua diem hien tai
                }
            }
        }

        private Vector3 GetTilePosition(int row, int column, float cellSize) // lay vi tri cua tile dua tren hang va cot
        {
            float x = column * cellSize;
            float y = row * cellSize;
            return new Vector3(x, y, 0f);
        }

        public Tile GetTileClicked(Vector3 mousePosition)
        {
            // ban 1 tia raycast tu vi tri chuot den cac tile
            RaycastHit2D hit = Physics2D.Raycast(CameraControl.Instance.MainCamera.ScreenToWorldPoint(mousePosition),
                Vector2.zero);
            //Debug.Log(hit.collider.name);
            if (hit.collider != null) // neu co va cham voi collider
            {
                for (int i = 1; i <= rows; ++i)
                {
                    for (int j = 1; j <= columns; ++j)
                    {
                        if (tiles[i, j] != null &&
                            hit.collider.gameObject ==
                            tiles[i, j].gameObject) // kiem tra xem collider co phai la tile dang duoc click
                        {
                            return tiles[i, j]; // tra ve tile dang duoc click
                        }
                    }
                }
            }

            return null;
        }

        public void OnMatching(ref Tile tile1, ref Tile tile2)
        {
            tile1.SetActiveMatching();
            tile2.SetActiveMatching();

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
                    OnDraw(2, new Vector2Int[] { startPoint, endPoint });
                }
                else if (pointMatchTwo == Vector2Int.zero && pointMatchOne != Vector2Int.zero)
                {
                    OnDraw(3, new Vector2Int[] { startPoint, pointMatchOne, endPoint });
                }
                else
                {
                    OnDraw(4, new Vector2Int[] { startPoint, pointMatchOne, pointMatchTwo, endPoint });
                }
                
                StartCoroutine(IECoroutineMatchTile(startPoint, endPoint));
            }
            else
            {
                Debug.Log("Khong the match duoc  voi nhau --> set lai mau");
                tile1.SetDeActiveMatching();
                tile2.SetDeActiveMatching();

                tile1.SetDeActiveSelected();
                tile1 = tile2;
                tile1.SetActiveSelected();
            }
        }
        
        public bool CanMatch(Vector2Int startPoint, Vector2Int endPoint, ref Vector2Int pointMatchOne, ref Vector2Int pointMatchTwo)
        {
            if ((int)tiles[startPoint.x, startPoint.y].Data.typeTile >= 1000 || (int)tiles[endPoint.x, endPoint.y].Data.typeTile >= 1000)
            {
                return false;
            }
            
            return CanTwoPointMatching(startPoint, endPoint) ||
                   CheckHavePathWithTwoLine(startPoint, endPoint, ref pointMatchOne) ||
                   CheckHavePathWithThreeLine(startPoint, endPoint, ref pointMatchOne, ref pointMatchTwo);
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
            for (Vector2Int point = startPoint + new Vector2Int(1, 0);
                 point.x <= upCoordinate.x;
                 point += new Vector2Int(1, 0)) // noi theo huong phia tren
            {
                // voi moi diem, duyet voi 4 huong xem co the di toi dau
                Vector2Int upCoordinatePoint = GetCoordinateTileUpCanMatch(point);
                Vector2Int downCoordinatePoint = GetCoordinateTileDownCanMatch(point);
                Vector2Int leftCoordinatePoint = GetCoordinateTileLeftCanMatch(point);
                Vector2Int rightCoordinatePoint = GetCoordinateTileRightCanMatch(point);

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
                // voi moi diem, duyet voi 4 huong xem co the di toi dau
                Vector2Int upCoordinatePoint = GetCoordinateTileUpCanMatch(point);
                Vector2Int downCoordinatePoint = GetCoordinateTileDownCanMatch(point);
                Vector2Int leftCoordinatePoint = GetCoordinateTileLeftCanMatch(point);
                Vector2Int rightCoordinatePoint = GetCoordinateTileRightCanMatch(point);

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
                // voi moi diem, duyet voi 4 huong xem co the di toi dau
                Vector2Int upCoordinatePoint = GetCoordinateTileUpCanMatch(point);
                Vector2Int downCoordinatePoint = GetCoordinateTileDownCanMatch(point);
                Vector2Int leftCoordinatePoint = GetCoordinateTileLeftCanMatch(point);
                Vector2Int rightCoordinatePoint = GetCoordinateTileRightCanMatch(point);

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
                // voi moi diem, duyet voi 4 huong xem co the di toi dau
                Vector2Int upCoordinatePoint = GetCoordinateTileUpCanMatch(point);
                Vector2Int downCoordinatePoint = GetCoordinateTileDownCanMatch(point);
                Vector2Int leftCoordinatePoint = GetCoordinateTileLeftCanMatch(point);
                Vector2Int rightCoordinatePoint = GetCoordinateTileRightCanMatch(point);

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
            tiles[startPoint.x, startPoint.y].SetDoneTask();
            tiles[endPoint.x, endPoint.y].SetDoneTask();

            tiles[startPoint.x, startPoint.y].OnDespawn();
            tiles[endPoint.x, endPoint.y].OnDespawn();
            
            SimplePool.Spawn<EffectMatching>(PoolType.EFFECT_MATCH, new Vector3(startPoint.y, startPoint.x, 0),
                Quaternion.identity).OnInit(0.5f);
            SimplePool.Spawn<EffectMatching>(PoolType.EFFECT_MATCH, new Vector3(endPoint.y, endPoint.x, 0),
                Quaternion.identity).OnInit(0.5f);

            GameEvent.Instance.RemoveTile(tiles[startPoint.x, startPoint.y], tiles[endPoint.x, endPoint.y]);
            yield return new WaitForSeconds(0.5f);

            if (GameEvent.Instance.CheckNeedShuffle()) // neu khong con tile nao co the match duoc
            {
                if (!GameEvent.Instance.CheckWinGame())
                {
                    Debug.Log("Shuffle khi khong con nuoc di");
                    BoosterControl.Instance.ShuffleArray(); // shuffle lai
                }
                else
                {
                    Debug.Log("Win game!");
                    if (GameManager.Instance.Progress == GameManager.MAX_LEVEL_OF_CLASSIC_MODE)
                    {
                        Debug.Log("Win all levels");
                        GameManager.Instance.ChangeState(GameState.WIN); // neu da thang thi chuyen trang thai sang WIN
                    }
                    else
                    {
                        Debug.Log("Next Level + anim");
                        GameManager.Instance.PlayGameOnClassicMode(); // neu chua win thi chuyen sang level tiep theo
                    }
                }
            }
        }

        private Vector2Int GetCoordinateTileRightCanMatch(Vector2Int point)
        {
            int idx = point.y;
            for (int i = point.y + 1; i <= columns + 1; i++) // duyet theo x ve phia ben phai cua diem
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
            for (int i = point.x + 1; i <= rows + 1; ++i) // duyet theo hang ve phia ben tren cua diem
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

        private Vector2Int GetCoordinateTile(ref Tile tile)
        {
            for (int i = 1; i <= rows; ++i)
            {
                for (int j = 1; j <= columns; ++j)
                {
                    if (tiles[i, j] == tile) // kiem tra xem tile co phai la tile dang duoc click
                    {
                        return new Vector2Int(i, j); // tra ve toa do cua tile
                    }
                }
            }

            return Vector2Int.zero; // neu khong tim thay tile, tra ve toa do (0,0)
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private void OnDraw(int numberPoint, Vector2Int[] pointsPos)
        {
            SimplePool.Spawn<LineDrawer>(PoolType.LINE_MATCH, lineParent.transform.position,
                    lineParent.transform.rotation)
                .DrawLine(numberPoint, pointsPos);
        }
    }
}