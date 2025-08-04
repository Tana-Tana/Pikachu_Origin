
public class CanvasFail : UICanvas
{
    public override void Open()
    {
        base.Open();
        SoundManager.Instance.PlayFx(FxID.LOSE_GAME);
        UIManager.Instance.GetUI<CanvasTransition>().OnOpenTransition(0f);
        EffectManager.Instance.OnEffectEndGame();
        Invoke(nameof(SetSoundVolume), 1f);
    }

    public void HomeButton()
    {
        SoundManager.Instance.PlayFx(FxID.BUTTON);
        
        GameManager.Instance.ChangeState(GameState.MAIN_MENU);
        UIManager.Instance.CloseUI<CanvasFail>(0.5f);
        UIManager.Instance.GetUI<CanvasTransition>().OnCloseTransition(0f);
        Invoke(nameof(OpenMainMenuUI), 0.5f);
    }

    public void SettingButton()
    {
        SoundManager.Instance.PlayFx(FxID.BUTTON);

        GameManager.Instance.ChangeState(GameState.SETTING);
        UIManager.Instance.OpenUI<CanvasSetting>();
    }

    public void ReplayButton()
    {
        SoundManager.Instance.PlayFx(FxID.BUTTON);

        GameManager.Instance.ChangeState(GameState.GAME_PLAY);
        UIManager.Instance.CloseUI<CanvasFail>(0.5f);
        UIManager.Instance.GetUI<CanvasTransition>().OnCloseTransition(0f);
        Invoke(nameof(OpenGamePlayUI), 0.5f);
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

    private void SetSoundVolume() {
        SoundManager.Instance.SetSoundVolume(0.8f); // cho tieng ve ban dau
    }
}

