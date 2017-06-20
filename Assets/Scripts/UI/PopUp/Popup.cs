using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Game.Managers;
using Game;
using Game.Enums;

namespace Game.UI
{

	public class Popup : MonoBehaviour
	{
        
		[SerializeField]
		private Text
			popupText;
		[SerializeField]
		private GameObject
			buttonsHolder;
		[SerializeField]
		private GameObject
			closeButton;
		[SerializeField]
		private CanvasGroup
			_canvasGroup;

		public PopupData p ;
		public PopupType pType;

		//=====================================================================================================
		// Text and 'button texts and callbacks' to show on popup!
		//  Do NOT call directly! Use PopupManager to show popups!
		public void Show (PopupType type, PopupData data)
		{

			Hide ();

			pType = type;
			p = data;
			Populate ();

			gameObject.SetActive (true);
		}

		public void Populate ()
		{
			popupText.text = p.popupText;
            
			int i = 0;
			foreach (ButtonData b in p.buttonData) {
                
				GameObject obj = Instantiate (PrefabsManager.Instance.Load (Constants.POPUP_BUTTON_PATH), Vector3.zero, Quaternion.identity) as GameObject;
				PopupButton pop = obj.GetComponent<PopupButton> ();
				pop.SetData (p.buttonData [i]);
				obj.transform.SetParent (buttonsHolder.transform, false);
				i++;
			}
            
			if (p.hasCloseButton) {
				closeButton.SetActive (true);
			} else {
				closeButton.SetActive (false);
			}
		}

        #region UIEvents

		public void CloseButtonClicked ()
		{
			PopupManager.Instance.HidePopup ();

		}



        #endregion

		public void Enable ()
		{
			_canvasGroup.alpha = 1;
			_canvasGroup.interactable = true;
			_canvasGroup.blocksRaycasts = true;
		}

		public void Disable ()
		{
			_canvasGroup.alpha = 0.7f;
			_canvasGroup.interactable = false;
			_canvasGroup.blocksRaycasts = false;
		}

		public void Hide ()
		{
			for (int i = buttonsHolder.transform.childCount - 1; i >= 0; i--) {
				Destroy (buttonsHolder.transform.GetChild (i).gameObject);
				// Debug.LogError("child count " + i);
			}

			gameObject.SetActive (false);
		}
		//=====================================================================================================


		//  //=====================================================================================================
		//  // Popup OnClick callback handler. Handles clicks on buttons ONLY!
		public void OnClick ()
		{
			Destroy (gameObject);
		}
		//  //=====================================================================================================
	}
}
