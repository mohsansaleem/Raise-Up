using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TienLen.Core.Models;

namespace Zems.Common.Domain.Messages
{
	public class GetGameStateResponse : IActionMessage
	{
		public string PlayerId;

		public int PlayerTurn;

		public Player[] Players;

		public GetGameStateResponse ()
		{
			PlayerId = "";
			PlayerTurn = -1;
		}

		public GetGameStateResponse (string playerID, int playerTurn, Player[] players)
		{
			PlayerId = playerID;
			Players = players;
			PlayerTurn = playerTurn;
		}
	}
}
