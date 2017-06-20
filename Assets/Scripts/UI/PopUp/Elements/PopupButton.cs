using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using UnityEngine.Events;

namespace Game.UI
{
	public class PopupButton : MonoBehaviour
	{


		[SerializeField]
		private Text
			buttonText;
		[SerializeField]
		private bool
			isClickable;
		[SerializeField]
		private Action
			btnAction;
		// Use this for initialization

		public void SetData (ButtonData btnData)
		{
			buttonText.text = btnData.buttonText;
			btnAction = btnData.callback;

			GetComponent<Button> ().onClick.RemoveAllListeners ();
			if (btnData.callback != null)
				GetComponent<Button> ().onClick.AddListener (() => {
					btnAction ();
				});
       
		}

		void Start ()
		{
    	    
		}
    	
		// Update is called once per frame
		void Update ()
		{
    	
		}
	}
}