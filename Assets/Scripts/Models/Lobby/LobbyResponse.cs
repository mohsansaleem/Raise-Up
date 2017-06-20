using System;
using System.Collections;
using System.Collections.Generic;

namespace Global
{
	public class LobbyResponse
	{

		public List<Room> rooms { get; set; }

		/// <summary>
		/// Examples: true
		/// </summary>
		public bool success { get; set; }

		/// <summary>
		/// Examples: "default"
		/// </summary>
		public string name { get; set; }

		/// <summary>
		/// Examples: "da11711c-26d7-47fc-abf1-187a66dd431f"
		/// </summary>
		public string correlationId { get; set; }
	}

}
