using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TienLen.Core.Models;

namespace Zems.Common.Domain.Messages
{
	public class SubmitCards : IActionMessage
	{
		public List<Card> Cards;

		public SubmitCards ()
		{
		}

		public SubmitCards (List<Card> submitCards)
		{
			this.Cards = submitCards;
		}
	}
}
