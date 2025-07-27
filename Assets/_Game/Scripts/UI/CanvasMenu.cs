using _Game.Common;
using _Game.Extensions.DP.ObserverPattern;
using _Game.Extensions.UI;
using UnityEngine;

namespace _Game.Scripts.UI
{
	public class CanvasMenu : UICanvas
	{
		// menu button
		[SerializeField] private Animator animMenuButton;
		private string animMenuButtonName = "isExpanded";
		private bool isExpanded;

		public override void SetUp()
		{
			base.SetUp();
			Messenger.Broadcast(EventKey.EffectOpenCanvas);
		}

		public void MenuButton()
		{
			animMenuButton.enabled = true;
			isExpanded = !isExpanded;

			if (isExpanded)
			{
				ChangeAnim(animMenuButton,ref  animMenuButtonName, GameConfig.ANIM_MENU_BUTTON_EXPAND);
			}
			else
			{
				ChangeAnim(animMenuButton,ref  animMenuButtonName,GameConfig.ANIM_MENU_BUTTON_COLLAPSE);
			}
		}

		public void SettingButton()
		{
			GameManager.Instance.ChangeState(GameState.SETTING);
			UIManager.Instance.OpenUI<CanvasSetting>();
		}

		public void InformationButton()
		{
			GameManager.Instance.ChangeState(GameState.INFORMATION);
			UIManager.Instance.OpenUI<CanvasInformation>();
		}

		public void ResetDataButton()
		{
			// reset data game
		}

		public void PlayButton()
		{
			Messenger.Broadcast(EventKey.EffectCloseCanvas);
			UIManager.Instance.CloseUI<CanvasMenu>(1f); // Thời gian đóng menu
			Invoke(nameof(ChangeStateGamePlay), 1f); // Thay đổi trạng thái sau khi đóng menu
		}

		private void ChangeStateGamePlay()
		{
			UIManager.Instance.OpenUI<CanvasGamePlay>();
			GameManager.Instance.PlayGame();
		}
		
		public void ShopButton()
		{
			GameManager.Instance.ChangeState(GameState.SHOP);
			UIManager.Instance.OpenUI<CanvasInformation>(); // sau doi thanh CanvasShop
		}

		public void AchievementButton()
		{
			GameManager.Instance.ChangeState(GameState.ACHIEVEMENT);
			UIManager.Instance.OpenUI<CanvasInformation>(); // sau doi thanh CanvasAchievement
		}

		public void AdsButton()
		{
			GameManager.Instance.ChangeState(GameState.ADS);
			UIManager.Instance.OpenUI<CanvasInformation>(); // sau doi thanh CanvasAds
		}
		
		private void ChangeAnim(Animator anim,ref string currentAnim, string animName)
		{
			if (currentAnim != animName)
			{
				anim.ResetTrigger(currentAnim);
				currentAnim = animName;
				anim.SetTrigger(currentAnim);
			}
		}
	}
}
