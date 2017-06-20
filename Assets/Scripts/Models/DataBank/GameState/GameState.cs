
using System;
using System.Collections.Generic;

namespace Global
{

	public class GameState
	{

		public Wallet wallet { get; set; }

		/// <summary>
		/// Examples: 1000
		/// </summary>
		public int rewardPoints { get; set; }

		public IList<object> games { get; set; }

		public int version;
	}

}
