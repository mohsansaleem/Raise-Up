using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using UnityEngine.Events;

namespace Game.UI
{
	public class ButtonData
	{

		public string buttonText;
		public bool isClickable;
		public Action callback;

		public ButtonData (string val1, bool val2, Action val3)
		{
			buttonText = val1;
			isClickable = val2;
			callback = val3;

           
		}


	}
}