using UnityEngine;
using System.Collections;
using TienLen.Core.Models;

namespace Zems.Common.Domain.Messages
{
	public class PlayerState : IActionMessage
	{
		public Player Player;

		public PlayerState ()
		{
		}

		public PlayerState (Player player)
		{
			this.Player = player;
		}
	}
}
