using _Game.Common;
using _Game.Extensions.UI;
using UnityEngine;

namespace _Game.Scripts.UI
{
    public class CanvasInformation : UICanvas
    {
        [SerializeField] private Animator anim;
        private string animPopup;

        public override void SetUp()
        {
            base.SetUp();
            animPopup = GameConfig.ANIM_SETTING_OPEN; // Khởi tạo trigger animation
        }

        public void ExitButton()
        {
            ChangeAnim(GameConfig.ANIM_SETTING_EXIT);
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
}
