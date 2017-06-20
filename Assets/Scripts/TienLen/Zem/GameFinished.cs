using UnityEngine;
using System.Collections;
using TienLen.Core.Models;

namespace Zems.Common.Domain.Messages
{
	public class GameFinished : IActionMessage
	{
		private Player[] players;

		public Player[] Players {
			get {
				return players;
			}

			set {
				players = value;
			}
		}

		public GameFinished ()
		{

		}

		public GameFinished (Player[] players)
		{
			Players = players;
		}
	}
}
