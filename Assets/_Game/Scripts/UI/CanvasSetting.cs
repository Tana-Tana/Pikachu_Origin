using UnityEngine;


public class CanvasSetting : UICanvas
{
    [SerializeField] private Animator anim;
    private string animPopup;

    // sound
    [SerializeField] private GameObject soundTick;
    
    // fx
    [SerializeField] private GameObject fxTick;

    public override void Open()
    {
        base.Open();
        animPopup = GameConfig.ANIM_SETTING_OPEN; // Khởi tạo trigger animation
        SetTickSound();
        SetTickFx();
    }

    public void SoundButton()
    {
        SoundManager.Instance.PlayFx(FxID.BUTTON);
        SoundManager.Instance.SetSoundStatus((SoundManager.Instance.GetSoundStatus == 1) ? 0 : 1);
        SetTickSound();
    }

    public void FxButton()
    {
        SoundManager.Instance.PlayFx(FxID.BUTTON);
        SoundManager.Instance.SetFxStatus((SoundManager.Instance.GetFxStatus == 1) ? 0 : 1);
        SetTickFx();
    }

    public void ExitButton()
    {
        SoundManager.Instance.PlayFx(FxID.BUTTON);

        if (UIManager.Instance.IsOpened<CanvasGamePlay>())
        {
            if (GamePlayManager.Instance.TimeController.IsFreezeTime)
            {
                GameManager.Instance.ChangeState(GameState.FREEZE_TIME);
            }
            else
            {
                GameManager.Instance.ChangeState((GameState.GAME_PLAY));
            }
        }
        else if (UIManager.Instance.IsOpened<CanvasMenu>())
        {
            GameManager.Instance.ChangeState((GameState.MAIN_MENU));
        }
        else if (UIManager.Instance.IsOpened<CanvasVictory>())
        {
            GameManager.Instance.ChangeState((GameState.WIN_GAME));
        }
        else if (UIManager.Instance.IsOpened<CanvasFail>())
        {
            GameManager.Instance.ChangeState((GameState.LOSE_GAME));
        }

        ChangeAnim(GameConfig.ANIM_SETTING_CLOSE);
        UIManager.Instance.CloseUI<CanvasSetting>(anim.GetCurrentAnimatorStateInfo(0).length);
    }

    private void ChangeAnim(string animName)
    {
        if (animName != animPopup)
        {
            anim.ResetTrigger(animPopup);
            animPopup = animName;
            anim.SetTrigger(animPopup);
        }
    }

    private void SetTickSound()
    {
        soundTick.SetActive(SoundManager.Instance.GetSoundStatus == 1);
    }

    private void SetTickFx()
    {
        fxTick.SetActive(SoundManager.Instance.GetFxStatus == 1);
    }
}

