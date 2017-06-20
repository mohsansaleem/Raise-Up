using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Game.Managers;
using Game.Enums;
using Game.UI;

namespace Game.UI
{
	public class SignUp : Widget<SignUp>
	{
		[SerializeField]
		private InputField
			_usernameInput;
		[SerializeField]
		private Button
			_signupButton;
		[SerializeField]
		private Text
			_statusLabel;

		#region UserInterface implementation

		override public void Show (params object[] param)
		{
			base.Show ();
			ResetUI ();
		}
		#endregion

		#region UI Events

		public void CloseUI ()
		{
			UIManager.Instance.PopScreen ();
		}

		public void RegisterUser ()
		{
			if (string.IsNullOrEmpty (_usernameInput.text)) {
				_statusLabel.text = "Please enter username!";
				return;
			}


			_statusLabel.text = "Registering...";

			Networking.Instance.RegisterNewUser (_usernameInput.text, "123", (Reply) => {

				Debug.Log ("RegisterUser: " + Reply);
				PlayerPrefs.SetString ("userName", _usernameInput.text);
				PlayerPrefs.SetString ("password", "123");

				RegistrationSuccess ();
			}, () => {
				Action a;
				a = delegate {
					PopupManager.Instance.HidePopup ();
				};

				
				ButtonData b = new ButtonData ("Try Again", true, a);

				
				List<ButtonData> bList = new List<ButtonData> ();
				bList.Add (b);
				
				PopupData p = new PopupData ("User Registration Failed", false, bList);

				PopupManager.Instance.ShowPopup (PopupType.TwoButton, p, Vector3.zero, true);
			}
			);


		}

//        public void LoginUser()
//        {
//            if (string.IsNullOrEmpty(_usernameInput.text) || string.IsNullOrEmpty(_passwordInput.text))
//            {
//                _statusLabel.text = "Please enter username/password!";
//                return;
//            }
//
//            _statusLabel.text = "Logging...";
//            Client.Client.LoginUser(_usernameInput.text, _passwordInput.text);
//            ToggleUserInterface(false);
//        }

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

//        public void LogInSuccess()
//        {
//            _statusLabel.text = "Logged In";
//        }

//        public void LogInFailed(string message)
//        {
//            _statusLabel.text = message;
//            ToggleUserInterface(true);
//        }

		public void RegistrationSuccess ()
		{
			_statusLabel.text = "Success";


			UIManager.Instance.ShowUI (GameUI.GameMenu, PreviousScreenVisibility.DequeuePreviousExcludingHud);
		}

		public void RegistrationFailed (string param)
		{
			_statusLabel.text = param;
			ToggleUserInterface (true);
		}

		public void ConnectionErorr (string message)
		{
			_statusLabel.text = message;
			ToggleUserInterface (true);
		}

        #endregion
        
        #region Signup

        

		bool IsAllFormFieldsFilled {
			get {
				if (string.IsNullOrEmpty (_usernameInput.text)) {
					return false;
				}
				return true;
			}
		}
        #endregion

		void ToggleUserInterface (bool doEnable)
		{
			_canvasGroup.interactable = doEnable;
		}

		public void OnUserNameTextChange (InputField textBox)
		{
			if (textBox.text.Trim () != "")
				_signupButton.interactable = true;
		}

		void ResetUI ()
		{
//            _gradeInput.text = "1";     // TODO : dummy value, till the drop down list of grade is not made
			_usernameInput.text = "";
			_signupButton.interactable = false;
			_statusLabel.text = "All fields are mandatory";
		}
	}
}