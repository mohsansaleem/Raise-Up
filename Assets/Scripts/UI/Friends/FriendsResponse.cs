using System;
using System.Collections;
using System.Collections.Generic;

namespace Global
{
	public class FriendsResponse
	{
		
		public List<User> friends { get; set; }
		public List<User> requests { get; set; }

		public bool success { get; set; }
	
		public string correlationId { get; set; }
	}
	
}
