using UnityEngine;
using System.Collections;


namespace Zems.Common.Domain.Messages
{
	public class PlayerTurn : IActionMessage
	{
		public int PlayersTurn;
		public string Id;
		public PlayerTurn ()
		{
		}

		public PlayerTurn (int playersTurn, string id)
		{
			this.PlayersTurn = playersTurn;
			Id = id;
		}
	}
}
