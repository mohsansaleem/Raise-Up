using UnityEngine;
using System.Collections;


namespace Zems.Common.Domain.Messages
{
	public class GetGameState : IActionMessage
	{
		public string PlayerId;

		public GetGameState ()
		{
			PlayerId = "";
		}

		public GetGameState (string playerId)
		{
			this.PlayerId = playerId;
		}
	}
}
