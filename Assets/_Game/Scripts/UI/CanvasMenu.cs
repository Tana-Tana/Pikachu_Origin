using System;
using TMPro;
using UnityEngine;

public class CanvasMenu : UICanvas
{
    // menu button
    [SerializeField] private Animator animMenuButton = null;
    private string animMenuButtonName = "isExpanded";
    private bool isExpanded = false;

    // text Level
    [SerializeField] private TextMeshProUGUI textLevel = null; // text hien thi level hien tai

    // reset Data
    [SerializeField] private GameObject tableNoticeResetData = null; // table hien thi canh bao reset data neu muon

    // tutorial
    [SerializeField] private GameObject tableNoticeTutorial = null; // table hien thi tutorial

    public override void SetUp()
    {
        base.SetUp();

        textLevel.text = "1 - " + LevelManager.Instance.LevelIndex.ToString();
        tableNoticeResetData.SetActive(false);
        tableNoticeTutorial.SetActive(false);
        EffectManager.Instance.OnInitEffect();
    }

    public override void Open()
    {
        base.Open();
        UIManager.Instance.GetUI<CanvasTransition>().OnOpenTransition(0f);
        if (LevelManager.Instance.LevelIndex == 1)
        {
            tableNoticeTutorial.SetActive(true);
        }
    }

    public void MenuButton()
    {
        SoundManager.Instance.PlayFx(FxID.BUTTON);

        animMenuButton.enabled = true;
        isExpanded = !isExpanded;

        if (isExpanded)
        {
            ChangeAnim(animMenuButton, ref animMenuButtonName, GameConfig.ANIM_MENU_BUTTON_EXPAND);
        }
        else
        {
            ChangeAnim(animMenuButton, ref animMenuButtonName, GameConfig.ANIM_MENU_BUTTON_COLLAPSE);
        }
    }

    public void SettingButton()
    {
        SoundManager.Instance.PlayFx(FxID.BUTTON);
        GameManager.Instance.ChangeState(GameState.SETTING);
        UIManager.Instance.OpenUI<CanvasSetting>();
    }

    public void ResetDataButton()
    {
        SoundManager.Instance.PlayFx(FxID.BUTTON);
        tableNoticeResetData.SetActive(true);
    }

    public void OnAcceptResetData()
    {
        SoundManager.Instance.PlayFx(FxID.BUTTON);
        DataManager.Instance.SetUserData(new UserData());
        DataManager.Instance.SaveUserData();
        LevelManager.Instance.SetLevelIndex(DataManager.Instance.UserData.CurrentLevel);


        GameManager.Instance.ChangeState(GameState.MAIN_MENU);
        UIManager.Instance.CloseUI<CanvasMenu>(0.5f);
        UIManager.Instance.GetUI<CanvasTransition>().OnCloseTransition(0f);
        Invoke(nameof(OpenMainMenuUI), 0.5f);
    }

    public void OnDenyResetData()
    {
        SoundManager.Instance.PlayFx(FxID.BUTTON);
        tableNoticeResetData.SetActive(false);
    }

    public void TutorialButton()
    {
        SoundManager.Instance.PlayFx(FxID.BUTTON);
        tableNoticeTutorial.SetActive(true);
    }

    public void OnCloseTutorial()
    {
        SoundManager.Instance.PlayFx(FxID.BUTTON);
        tableNoticeTutorial.SetActive(false);
    }

    public void PlayButton()
    {
        SoundManager.Instance.PlayFx(FxID.BUTTON);
        GameManager.Instance.ChangeState(GameState.GAME_PLAY);
        UIManager.Instance.CloseUI<CanvasMenu>(0.5f);
        UIManager.Instance.GetUI<CanvasTransition>().OnCloseTransition(0f);
        Invoke(nameof(OpenGamePlayUI), 0.5f);
    }

    public void ShopButton()
    {
        SoundManager.Instance.PlayFx(FxID.BUTTON);
        GameManager.Instance.ChangeState(GameState.SHOP);
        UIManager.Instance.OpenUI<CanvasInformation>(); // sau doi thanh CanvasShop
    }

    public void AchievementButton()
    {
        SoundManager.Instance.PlayFx(FxID.BUTTON);
        GameManager.Instance.ChangeState(GameState.ACHIEVEMENT);
        UIManager.Instance.OpenUI<CanvasInformation>(); // sau doi thanh CanvasAchievement
    }

    public void AdsButton()
    {
        SoundManager.Instance.PlayFx(FxID.BUTTON);
        GameManager.Instance.ChangeState(GameState.ADS);
        UIManager.Instance.OpenUI<CanvasInformation>(); // sau doi thanh CanvasAds
    }

    private void ChangeAnim(Animator anim, ref string currentAnim, string animName)
    {
        if (currentAnim != animName)
        {
            anim.ResetTrigger(currentAnim);
            currentAnim = animName;
            anim.SetTrigger(currentAnim);
        }
    }

    private void OpenMainMenuUI()
    {
        UIManager.Instance.OpenUI<CanvasMenu>();
    }

    private void OpenGamePlayUI()
    {
        UIManager.Instance.OpenUI<CanvasGamePlay>();
        GamePlayManager.Instance.OnPlayGame();
    }
}

