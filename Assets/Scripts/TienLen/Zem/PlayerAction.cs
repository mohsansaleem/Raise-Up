using UnityEngine;
using System.Collections;
using TienLen.Utils.Enums;

namespace Zems.Common.Domain.Messages
{
	public class PlayerAction : IActionMessage
	{
		public ActionType PlayerActionType;

		public PlayerAction ()
		{
		}

		public PlayerAction (ActionType playerActionType)
		{
			this.PlayerActionType = playerActionType;
		}
	}
}
