using UnityEngine;
using System.Collections;


namespace Zems.Common.Domain.Messages
{
	public class PlayerNames : IActionMessage
	{
		public string[] GamePlayerNames { get; set; }

		public PlayerNames ()
		{
		}

		public PlayerNames (string[] gamePlayerNames)
		{
			this.GamePlayerNames = gamePlayerNames;
		}
	}
}
