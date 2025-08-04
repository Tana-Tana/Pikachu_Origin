using _Game.Extensions.DP;
using UnityEngine;

public class LevelManager : Singleton<LevelManager>
{
    [SerializeField] private Level level;
    private int levelIndex = 0;
    
    //khoi tao trang thai bat dau game
    public void OnInit()
    {
        level.OnInit();
    }

    //reset trang thai khi ket thuc game
    public void OnDespawn()
    {
        level.OnDespawn();
    }

    //Load Level Moi
    public void OnLoadLevel(int levelIndex)
    {
        level.OnLoadLevel(levelIndex);
    }

    public void OnNextLevel()
    {
        OnDespawn();
        OnLoadLevel(levelIndex);
        OnInit();
    }

    public void OnPlayLevel() 
    {
        OnDespawn();
        OnLoadLevel(levelIndex);
        OnInit();
    }

    #region Getters

    public int LevelIndex => levelIndex;
    public Level Level => level;

    public int GetRows => level.LevelData.Rows;
    public int GetColumns => level.LevelData.Columns;
    public Tile[,] GetTiles => level.Tiles;
    public float GetTimeLevel => level.LevelData.TimeLevel;
#endregion

    public void SetLevelIndex(int levelIndex)
    {
        this.levelIndex = levelIndex;
    }
    
    public void AddLevel() 
    {
        ++levelIndex;
        if (levelIndex > 30) levelIndex = 30;
    }
}