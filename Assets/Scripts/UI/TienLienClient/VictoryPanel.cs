using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace TienLen.Core.GamePlay
{
	public class VictoryPanel : MonoBehaviour
	{
		public Text WinLoseText;
		public GameObject WinningImage;
		public Text ResultNames;
		public Text ResultScore;
		public Button LobbyButton;
		public Button ReplayButton;


		public void PopulateData (string resultNames, string resultScores, string result)
		{
			Result = result;
			ResultNames.text = resultNames;
			ResultScore.text = resultScores;
		}

		string Result {
			set {
				WinLoseText.text = value;
				WinningImage.SetActive ( value.Equals("You Win"));
			}
		}

	}
}
