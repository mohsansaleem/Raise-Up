using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Game;

namespace TienLen.View
{
	public class ButtonProps
	{

		public string text;
		public UnityAction callback;
	}

	public class PopupHandler : Singleton<PopupHandler>
	{

		[SerializeField]
		private Text popupText;
		[SerializeField]
		private Button[] popupButtons;
		[SerializeField]
		private GameObject parentCanvas;
		[SerializeField]
		private GameObject popup;

		public void Show (string text, params ButtonProps[] buttonsProp)
		{
			parentCanvas.SetActive (true);
			popup.SetActive (true);

			popupText.text = text;

			for (int i = 0; i < buttonsProp.Length; i++) {
				popupButtons [i].gameObject.SetActive (true);
				popupButtons [i].GetComponentInChildren<Text> ().text = buttonsProp [i].text;
				popupButtons [i].onClick.RemoveAllListeners ();
				popupButtons [i].onClick.AddListener (Hide);

				if (buttonsProp [i].callback != null)
					popupButtons [i].onClick.AddListener (buttonsProp [i].callback);
			}
		}

		public void Hide ()
		{
			parentCanvas.SetActive (false);
			popup.SetActive (false);

			foreach (Button button in popupButtons) {

				button.gameObject.SetActive (false);
			}
		}
	}
}