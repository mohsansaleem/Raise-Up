
using System;
using System.Collections.Generic;
using TienLen.Utils.Enums;

namespace Global
{

	public class Room
	{

		/// <summary>
		/// Examples: 1000, 5000, 10000, 20000, 50000
		/// </summary>
		public int buyIn { get; set; }

		/// <summary>
		/// Examples: "Tien Lien"
		/// </summary>
		public string gameType { get; set; }

		/// <summary>
		/// Examples: 3, 2, 1
		/// </summary>
		public int roomQuantity { get; set; }

		/// <summary>
		/// Examples: 4
		/// </summary>
		public int roomSize { get; set; }

		/// <summary>
		/// Examples: "tienLien_1000_4", "tienLien_5000_4", "tienLien_10000_4", "tienLien_20000_4", "tienLien_50000_4"
		/// </summary>
		public string id { get; set; }

		/// <summary>
		/// Examples: "Newbie Room", "Amateur Room", "Intermediate Room", "Expert Room", "Pro Room"
		/// </summary>
		public string roomName { get; set; }


		public RoomStatus status;

		public List<User> roomUsers = new List<User> ();
	}

}
