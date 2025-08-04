using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasTransition : UICanvas
{
    [SerializeField] private Animator anim;

    private string animName = "";

    public void OnOpenTransition(float delay) 
    {
        Invoke(nameof(ChangeAnimOpen), delay);
    }

    public void OnCloseTransition(float delay) 
    {
        Invoke(nameof(ChangeAnimClose), delay);
    }

    private void ChangeAnimOpen() 
    {
        ChangeAnim(GameConfig.ANIM_TRANSITION_OPEN);
    }

    private void ChangeAnimClose() 
    {
        ChangeAnim(GameConfig.ANIM_TRANSITION_CLOSE);
    }

    private void ChangeAnim(string animName)
    {
        if (this.animName != animName)
        {
            if(this.animName != "") anim.ResetTrigger(this.animName);
            this.animName = animName;
            anim.SetTrigger(this.animName);
        }
    }
}
