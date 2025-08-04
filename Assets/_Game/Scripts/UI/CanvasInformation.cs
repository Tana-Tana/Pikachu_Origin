using UnityEngine;

public class CanvasInformation : UICanvas
{
    [SerializeField] private Animator anim;
    private string animPopup;

    public override void Open()
    {
        base.Open();
        animPopup = GameConfig.ANIM_SETTING_OPEN; // Khởi tạo trigger animation
    }

    public void ExitButton()
    {
        SoundManager.Instance.PlayFx(FxID.BUTTON);

        ChangeAnim(GameConfig.ANIM_SETTING_CLOSE);
        UIManager.Instance.CloseUI<CanvasInformation>(anim.GetCurrentAnimatorStateInfo(0).length);
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
}

