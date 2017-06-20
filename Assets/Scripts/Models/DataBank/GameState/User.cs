using System;
using System.Collections.Generic;
using TienLen.Utils.Enums;

namespace Global
{

	public class User
	{

		/// <summary>
		/// Examples: 111
		/// </summary>
		public int userID { get; set; }

		public UserType userType;

		public int seat;

		public bool startVote;

		/// <summary>
		/// Examples: "Abc"
		/// </summary>
		public string username { get; set; }

		public GameState gameState { get; set; }

		/// <summary>
		/// Examples: null
		/// </summary>
		public object _id { get; set; }

		public UserProfile userProfile;
	}

}
