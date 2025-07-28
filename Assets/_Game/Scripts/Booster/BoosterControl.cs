using System;
using System.Collections;
using _Game.Extensions.DP;
using _Game.Scripts.Manager;
using _Game.Scripts.Tile;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Game.Scripts.Booster
{
    public class BoosterControl : Singleton<BoosterControl>
    {
        [SerializeField] private float timeFreeze = 10f; // Thời gian đông băng
        public float FreezeTimeDurationInSeconds => timeFreeze;
        
        private Tile.Tile tile1;
        private Tile.Tile tile2;

        [ContextMenu("Shuffle Array")]
        public void ShuffleArray()
        {
            StartCoroutine(IEShuffle());
        }
        
        [ContextMenu("Hint")]

        public void PlayHint()
        {
            if (tile1 == null || tile2 == null)
            {
                Debug.LogWarning("No hint set. Please set a hint before playing.");
                return;
            }
            
            this.tile1.SetActiveSelected();
            this.tile2.SetActiveSelected();
            TileManager.Instance.OnMatching(ref tile1, ref tile2); // goi ham match hai tile
        }
        
        public void SetHint(Tile.Tile tile1, Tile.Tile tile2)
        {
            this.tile1 = tile1;
            this.tile2 = tile2;
        }

        [ContextMenu("Freeze Time")]
        public void FreezeTime()
        {
            if (!GameManager.IsState(GameState.GAME_PLAY)) return; // Neu khong phai trang thai GAME_PLAY thi khong lam gi ca
            GameManager.Instance.ChangeState(GameState.FREEZE_TIME);
        }
        
        private IEnumerator IEShuffle()
        {
            PlayInEffectShuffle();
            yield return new WaitForSeconds(0.5f); // cho hieu ung xuat hien trong 0.5 giay
            Shuffle();
            PlayOutEffectShuffle();
            yield return new WaitForSeconds(0.5f); // hieu ung xuat hien trong 0.5 giay
            
            if (GameEvent.Instance.CheckNeedShuffle()) // neu khong con tile nao co the match duoc
            {
                ShuffleArray(); // shuffle lai
            }
        }

        public void Shuffle()
        {
            for (int i = 1; i <= TileManager.Instance.Rows; ++i)
            {
                for (int j = 1; j <= TileManager.Instance.Columns; ++j)
                {
                    int randomRow = Random.Range(1, TileManager.Instance.Rows + 1);
                    int randomColumn = Random.Range(1, TileManager.Instance.Columns + 1);
                    // Hoán đổi vị trí của tileActives[i, j] với tileActives[randomRow, randomColumn]
                    (TileManager.Instance.Tiles[i,j].TF.position, TileManager.Instance.Tiles[randomRow, randomColumn].TF.position) = (TileManager.Instance.Tiles[randomRow,randomColumn].TF.position,TileManager.Instance.Tiles[i,j].TF.position);
                    (TileManager.Instance.Tiles[i, j], TileManager.Instance.Tiles[randomRow, randomColumn]) = (TileManager.Instance.Tiles[randomRow, randomColumn], TileManager.Instance.Tiles[i, j]);
                }
            }
            
            Debug.Log("Shuffle complete!");
            
            for (int i = 1; i <= TileManager.Instance.Rows; ++i)
            {
                for (int j = 1; j <= TileManager.Instance.Columns; ++j)
                {
                    TileManager.Instance.Tiles[i,j].SetName($"Tile_{i}_{j}"); // dat ten cho tile de de dang quan ly
                    TileManager.Instance.Tiles[i,j].SetLocationInMatrix(i,j);
                }
            }
        }

        private void PlayInEffectShuffle()
        {
            for (int i=1;i<=TileManager.Instance.Rows; ++i)
            {
                for (int j = 1; j <= TileManager.Instance.Columns; ++j)
                {
                    TileManager.Instance.Tiles[i,j].OnDespawn();
                }
            }
        }
        
        private void PlayOutEffectShuffle()
        {
            for (int i=1;i<=TileManager.Instance.Rows; ++i)
            {
                for (int j = 1; j <= TileManager.Instance.Columns; ++j)
                {
                    if (!TileManager.Instance.Tiles[i, j].IsDoneTask)
                    {
                        TileManager.Instance.Tiles[i,j].OnInit();
                    } 
                }
            }
        }
    }
}
