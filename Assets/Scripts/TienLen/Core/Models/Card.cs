using TienLen.Utils.Enums;
using Newtonsoft.Json;

namespace TienLen.Core.Models
{
	public class Card
	{
		public int Player;
		public int CardOrder;
		public bool IsSelected;
		public Suite Suite;
		public Value Value;

		public Card ()
		{
			IsSelected = false;
		}

		public Card (Suite suite, Value number)
		{
			this.Suite = suite;
			this.Value = number;
		}
		

	}
}