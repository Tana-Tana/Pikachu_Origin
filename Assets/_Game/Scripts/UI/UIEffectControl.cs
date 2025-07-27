using System;
using _Game.Common;
using _Game.Extensions.DP.ObserverPattern;
using UnityEngine;

namespace _Game.Scripts.UI
{
    public class UIEffectControl : MonoBehaviour
    {
        // effect khi bat dau game
        [SerializeField] private Animator animScrollViewLeft;
        private string currentAnimLeft = string.Empty;
    
        [SerializeField] private Animator animScrollViewRight;
        private string currentAnimRight = string.Empty;
    
        private void OnEnable()
        {
            Messenger.AddListener(EventKey.EffectOpenCanvas, OnEffectOpen);
            Messenger.AddListener(EventKey.EffectCloseCanvas, OnEffectClose);
        }

        private void OnDisable()
        {
            Messenger.RemoveListener(EventKey.EffectOpenCanvas, OnEffectOpen);
            Messenger.RemoveListener(EventKey.EffectCloseCanvas, OnEffectClose);
        }

        private void OnEffectClose()
        {
            ChangeAnim(animScrollViewLeft, ref currentAnimLeft, GameConfig.ANIM_SC_CLOSE);
            ChangeAnim(animScrollViewRight, ref currentAnimRight, GameConfig.ANIM_SC_CLOSE);
        }

        private void OnEffectOpen()
        {
            ChangeAnim(animScrollViewLeft, ref currentAnimLeft, GameConfig.ANIM_SC_OPEN);
            ChangeAnim(animScrollViewRight, ref currentAnimRight, GameConfig.ANIM_SC_OPEN);
        }
    
        private void ChangeAnim(Animator anim,ref string currentAnim, string animName)
        {
            if (currentAnim != animName)
            {
                if(currentAnim != String.Empty) anim.ResetTrigger(currentAnim);
                currentAnim = animName;
                anim.SetTrigger(currentAnim);
            }
        }
    }
}
