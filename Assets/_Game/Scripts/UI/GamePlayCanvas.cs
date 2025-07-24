using System;
using _Game.Extensions.UI;
using _Game.Scripts.Booster;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Scripts.UI
{
    public class GamePlayCanvas : UICanvas
    {
        [SerializeField] private Image timerCountImage; // Hình ảnh hiển thị thời gian
        public override void SetUp()
        {
            base.SetUp();
            timerCountImage.fillAmount = 1f; // Khởi tạo hình ảnh thời gian đầy
            Messenger.AddListener<float>(EventKey.UpdateTimer, UpdateTimer); // Đăng ký sự kiện cập nhật thời gian
        }

        public override void Close(float delay)
        {
            base.Close(delay);
            Messenger.RemoveListener<float>(EventKey.UpdateTimer, UpdateTimer); // Huy sự kiện cập nhật thời gian
        }
        
        private void UpdateTimer(float fillAmount)
        {
            timerCountImage.fillAmount = fillAmount;
        }
        
        public void HintButton()
        {
            BoosterControl.Instance.PlayHint();
        }
    
        public void ShuffleButton()
        {
            BoosterControl.Instance.ShuffleArray();
        }
    }
}
