using System.Collections;
using System.Collections.Generic;
using _Game.Extensions.DP;
using _Game.Extensions.UI;
using _Game.Scripts.UI;
using UnityEngine;

public enum GameState
{
    MAIN_MENU = 0,
    GAME_PLAY = 1,
    WIN = 2,
    LOSE = 3,
    REVIVE = 4,
    PAUSE = 5,
    SHUFFLE = 6,
    MATCHING = 7,
}

public class GameManager : Singleton<GameManager>
{
    private static GameState gameState;
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
        //UIManager.Ins.OpenUI<UIMainMenu>();
        UIManager.Instance.OpenUI<GamePlayCanvas>();
        ChangeState(GameState.GAME_PLAY);
    }
    
    public void ChangeState(GameState state)
    {
        gameState = state;
    }

    public static bool IsState(GameState state) => gameState == state;
}