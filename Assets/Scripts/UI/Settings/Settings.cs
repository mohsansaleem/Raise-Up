using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Global;
using Game.Managers;
using TienLen.Core.Models;
using Facebook.Unity;
using Newtonsoft.Json;
using TienLen.Managers;

namespace Game.UI
{
	
	
	public class Settings : Widget<Settings> {
		

		public Slider volumeSlider;
		public Slider sfxSlider;
		public Button facebookConnectBtn;
		public Button helpAndSupportBtn;

		
		override public void Show (params object[] param)
		{
			base.Show (param);
			Debug.Log ("Settings Screen-> Volume: " + (float)param[0] + " SFX: " + (float)param[1]);
			volumeSlider.value = (float)param [0];
			sfxSlider.value = (float)param [1];
			//ResetUI ();
			if (FB.IsLoggedIn) {
				Debug.Log("Facebook is LoggedIn already!");
				facebookConnectBtn.interactable = false;
			}

		}
		
		public void OnFacebookButton(){
			if (Application.platform != RuntimePlatform.OSXEditor) {
				if (!FB.IsLoggedIn) {
					FacebookManager.Instance.LoginWithReadPermissions ();
				} else {
					Debug.Log ("Facebook is LoggedIn already!");
					facebookConnectBtn.interactable = false;
				}
			} else {
				if (!FB.IsLoggedIn) {
					Debug.Log ("Doesn't work in editor, Use GUI login Button for Login!");
				} else {
					Debug.Log ("Facebook is LoggedIn already!");
					facebookConnectBtn.interactable = false;
				}
			}
		}

		public void OnHelpAndSupportButton(){
			string email = "gruvmonkeybiz@gmail.com";
			string subject = MyEscapeURL("Raise Up Support");
			Application.OpenURL("mailto:" + email + "?subject=" + subject);
		}

		string MyEscapeURL (string url)
		{
			return WWW.EscapeURL(url).Replace("+","%20");
		}

		public void OnVolumeSliderDrag(Slider slider){
			Debug.Log ("Volume Slider Value: " + slider.value);
		}

		public void OnSFXSliderDrag(Slider slider){
			Debug.Log ("SFX Slider Value: " + slider.value);
		}

		public void OnCloseButton(){
			PlayerPrefs.SetFloat ("PokerVolume",volumeSlider.value);
			PlayerPrefs.SetFloat ("PokerSFX",sfxSlider.value);
			UIManager.Instance.HideUI (Game.Enums.GameUI.Settings);
		}

	}
}
