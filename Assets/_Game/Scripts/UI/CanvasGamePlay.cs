using _Game.Extensions.DP.ObserverPattern;
using _Game.Extensions.UI;
using _Game.Scripts.Booster;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Scripts.UI
{
    public class CanvasGamePlay : UICanvas
    {
        // Time
        [SerializeField] private Image timerCountImage; // Hình ảnh hiển thị thời gian
        
        // effect GamePlay
        [SerializeField] private GameObject effectPlaying;
        [SerializeField] private GameObject effectFreezeTime;
        
        // freeze
        [SerializeField] private TextMeshProUGUI textFreeze;
        
        public override void SetUp()
        {
            base.SetUp();
            EffectInit();
            
            timerCountImage.fillAmount = 1f; // Khởi tạo hình ảnh thời gian đầy
            Messenger.AddListener<float>(EventKey.UpdateTimer, UpdateTimer); // Đăng ký sự kiện cập nhật thời gian
        }

        public override void Open()
        {
            base.Open();
            Messenger.Broadcast(EventKey.EffectOpenCanvas); // Phát sự kiện mở canvas
        }

        public override void Close(float delay)
        {
            base.Close(delay);
            Messenger.RemoveListener<float>(EventKey.UpdateTimer, UpdateTimer); // Huy sự kiện cập nhật thời gian
        }
        
        public void HintButton()
        {
            BoosterControl.Instance.PlayHint();
        }
    
        public void ShuffleButton()
        {
            BoosterControl.Instance.ShuffleArray();
        }

        public void FreezeTimeButton()
        {
            if (GameManager.IsState(GameState.FREEZE_TIME)) return;
            
            textFreeze.gameObject.SetActive(true);
            effectPlaying.SetActive(false);
            effectFreezeTime.SetActive(true);
            
            BoosterControl.Instance.FreezeTime();
            
            Invoke(nameof(EffectInit), BoosterControl.Instance.FreezeTimeDurationInSeconds);
        }

        public void HomeButton()
        {
			Messenger.Broadcast(EventKey.EffectCloseCanvas);
			UIManager.Instance.CloseUI<CanvasGamePlay>(0.5f); // Thời gian đóng menu
			Invoke(nameof(ChangeStateMainMenu), 1f); // Thay đổi trạng thái sau khi đóng menu
        }
        
        private void ChangeStateMainMenu()
        {
            GameManager.Instance.ChangeState(GameState.MAIN_MENU);
            UIManager.Instance.OpenUI<CanvasMenu>();
        }
        
        public void ReplayButton() {}
        
        public void SettingButton() {}
        
        
        private void EffectInit()
        {
            textFreeze.gameObject.SetActive(false);
            effectPlaying.SetActive(true);
            effectFreezeTime.SetActive(false);
        }
        
        private void UpdateTimer(float fillAmount)
        {
            timerCountImage.fillAmount = fillAmount;
        }
        
    }
}
