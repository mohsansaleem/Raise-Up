using System;
using System.Collections;
using System.Collections.Generic;

namespace Global
{
	public class UpdateRoomResponse
	{

		public Room room { get; set; }

		/// <summary>
		/// Examples: true
		/// </summary>
		public bool success { get; set; }

		/// <summary>
		/// Examples: "default"
		/// </summary>
		public string message { get; set; }
	}

}
