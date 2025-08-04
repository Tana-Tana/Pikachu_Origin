using System;
using System.Collections;
using _Game.Extensions.DP;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;


public enum ETypeBooster
{
    NONE = 0,
    HINT = 1,
    SHUFFLE = 2,
    FREEZE_TIME = 3,
}

public class BoosterManager : Singleton<BoosterManager>
{
    [SerializeField] private float timeFreeze = 15f; // freeTime

    // hint
    private Tile tile1;
    private Tile tile2;

    public void OnBooster(ETypeBooster type)
    {
        switch (type)
        {
            case ETypeBooster.HINT:
                OnHintBooster();
                break;
            case ETypeBooster.SHUFFLE:
                OnShuffleBooster();
                break;
            case ETypeBooster.FREEZE_TIME:
                OnFreezeTimeBooster();
                break;
            case ETypeBooster.NONE:
            default:
                Debug.Log("Khong dung booster");
                break;
        }
    }


    public void OnHintBooster()
    {
        if (tile1 == null || tile2 == null)
        {
            Debug.LogWarning("No hint set. Please set a hint before playing.");
            return;
        }

        tile1.OnSelect();
        tile2.OnSelect();
        LevelManager.Instance.Level.OnMatching(ref tile1, ref tile2); // goi ham match hai tile
    }

    public void OnShuffleBooster()
    {
        StartCoroutine(IEShuffle());
    }

    public void OnFreezeTimeBooster()
    {
        SoundManager.Instance.PlayFx(FxID.TIME_FREEZE);
        GamePlayManager.Instance.TimeController.SetFreezeTime(timeFreeze);
    }

    private IEnumerator IEShuffle()
    {
        EffectManager.Instance.OnDespawnEffectShuffle();
        yield return new WaitForSeconds(0.5f); // cho hieu ung xuat hien trong 0.5 giay
        Shuffle();
        EffectManager.Instance.OnInitEffectShuffle();
        yield return new WaitForSeconds(0.5f); // hieu ung xuat hien trong 0.5 giay

        if (GamePlayManager.Instance.TimeController.IsFreezeTime)
        {
            GameManager.Instance.ChangeState(GameState.FREEZE_TIME);
        }
        else
        {
            GameManager.Instance.ChangeState((GameState.GAME_PLAY));
        }

        if (GamePlayManager.Instance.OnCheckShuffle()) // kiem tra de shuffle tiep neu can
        {
            OnShuffleBooster();
        }
    }

    public void Shuffle()
    {
        for (int i = 1; i <= LevelManager.Instance.GetRows; ++i)
        {
            for (int j = 1; j <= LevelManager.Instance.GetColumns; ++j)
            {
                int rowRandom = Random.Range(1, LevelManager.Instance.GetRows + 1);
                int colRandom = Random.Range(1, LevelManager.Instance.GetColumns + 1);

                if ((int)LevelManager.Instance.GetTiles[i, j].GetTypeData >= 1000
                    || LevelManager.Instance.GetTiles[i, j].IsDone
                    || (int)LevelManager.Instance.GetTiles[rowRandom, colRandom].GetTypeData >= 1000
                    || LevelManager.Instance.GetTiles[rowRandom, colRandom].IsDone) // neu la vat can thi thoi
                {
                    continue;
                }

                // swap
                TileData data = new(LevelManager.Instance.GetTiles[i, j].GetTypeData, LevelManager.Instance.GetTiles[i, j].GetSprite);

                LevelManager.Instance.GetTiles[i, j].SetTileData(LevelManager.Instance.GetTiles[rowRandom, colRandom].TileData);
                LevelManager.Instance.GetTiles[rowRandom, colRandom].SetTileData(data);
            }
        }

        Debug.Log("Shuffle complete!");

        for (int i = 1; i <= LevelManager.Instance.GetRows; ++i)
        {
            for (int j = 1; j <= LevelManager.Instance.GetColumns; ++j)
            {
                LevelManager.Instance.GetTiles[i, j].SetName($"Tile_{i}_{j}");
                LevelManager.Instance.GetTiles[i, j].SetPosInMatrix(i, j);
            }
        }

        GamePlayManager.Instance.GameEvent.OnInit(); // dat lai list cho checker cho chuan
    }

    #region Setters

    public void SetHint(Tile tile1, Tile tile2)
    {
        this.tile1 = tile1;
        this.tile2 = tile2;
    }

    #endregion
}