using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace Game.UI
{
	public class PopupData
	{

		public string popupText;
		public bool hasCloseButton;
		public List<ButtonData> buttonData;

		public PopupData (string val1, bool val2, List <ButtonData> btnData)
		{
			popupText = val1;
			hasCloseButton = val2;
			//buttonData = new List<ButtonData> ();
			buttonData = btnData;
		}

	}
}