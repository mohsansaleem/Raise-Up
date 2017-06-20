using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Game.Managers;
using Game.Enums;
using Game.Managers;
using Global;
using Facebook.Unity;

namespace Game.UI
{
	public class GameMenu : Widget<GameMenu>
	{
		[SerializeField]
		private Button
			_homeButton;
		[SerializeField]
		private Button
			_chatButton;
		[SerializeField]
		private Button
			_settingsButton;
		[SerializeField]
		private Button
			_playerProfileButton;
		[SerializeField]
		private Text
			_currencyLabel;
		[SerializeField]
		private Text
			_playerNameLabel;
		[SerializeField]
		private Button
			_game1Button;
		[SerializeField]
		private Button
			_game2Button;
		[SerializeField]
		private Button
			_game3Button;

		override public void Show (params object[] param)
		{
			DataBank.Instance.OnGameStateUpdate += PopulateUserData;

			base.Show (param);
			if (DataBank.Instance.user != null)
				PopulateUserData ();
			else
				DataBank.Instance.OnLocalGameStateLoad = PopulateUserData;

			if (DataBank.Instance.LoggedIn)
				PopulateUserData ();
			else if (!DataBank.Instance.LoggedIn && Networking.Instance.Connected)
				LogIn ();
			else if (!DataBank.Instance.LoggedIn && !Networking.Instance.Connected)
				Networking.Instance.OnConnected = LogIn;
			AudioManager.Instance.PlayBackgroundMusic (BackgroundMusicType.LogoTheme);
		}

		public override void Hide ()
		{
			base.Hide ();
			DataBank.Instance.OnGameStateUpdate -= PopulateUserData;

			AudioManager.Instance.StopBackgroundMusic ();
		}

		override public void Destroy ()
		{
			DataBank.Instance.OnGameStateUpdate -= PopulateUserData;
			base.Destroy ();

			AudioManager.Instance.StopBackgroundMusic ();
		}

		public void LogIn ()
		{
			DataBank.Instance.LogIn (onLogin: PopulateUserData);
		}

		public void PopulateUserData ()
		{
			Debug.LogError ("GameMemu.PopulateUserData called!!!");
			_currencyLabel.text = DataBank.Instance.user.gameState.wallet.chips.ToString ();
			_playerNameLabel.text = DataBank.Instance.user.username;
			if (DataBank.Instance.user.userProfile.customFields != null)
				gameObject.GetComponentInChildren<WWWLoadImage> ().LoadWithFBID (DataBank.Instance.user.userProfile.customFields.fbId);
		}

		public void OnGame1ButtonClick ()
		{
			UIManager.Instance.ShowUI (Game.Enums.GameUI.Lobby, Game.Enums.PreviousScreenVisibility.HidePreviousExcludingHud);
		}

		public void ShowUserProfile ()
		{
			UIManager.Instance.ShowNotificationMessage("Hello!!!!");
			return;
			UIManager.Instance.ShowUI (GameUI.UserProfile, PreviousScreenVisibility.DoNothing, DataBank.Instance.user);
		}

		private void OnAppInvite(Facebook.Unity.IAppInviteResult result) {
			foreach(string str in result.ResultDictionary.Values) {
				Debug.LogError("result "+str);
			}
		}

		public void OnSettingsButton ()
		{
			UIManager.Instance.ShowUI (GameUI.Settings, PreviousScreenVisibility.DoNothing, PlayerPrefs.GetFloat ("PokerVolume", 1.0f), PlayerPrefs.GetFloat ("PokerSFX", 1.0f));
		}

		public void OnShopButtonClick ()
		{
			UIManager.Instance.ShowUI (GameUI.Market, Game.Enums.PreviousScreenVisibility.HidePreviousExcludingHud);
		}
	}
}