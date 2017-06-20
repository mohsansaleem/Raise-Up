using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Game.Enums;
using Game.UI;
using System.Collections.Generic;
using System;
using Game.Managers;
using Game;

namespace Game.UI
{
	public class Hud : Widget<Hud>
	{
		[SerializeField]
		private Button
			_siginButton;
		[SerializeField]
		private Button
			_signupButton;
		[SerializeField]
		private Button
			_othersButton;
		[SerializeField]
		private Button
			_popUpButton;

        #region UI Events

		public void SignInButtonClicked ()
		{
			UIManager.Instance.ShowUI (GameUI.Login, PreviousScreenVisibility.HidePreviousExcludingHud);
		}

		public void SigUpButtonClicked ()
		{
			UIManager.Instance.ShowUI (GameUI.Signup, PreviousScreenVisibility.HidePreviousExcludingHud);
		}

		//Popup Sample
		public void PopUpButtonClicked ()
		{
			Action a, a2;
			a = delegate {
				PopupManager.Instance.HidePopup ();
			};

			a2 = delegate {
				Action a3 = delegate {
					PopupManager.Instance.HidePopup ();
				};
				ButtonData b1 = new ButtonData ("showPrev", true, a3);
                
				List<ButtonData> bList1 = new List<ButtonData> ();
				bList1.Add (b1);
                
				PopupData p1 = new PopupData ("one button!!!", true, bList1);
              

				PopupManager.Instance.ShowPopup (PopupType.OneButton, p1, Vector3.zero, false);
			};
          
			ButtonData b = new ButtonData ("showNext", true, a2);
			ButtonData b2 = new ButtonData ("close", true, a);
           
			List<ButtonData> bList = new List<ButtonData> ();
			bList.Add (b);
			bList.Add (b2);
           
			PopupData p = new PopupData ("two button!!!", true, bList);
			PopupManager.Instance.ShowPopup (PopupType.TwoButton, p, Vector3.zero, true);
		}

        #endregion

		void ToggleUserInterface (bool doEnable)
		{
			_canvasGroup.interactable = doEnable;
		}

	}
}
