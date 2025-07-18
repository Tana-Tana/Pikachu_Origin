using System.Collections;
using _Game.Extensions;
using _Game.Extensions.DP;
using _Game.Scripts.Booster;
using _Game.Scripts.Camera;
using _Game.Scripts.LineRender;
using _Game.Scripts.ScriptableObject;
using UnityEngine;

namespace _Game.Scripts.Tile
{
    public class TileManager : Singleton<TileManager>
    {
        [Header("Data")]
        [SerializeField] private TileDatabase tileDatabase; // database chua thong tin ve tile
        
        [Header("Board Settings")]
        [SerializeField] private Transform boardTransform; // transform cua board noi ma tile se duoc spawn
        [SerializeField] private Tile tilePrefab; // prefab cua tile
        [SerializeField] private int rows = 4; // so hang cua board
        [SerializeField] private int columns = 4; // so cot cua board
        private float tileSize = 1f; // kich thuoc cua tile

        [Header("Line")] [SerializeField] private GameObject lineParent;
        
        private Tile[,] tiles; // mang luu tru cac tile dang duoc su dung
        
        private void Start()
        {
            OnInit(ref rows, ref columns);
        }

        private void OnInit(ref int rows, ref int columns) // khoi tao board voi so hang va cot
        {
            if ((rows * columns) % 2 != 0)
            {
                Debug.LogError("So phan tu cua board phai la so chan!");
                return;
            }

            rows = Mathf.Min(12, rows); // gioi han so hang toi da la 12
            columns = Mathf.Min(7, columns); // gioi han so cot toi da la 7
            
            tiles = new Tile[rows+2, columns+2]; // khoi tao mang tileActives voi kich thuoc lon hon so hang va cot de tranh bi out of index khi truy cap
         	
    		SpawnTileOnBoard(tilePrefab, boardTransform, tileSize); // spawn tile tren board
            
            GiveDataToBoard(); // dua du lieu vao board
            
            CameraControl.Instance.FitCameraToBoard(tiles[1, 1].TF.position,
                tiles[rows, columns].TF.position); // canh chinh camera theo board
            
            BoosterControl.Instance.ShuffleArray(rows, columns, tiles); // shuffle lan dau
            
        }
        
        private void OnDespawn()
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
                for (int i=1;i<=rows;i++) // đưa dữ liệu theo dạng cặp đôi
                {
                    for (int j=1;j<=columns;j+=2)
                    {
                        // lay tile tu database
                        TileData tileData = Utilities.TakeRandom(tileDatabase.Tiles); // lay ngau nhien 1 tile tu database
                        
                        tiles[i,j].ActiveGameObject();
                        tiles[i, j].OnInit(tileData); // dua du lieu vao tile
                        
                        tiles[i,j+1].ActiveGameObject();
                        tiles[i, j + 1].OnInit(tileData); // dua du lieu vao tile ke tiep
                    }
                } 
            }
            else
            {
                for (int i=1;i<=columns;i++) // đưa dữ liệu theo dạng cặp đôi
                {
                    for (int j=1;j<=rows;j+=2)
                    {
                        // lay tile tu database
                        TileData tileData = Utilities.TakeRandom(tileDatabase.Tiles); // lay ngau nhien 1 tile tu database
                        
                        tiles[j,i].ActiveGameObject();
                        tiles[j,i].OnInit(tileData); // dua du lieu vao tile
                        
                        tiles[j+1,i].ActiveGameObject();
                        tiles[j+1, i].OnInit(tileData); // dua du lieu vao tile ke tiep
                    }
                } 
            }
        }

        private void SpawnTileOnBoard(Tile prefab, Transform parent, float cellSize)
        {
            // spawn tile tren board
            for (int x=0; x <= rows+1; x++)
            {
                for (int y=0; y <= columns+1; y++)
                {
                    Vector3 position = GetTilePosition(x, y, cellSize);
                    Tile tile = Instantiate(prefab, position, Quaternion.identity, parent);
                    
                    tile.DeActiveGameObject();
                    tile.SetName($"Tile_{x}_{y}");
                    tile.OnInit(new TileData(TileType.ROAD, null));
                    tiles[x, y] = tile; // luu tile vao mang
                    
                    tiles[x,y].SetLocationInMatrix(x,y); // luu toa do cua diem hien tai
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
            RaycastHit2D hit = Physics2D.Raycast(CameraControl.Instance.MainCamera.ScreenToWorldPoint(mousePosition), Vector2.zero);
            //Debug.Log(hit.collider.name);
            if (hit.collider != null) // neu co va cham voi collider
            {
                for (int i = 1; i <= rows; ++i)
                {
                    for (int j = 1; j <= columns; ++j)
                    {
                        if (tiles[i,j] != null && hit.collider.gameObject == tiles[i, j].gameObject) // kiem tra xem collider co phai la tile dang duoc click
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
            // de tam, sau de o cho khac
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
            
            bool canMatch = CanTwoPointMatching(startPoint, endPoint); // bien kiem tra xem co the ve duong thang giua hai tile hay khong
            //Debug.Log(canDrawOneLine);
            if (canMatch) 
            {
                OnDrawWithOneLine(tiles[startPoint.x, startPoint.y].SpriteRenderer.bounds.center, tiles[endPoint.x, endPoint.y].SpriteRenderer.bounds.center);
                StartCoroutine(IECoroutineMatchTile(startPoint, endPoint));
            }
            else
            {
                canMatch = CheckHavePathWithTwoLine(startPoint, endPoint); // kiem tra xem voi 2 duong thang co the ve duoc duong noi 2 tile khong
                if (canMatch)
                {
                    StartCoroutine(IECoroutineMatchTile(startPoint, endPoint));
                }
                else
                {
                    canMatch = CheckHavePathWithThreeLine(startPoint, endPoint);

                    if (canMatch)
                    {
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
            }
        }

        private bool CheckHavePathWithThreeLine(Vector2Int startPoint, Vector2Int endPoint)
        {
            // kiem tra xung quanh diem startPoint co the noi max toi dau
            Vector2Int upCoordinate = GetCoordinateTileUpCanMatch(startPoint);
            Vector2Int downCoordinate = GetCoordinateTileDownCanMatch(startPoint);
            Vector2Int leftCoordinate = GetCoordinateTileLeftCanMatch(startPoint);
            Vector2Int rightCoordinate = GetCoordinateTileRightCanMatch(startPoint);
            
            // doi voi cac diem phia tren cua startPoint
            for (Vector2Int point = startPoint + new Vector2Int(1,0); point.x <= upCoordinate.x; point += new Vector2Int(1,0)) // noi theo huong phia tren
            {
                // voi moi diem, duyet voi 4 huong xem co the di toi dau
                Vector2Int upCoordinatePoint = GetCoordinateTileUpCanMatch(point);
                Vector2Int downCoordinatePoint = GetCoordinateTileDownCanMatch(point);
                Vector2Int leftCoordinatePoint = GetCoordinateTileLeftCanMatch(point);
                Vector2Int rightCoordinatePoint = GetCoordinateTileRightCanMatch(point);
                
                // voi moi huong se kiem tra co noi duoc tu point sang endPoint khong
                bool canMatch = CheckHavePathWithTwoLine(point, endPoint);
                if (canMatch)
                {
                    OnDrawWithOneLine(tiles[startPoint.x, startPoint.y].SpriteRenderer.bounds.center, tiles[point.x, point.y].SpriteRenderer.bounds.center);
                    return true;
                }
            }
            
            // doi voi cac diem phia duoi cua startPoint
            for (Vector2Int point = startPoint + new Vector2Int(-1,0); point.x >= downCoordinate.x; point += new Vector2Int(-1,0) ) // noi theo huong phia duoi
            {
                // voi moi diem, duyet voi 4 huong xem co the di toi dau
                Vector2Int upCoordinatePoint = GetCoordinateTileUpCanMatch(point);
                Vector2Int downCoordinatePoint = GetCoordinateTileDownCanMatch(point);
                Vector2Int leftCoordinatePoint = GetCoordinateTileLeftCanMatch(point);
                Vector2Int rightCoordinatePoint = GetCoordinateTileRightCanMatch(point);
                
                // voi moi huong se kiem tra co noi duoc tu point sang endPoint khong
                bool canMatch = CheckHavePathWithTwoLine(point, endPoint);
                if (canMatch)
                {
                    OnDrawWithOneLine(tiles[startPoint.x, startPoint.y].SpriteRenderer.bounds.center, tiles[point.x, point.y].SpriteRenderer.bounds.center);
                    return true;
                }
            }
            
            // doi voi cac diem ben trai
            for (Vector2Int point = startPoint + new Vector2Int(0,-1); point.y >= leftCoordinate.y; point += new Vector2Int(0,-1)) // noi theo huong phia trai
            {
                // voi moi diem, duyet voi 4 huong xem co the di toi dau
                Vector2Int upCoordinatePoint = GetCoordinateTileUpCanMatch(point);
                Vector2Int downCoordinatePoint = GetCoordinateTileDownCanMatch(point);
                Vector2Int leftCoordinatePoint = GetCoordinateTileLeftCanMatch(point);
                Vector2Int rightCoordinatePoint = GetCoordinateTileRightCanMatch(point);
                
                // voi moi huong se kiem tra co noi duoc tu point sang endPoint khong
                bool canMatch = CheckHavePathWithTwoLine(point, endPoint);
                if (canMatch)
                {
                    OnDrawWithOneLine(tiles[startPoint.x, startPoint.y].SpriteRenderer.bounds.center, tiles[point.x, point.y].SpriteRenderer.bounds.center);
                    return true;
                }
            }
            
            // doi voi cac diem ben phai
            for (Vector2Int point = startPoint + new Vector2Int(0,1); point.y <= rightCoordinate.y; point += new Vector2Int(0,1)) // noi theo huong phia phai
            {
                // voi moi diem, duyet voi 4 huong xem co the di toi dau
                Vector2Int upCoordinatePoint = GetCoordinateTileUpCanMatch(point);
                Vector2Int downCoordinatePoint = GetCoordinateTileDownCanMatch(point);
                Vector2Int leftCoordinatePoint = GetCoordinateTileLeftCanMatch(point);
                Vector2Int rightCoordinatePoint = GetCoordinateTileRightCanMatch(point);
                
                // voi moi huong se kiem tra co noi duoc tu point sang endPoint khong
                bool canMatch = CheckHavePathWithTwoLine(point, endPoint);
                if (canMatch)
                {
                    OnDrawWithOneLine(tiles[startPoint.x, startPoint.y].SpriteRenderer.bounds.center, tiles[point.x, point.y].SpriteRenderer.bounds.center);
                    return true;
                }
            }
            
            return false;
        }

        private bool CheckHavePathWithTwoLine(Vector2Int startPoint, Vector2Int endPoint)
        {
            // kiem tra xung quanh diem startPoint co the noi max toi dau
            Vector2Int upCoordinate = GetCoordinateTileUpCanMatch(startPoint);
            Vector2Int downCoordinate = GetCoordinateTileDownCanMatch(startPoint);
            Vector2Int leftCoordinate = GetCoordinateTileLeftCanMatch(startPoint);
            Vector2Int rightCoordinate = GetCoordinateTileRightCanMatch(startPoint);
            
            // kiem tra xem endPoint co noi duoc toi cac diem thuoc duong thang co gioi han tren khong
            for (Vector2Int point = startPoint + new Vector2Int(1,0); point.x <= upCoordinate.x; point += new Vector2Int(1,0)) // noi theo huong phia tren
            {
                if (CanTwoPointMatching(point, endPoint)) // neu co the noi duoc den endPoint
                {
                    OnDrawWithTwoLines(tiles[startPoint.x, startPoint.y].SpriteRenderer.bounds.center, tiles[point.x, point.y].SpriteRenderer.bounds.center, tiles[endPoint.x, endPoint.y].SpriteRenderer.bounds.center);
                    return true;
                }
            }
            
            for (Vector2Int point = startPoint + new Vector2Int(-1,0); point.x >= downCoordinate.x; point += new Vector2Int(-1,0) ) // noi theo huong phia duoi
            {
                if (CanTwoPointMatching(point, endPoint)) // neu co the noi duoc den endPoint
                {
                    OnDrawWithTwoLines(tiles[startPoint.x, startPoint.y].SpriteRenderer.bounds.center, tiles[point.x, point.y].SpriteRenderer.bounds.center, tiles[endPoint.x, endPoint.y].SpriteRenderer.bounds.center);
                    return true;
                }
            }
            
            for (Vector2Int point = startPoint + new Vector2Int(0,-1); point.y >= leftCoordinate.y; point += new Vector2Int(0,-1)) // noi theo huong phia trai
            {
                if (CanTwoPointMatching(point, endPoint)) // neu co the noi duoc den endPoint
                {
                    OnDrawWithTwoLines(tiles[startPoint.x, startPoint.y].SpriteRenderer.bounds.center, tiles[point.x, point.y].SpriteRenderer.bounds.center, tiles[endPoint.x, endPoint.y].SpriteRenderer.bounds.center);
                    return true;
                }
            }
            
            for (Vector2Int point = startPoint + new Vector2Int(0,1); point.y <= rightCoordinate.y; point += new Vector2Int(0,1)) // noi theo huong phia phai
            {
                if (CanTwoPointMatching(point, endPoint)) // neu co the noi duoc den endPoint
                {
                    OnDrawWithTwoLines(tiles[startPoint.x, startPoint.y].SpriteRenderer.bounds.center, tiles[point.x, point.y].SpriteRenderer.bounds.center, tiles[endPoint.x, endPoint.y].SpriteRenderer.bounds.center);
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
            
            if (upCoordinate + new Vector2Int(1,0) == endPoint || downCoordinate + new Vector2Int(-1,0) == endPoint 
                                                               || leftCoordinate + new Vector2Int(0,-1) == endPoint || rightCoordinate + new Vector2Int(0,1) == endPoint)
            {
                return true;
            }

            return false;
        }
        
        private IEnumerator IECoroutineMatchTile(Vector2Int startPoint, Vector2Int endPoint)
        {
            tiles[startPoint.x, startPoint.y].PlayEffectMatch();
            tiles[endPoint.x, endPoint.y].PlayEffectMatch();
            
            yield return new WaitForSeconds(1f);
            
            tiles[startPoint.x, startPoint.y].OnDespawn();
            tiles[endPoint.x, endPoint.y].OnDespawn();
        }
        
        private Vector2Int GetCoordinateTileRightCanMatch(Vector2Int point)
        {
            int idx = point.y;
            for (int i = point.y + 1; i <= columns+1; i++) // duyet theo x ve phia ben phai cua diem
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
            for (int i  = point.x - 1; i >= 0; --i) // duyet theo hang ve phia ben duoi cua diem
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
            for (int i = point.x + 1; i <= rows+1; ++i) // duyet theo hang ve phia ben tren cua diem
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
        private void OnDrawWithOneLine(Vector3 point1, Vector3 point2)
        {
            LineDrawer line = SimplePool.Spawn<LineDrawer>(PoolType.LINE_MATCH, lineParent.transform.position, lineParent.transform.rotation);
            line.DrawLine(point1, point2);
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private void OnDrawWithTwoLines(Vector3 point1, Vector3 point2, Vector3 point3)
        {
            LineDrawer line1 = SimplePool.Spawn<LineDrawer>(PoolType.LINE_MATCH, lineParent.transform.position, lineParent.transform.rotation);
            line1.DrawLine(point1, point2);
            
            LineDrawer line2 = SimplePool.Spawn<LineDrawer>(PoolType.LINE_MATCH, lineParent.transform.position, lineParent.transform.rotation);
            line2.DrawLine(point2, point3);
        }
    }
}
