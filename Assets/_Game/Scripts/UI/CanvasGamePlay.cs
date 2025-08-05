using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CanvasGamePlay : UICanvas
{
    // level
    [SerializeField] private TextMeshProUGUI textLevel;

    // Time
    [SerializeField] private Image timerCountImage; // hien thi thoi gian troi qua

    // freeze
    [SerializeField] private GameObject freezeBarObject; // hien thi thoi gian bi dong bang
    [SerializeField] private GameObject freezeBorder; // hien thi thoi gian bi dong bang

    // time out
    [SerializeField] private GameObject timeOutBorder; // canh bao thoi gian sap het

    // overlay ui
    [SerializeField] private GameObject overlayUI; // che khi win or lose
    [SerializeField] private TextMeshProUGUI textEndLevel; // hien cung overlayUI

    // replay Game
    [SerializeField] private GameObject tableNoticeReplay; // bang lua chon co chac chan khong

    // back menu
    [SerializeField] private GameObject tableNoticeMenu; // bang lua chon co chac chan khong

    public override void SetUp()
    {
        base.SetUp();
        textLevel.text = "LEVEL " + LevelManager.Instance.LevelIndex.ToString();
        timerCountImage.fillAmount = 1f; // Khoi tao lai thoi gian UI

        SetDeActiveOverlayUI();
        SetDeACtiveTimeoutBorder();
        SetDeActiveFreezeAction();
        SetDeActiveTableNoticeMenu();
        SetDeActiveTableNoticeReplay();
    }

    public override void Open()
    {
        base.Open();
        UIManager.Instance.GetUI<CanvasTransition>().OnOpenTransition(0.25f);
        EffectManager.Instance.OnEffectGamePlay();
    }

    public void HintButton()
    {
        SoundManager.Instance.PlayFx(FxID.BUTTON);
        if (!GameManager.Instance.IsState(GameState.HINT) && !GameManager.Instance.IsState(GameState.SHUFFLE))  // neu dang hint hoac shuffle thi khong cho shuffle
        {
            GameManager.Instance.ChangeState(GameState.HINT);
            BoosterManager.Instance.OnBooster(ETypeBooster.HINT);
        }
    }

    public void ShuffleButton()
    {
        SoundManager.Instance.PlayFx(FxID.BUTTON);

        if (!GameManager.Instance.IsState(GameState.HINT) && !GameManager.Instance.IsState(GameState.SHUFFLE))  // neu dang hint hoac shuffle thi khong cho shuffle
        {
            GameManager.Instance.ChangeState(GameState.SHUFFLE);
            BoosterManager.Instance.OnBooster(ETypeBooster.SHUFFLE);
        }
    }

    public void FreezeTimeButton()
    {
        SoundManager.Instance.PlayFx(FxID.BUTTON);

        if (GameManager.Instance.IsState(GameState.FREEZE_TIME)) return;

        GameManager.Instance.ChangeState(GameState.FREEZE_TIME);
        BoosterManager.Instance.OnBooster(ETypeBooster.FREEZE_TIME);

        EffectManager.Instance.OnEffectFreezeTime();
        SetActiveFreezeAction();
        SetDeACtiveTimeoutBorder();
    }

    public void HomeButton()
    {
        SoundManager.Instance.PlayFx(FxID.BUTTON);

        GameManager.Instance.ChangeState(GameState.PAUSE);
        SetActiveTableNoticeMenu();
    }

    public void OnYesBackHomeButton()
    {
        SoundManager.Instance.PlayFx(FxID.BUTTON);

        GameManager.Instance.ChangeState(GameState.MAIN_MENU);
        UIManager.Instance.CloseUI<CanvasGamePlay>(0.5f);
        UIManager.Instance.GetUI<CanvasTransition>().OnCloseTransition(0f);
        Invoke(nameof(OpenMainMenuUI), 0.5f);
    }

    public void OnNoBackHomeButton()
    {
        SoundManager.Instance.PlayFx(FxID.BUTTON);

        if (GamePlayManager.Instance.TimeController.IsFreezeTime)
        {
            GameManager.Instance.ChangeState(GameState.FREEZE_TIME);
        }
        else
        {
            GameManager.Instance.ChangeState((GameState.GAME_PLAY));
        }

        SetDeActiveTableNoticeMenu();
    }

    public void ReplayButton()
    {
        SoundManager.Instance.PlayFx(FxID.BUTTON);

        GameManager.Instance.ChangeState(GameState.PAUSE);
        SetActiveTableNoticeReplay();
    }

    public void OnYesReplayButton()
    {
        SoundManager.Instance.PlayFx(FxID.BUTTON);

        GameManager.Instance.ChangeState(GameState.GAME_PLAY);
        UIManager.Instance.CloseUI<CanvasGamePlay>(0.5f);
        UIManager.Instance.GetUI<CanvasTransition>().OnCloseTransition(0f);
        Invoke(nameof(OpenGamePlayUI), 0.5f);
    }

    public void OnNoReplayButton()
    {
        SoundManager.Instance.PlayFx(FxID.BUTTON);

        if (GamePlayManager.Instance.TimeController.IsFreezeTime)
        {
            GameManager.Instance.ChangeState(GameState.FREEZE_TIME);
        }
        else
        {
            GameManager.Instance.ChangeState((GameState.GAME_PLAY));
        }

        SetDeActiveTableNoticeReplay();
    }

    public void SettingButton()
    {
        SoundManager.Instance.PlayFx(FxID.BUTTON);

        GameManager.Instance.ChangeState(GameState.SETTING);
        UIManager.Instance.OpenUI<CanvasSetting>();
    }

    public void UpdateTimer(float fillAmount)
    {
        timerCountImage.fillAmount = fillAmount;
    }

    public void SetActiveOverlayUI()
    {
        if (GameManager.Instance.IsState(GameState.WIN_GAME))
        {
            textEndLevel.text = "<incr> LEVEL COMPLETE </incr>";
        }
        else if (GameManager.Instance.IsState(GameState.LOSE_GAME))
        {
            textEndLevel.text = "<shake> TIME OUT !!! </shake>";
        }

        overlayUI.SetActive(true);
    }

    private void OpenGamePlayUI()
    {
        UIManager.Instance.OpenUI<CanvasGamePlay>();
        GamePlayManager.Instance.OnPlayGame();
    }

    private void OpenMainMenuUI()
    {
        LevelManager.Instance.OnDespawn();
        UIManager.Instance.OpenUI<CanvasMenu>();
        SoundManager.Instance.ChangeSound(SoundID.BG_CLASSIC, 0);

    }
    #region Setters

    public void SetActiveFreezeAction()
    {
        freezeBarObject.SetActive(true);
        freezeBorder.SetActive(true);
    }

    public void SetDeActiveFreezeAction()
    {
        freezeBarObject.SetActive(false);
        freezeBorder.SetActive(false);
    }

    public void SetActiveTimeOutBorder()
    {
        timeOutBorder.SetActive(true);
    }

    public void SetDeACtiveTimeoutBorder()
    {
        timeOutBorder.SetActive(false);
    }

    private void SetDeActiveOverlayUI()
    {
        overlayUI.SetActive(false);
    }

    private void SetActiveTableNoticeMenu()
    {
        tableNoticeMenu.SetActive(true);
    }

    private void SetDeActiveTableNoticeMenu()
    {
        tableNoticeMenu.SetActive(false);
    }

    private void SetActiveTableNoticeReplay()
    {
        tableNoticeReplay.SetActive(true);
    }

    private void SetDeActiveTableNoticeReplay()
    {
        tableNoticeReplay.SetActive(false);
    }

    #endregion

    #region Getters
    public bool IsActiveTimeOutBorder => timeOutBorder.activeSelf;
    #endregion
}
