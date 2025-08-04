using System;
using _Game.Extensions.DP;
using Unity.VisualScripting;
using UnityEngine;

public enum GameState
{
    MAIN_MENU = 0,
    GAME_PLAY = 1,
    SETTING = 2,
    INFORMATION = 3,
    SHOP = 4,
    ACHIEVEMENT = 5,
    ADS = 6,
    NEXT_LEVEL = 7,
    SHUFFLE = 8,
    HINT = 9,
    FREEZE_TIME = 10,
    WIN_GAME = 11,
    LOSE_GAME = 12,
    PAUSE = 13,
}

public class GameManager : Singleton<GameManager>
{
    private GameState gameState;
    private void Awake()
    {
        //tranh viec nguoi choi cham da diem vao man hinh
        Input.multiTouchEnabled = false;
        //target frame rate ve 60 fps
        Application.targetFrameRate = 60;
        //tranh viec tat man hinh
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        //xu li tai tho
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
        DataManager.Instance.OnInit();
        LevelManager.Instance.SetLevelIndex(DataManager.Instance.UserData.CurrentLevel);

        UIManager.Instance.OnInit();
        SoundManager.Instance.OnInit();
        ChangeState(GameState.MAIN_MENU);
    }
    
    public void ChangeState(GameState state)
    {
        gameState = state;
    }

    public bool IsState(GameState state) => gameState == state;
}