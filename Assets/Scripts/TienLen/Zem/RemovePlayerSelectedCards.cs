using UnityEngine;
using System.Collections;


namespace Zems.Common.Domain.Messages
{
	public class RemovePlayerSelectedCards : IActionMessage
	{
		public string[] CardNames { get; set; }

		public RemovePlayerSelectedCards ()
		{
		}

		public RemovePlayerSelectedCards (string[] cardNames)
		{
			this.CardNames = cardNames;
		}
	}
}
