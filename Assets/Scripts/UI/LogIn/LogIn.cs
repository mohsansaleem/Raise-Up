using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Game.Managers;
using Game.Enums;
using Newtonsoft.Json;
using Global;
using TienLen.Managers;
using Zems.Common.Domain.Messages;
using TienLen.Core.Models;

namespace Game.UI
{
	public class LogIn : Widget<LogIn>
	{
		[SerializeField]
		private Button
			_loginButton;
		[SerializeField]
		private Button
			_registerButton;
		[SerializeField]
		private Text
			_statusLabel;

		static int used = 0;

		override public void Show (params object[] param)
		{
			base.Show (param);
			//ResetUI ();
			_loginButton.interactable = PlayerPrefs.HasKey ("userName");
			_registerButton.interactable = !PlayerPrefs.HasKey ("userName");

			AudioManager.Instance.PlayBackgroundMusic (BackgroundMusicType.LogoTheme);
		}

		#region UI Events

		public void RegisterUser ()
		{
			UIManager.Instance.ShowUI (GameUI.Signup, PreviousScreenVisibility.HidePreviousExcludingHud);
//            if (string.IsNullOrEmpty(_usernameInput.text) || string.IsNullOrEmpty(_passwordInput.text))
//            {
//                _statusLabel.text = "Please enter username/password!";
//                return;
//            }
//
//            _statusLabel.text = "Registering...";
//            Client.Client.RegisterUser(_usernameInput.text, _passwordInput.text);
//            ToggleUserInterface(false);
		}

		public void LoginUser ()
		{
			_statusLabel.text = "Logging...";

			ToggleUserInterface (false);

			used++;

//			Debug.LogError ("Fuck fuck");
//			Card card = new Card (TienLen.Utils.Enums.Suite.Diamonds, TienLen.Utils.Enums.Value.Ace);
//
//
//			string str = (string)JsonConvert.SerializeObject (card, Formatting.None, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });
//			JsonConvert.DeserializeObject<Card> (str, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });
//
//
//			Debug.LogError ("Before Cerds:");
//			List<Card> cards = new List<Card> ();
//
//			cards.Add (card);
//			str = (string)JsonConvert.SerializeObject (cards, Formatting.None, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });
//			JsonConvert.DeserializeObject<List<Card>> (str, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });
//			Debug.LogError ("After Cards");
//
//			PlayerState ps = new PlayerState ();
//			ps.Player = new Player ("abc", TienLen.Utils.Enums.UserType.Player, 1, "id");
//			ps.Player.cards = cards;
//		
//			try {
//				Debug.LogError ("Before Serilize\n" + str);
//				str = (string)JsonConvert.SerializeObject (ps, Formatting.None, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });
//				Debug.LogError ("after Serilize\n" + str);
//				string msgString = str.Replace ("TienLen.Engine", "Assembly-CSharp");
//				IActionMessage IA = JsonConvert.DeserializeObject<IActionMessage> (str, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });
//
//				Debug.LogError ("After Player.\n" + msgString);
//			} catch (System.Exception e) {
//				Debug.LogError (">>>>>>>>>>>>>" + e.Message + " \n " + e.StackTrace + " \n " + e.Source);
//			}

			//Debug.LogError ("Used: " + used);
			//GameManager.Instance.StartOffline ();

//			List<Card> cards = new List<Card> ();
//			Card card = new Card (TienLen.Utils.Enums.Suite.Diamonds, TienLen.Utils.Enums.Value.Ace);
//			cards.Add (card);
//			SubmitCards submitCards = new SubmitCards (cards);
//
//			string msg = JsonConvert.SerializeObject (submitCards, Formatting.None, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });
//			Debug.LogError ("Serialized: " + msg);
//			ICoreMessage coreMessage = (ICoreMessage)JsonConvert.DeserializeObject (msg, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });
//			Networking.Instance.SentMessageToGameEngine ("abc", submitCards, (str) => {
//				Debug.Log ("SubmitCards Message Sent: " + str);}, () => {});

//
//
			DataBank.Instance.LogIn (() => {
				UIManager.Instance.ShowUI (GameUI.Lobby, PreviousScreenVisibility.DequeuePreviousExcludingHud);});
		}

		override public void Hide ()
		{
			AudioManager.Instance.StopBackgroundMusic ();
		}

		override public void Destroy ()
		{
			AudioManager.Instance.StopBackgroundMusic ();
		}

//		public void PlayOffline ()
//		{
//			GameManager.Instance.StartOffline ();
//		}

		#endregion

		#region ClientEvents callbacks

		public void ConnectingToServer ()
		{
			_statusLabel.text = "Connecting to server!";
		}

		public void ConnectedToServer ()
		{
			_statusLabel.text = "Connected!";
		}

		public void LogInSuccess ()
		{
			_statusLabel.text = "Logged In";
		}

		public void LogInFailed (string message)
		{
			_statusLabel.text = message;
			ToggleUserInterface (true);
		}

//        public void RegistrationSuccess()
//        {
//            _statusLabel.text = "Success";
//            ToggleUserInterface(true);
//        }
//
//        public void RegistrationFailed(string param)
//        {
//            _statusLabel.text = param;
//            ToggleUserInterface(true);
//        }

		public void ConnectionErorr (string message)
		{
			_statusLabel.text = message;
			ToggleUserInterface (true);
		}

		#endregion

		void ToggleUserInterface (bool doEnable)
		{
			_canvasGroup.interactable = doEnable;
		}

		void ResetUI ()
		{
			_loginButton.interactable = false;
			//_statusLabel.text = "Enter Username and Password";
		}
	}
}