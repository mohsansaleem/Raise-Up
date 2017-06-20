
using System;
using System.Collections.Generic;

namespace Global
{

	public class GameStateResponse
	{
		/// <summary>
		/// Examples: true
		/// </summary>
		public bool success { get; set; }

		/// <summary>
		/// Examples: "fd8ff25b-1585-4129-abb3-472da31112ac"
		/// </summary>
		public string correlationId { get; set; }

		public User user { get; set; }
	}

}
