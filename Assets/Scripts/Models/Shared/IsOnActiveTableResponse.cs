using System;
using System.Collections;
using System.Collections.Generic;

namespace Global
{
	public class IsOnActiveTableResponse
	{
		/// <summary>
		/// Examples: true
		/// </summary>
		public bool success { get; set; }

		public string roomId { get; set; }

		public bool Status{ get; set; }

		/// <summary>
		/// Examples: "da11711c-26d7-47fc-abf1-187a66dd431f"
		/// </summary>
		public string correlationId { get; set; }
	}

}
