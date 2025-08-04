using System;
using _Game.Extensions.DP;
using UnityEngine;

public class GamePlayManager : Singleton<GamePlayManager>
{
    [SerializeField] private GamePlayEventChecker gameEvent;
    [SerializeField] private CameraController cameraController;
    [SerializeField] private TimeController timeController;

    #region core GamePlay
    //goi khi bat dau gameplay
    public void OnPlayGame()
    {
        SoundManager.Instance.ChangeSound(SoundID.BG_GAMEPLAY, 0);
        LevelManager.Instance.OnPlayLevel();
        gameEvent.OnInit();

        GameManager.Instance.ChangeState(GameState.SHUFFLE);
        BoosterManager.Instance.OnBooster(ETypeBooster.SHUFFLE); // shuffle lan dau cho cac game man khi lap lai no khac nhau
        timeController.OnInit(LevelManager.Instance.GetTimeLevel); // dat thoi gian cho level hien tai
    }

    // goi khi chien thang
    public void OnWinGame()
    {
        Debug.Log("Win game!");
        SoundManager.Instance.SetSoundVolume(0.3f); // cho be tieng sound lai
        SoundManager.Instance.PlayFx(FxID.LEVEL_COMPLETE);
        LevelManager.Instance.AddLevel(); // cong them level tiep theo
        DataManager.Instance.UserData.SetDataLevelIndex(LevelManager.Instance.LevelIndex); // dat lai data cho user o thoi diem hien tai
        DataManager.Instance.SaveUserData(); // save lai vao file

        LevelManager.Instance.OnDespawn();
        UIManager.Instance.CloseUI<CanvasGamePlay>(3f);
        UIManager.Instance.GetUI<CanvasTransition>().OnCloseTransition(2.5f);
        Invoke(nameof(OpenUIVictory), 3.5f);
    }

    // goi khi thua
    public void OnLoseGame()
    {
        Debug.Log("Lose Game");
        SoundManager.Instance.SetSoundVolume(0.3f); // cho be tieng sound lai
        SoundManager.Instance.PlayFx(FxID.TIME_OUT);

        LevelManager.Instance.OnDespawn();
        UIManager.Instance.CloseUI<CanvasGamePlay>(3f);
        UIManager.Instance.GetUI<CanvasTransition>().OnCloseTransition(2.5f);
        Invoke(nameof(OpenUIFail), 3.5f);
    }

    public void OnNextLevelGame()
    {
        LevelManager.Instance.OnNextLevel();
        gameEvent.OnInit();

        GameManager.Instance.ChangeState(GameState.SHUFFLE);
        BoosterManager.Instance.OnBooster(ETypeBooster.SHUFFLE); // shuffle lan dau cho cac game man khi lap lai no khac nhau
        timeController.OnInit(LevelManager.Instance.GetTimeLevel); // dat thoi gian cho level hien tai
    }

    #endregion

    // kiem tra khi nao can shuffle
    public bool OnCheckShuffle()
    {
        return gameEvent.CheckNeedShuffle();
    }

    public bool OnCheckWinGame()
    {
        return gameEvent.CheckWinGame();
    }

    #region Getters and Setters

    public CameraController CameraController => cameraController;
    public GamePlayEventChecker GameEvent => gameEvent;
    public TimeController TimeController => timeController;
    #endregion

    private void OpenUIVictory()
    {
        UIManager.Instance.OpenUI<CanvasVictory>();
    }

    private void OpenUIFail()
    {
        UIManager.Instance.OpenUI<CanvasFail>();
    }
}
