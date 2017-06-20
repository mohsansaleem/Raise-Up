using UnityEngine;
using System.Collections;


namespace Zems.Common.Domain.Messages
{
	public class DoSubmitCards : IActionMessage
	{
		public bool SubmitCards;
		public int PlayerNumber;

		public DoSubmitCards ()
		{
		}

		public DoSubmitCards (bool submitCards, int playerNumber)
		{
			this.SubmitCards = submitCards;
			this.PlayerNumber = playerNumber;
		}
	}
}
