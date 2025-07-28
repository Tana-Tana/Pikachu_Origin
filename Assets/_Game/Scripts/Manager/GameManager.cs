using System.Collections;
using System.Collections.Generic;
using _Game.Extensions.DP;
using _Game.Extensions.DP.ObserverPattern;
using _Game.Extensions.UI;
using _Game.Scripts.Manager;
using _Game.Scripts.Player;
using _Game.Scripts.Tile;
using _Game.Scripts.Timer;
using _Game.Scripts.UI;
using UnityEngine;

public enum GameState
{
    MAIN_MENU = 0,
    GAME_PLAY = 1,
    WIN = 2,
    LOSE = 3,
    SETTING = 4,
    FREEZE_TIME = 5,
    INFORMATION = 6,
    SHOP = 7,
    ACHIEVEMENT = 8,
    ADS = 9,
    REPLAY = 10,
}

public class GameManager : Singleton<GameManager>
{
    [Header("Script Control Game")] [SerializeField]
    private PlayerControl playerControl;

    [SerializeField] private TimeControl timeControl;
    private static GameState gameState;
    
    // classic mode
    private int progress = 1;
    public const int MAX_LEVEL_OF_CLASSIC_MODE = 14;
    
    private void Awake()
    {
        //tranh viec nguoi choi cham da diem vao man hinh
        Input.multiTouchEnabled = false;
        //target frame rate ve 60 fps
        Application.targetFrameRate = 60;
        //tranh viec tat man hinh
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        //xu tai tho
        int maxScreenHeight = 1280;
        float ratio = (float)Screen.currentResolution.width / (float)Screen.currentResolution.height;
        if (Screen.currentResolution.height > maxScreenHeight)
        {
            Screen.SetResolution(Mathf.RoundToInt(ratio * (float)maxScreenHeight), maxScreenHeight, true);
        }
    }

    private void Start()
    {
        OnInit();
    }

    private void OnInit()
    {
        UIManager.Instance.OpenUI<CanvasMenu>();
        ChangeState(GameState.MAIN_MENU);
        
        // classic mode
        SetProgress(0);
    }
    
    public void PlayGameOnClassicMode()
    {
        if (IsState(GameState.REPLAY) || IsState(GameState.MAIN_MENU)) SetProgress(1);
        else
        {
            SetProgress(++progress);
        }
        
        ChangeState(GameState.GAME_PLAY);
        ActiveControlOfPlayerInGame();
        timeControl.OnInit(Random.Range(30f,60f)); // Thay đổi thời gian chơi nếu cần
        
        // de tam
        TileManager.Instance.OnDespawn();
        TileManager.Instance.OnInit();
    }

    private void ActiveControlOfPlayerInGame()
    {
        playerControl.enabled = true;
    }
    
    public void DeActiveControlOfPlayerInGame()
    {
        playerControl.enabled = false;
    }
    
    public void ChangeState(GameState state)
    {
        gameState = state;
    }

    public static bool IsState(GameState state) => gameState == state;
    
    public int Progress => progress;
    public void SetProgress(int value)
    {
        progress = value;
    }
    
}