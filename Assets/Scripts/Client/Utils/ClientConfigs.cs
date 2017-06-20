using UnityEngine;
using System.Collections;

namespace Client.Utils
{
	public static class ClientConfigs {

		public const string SERVER_IP = "192.168.9.74";
		public const int SERVER_PORT = 10000;

		public const char DATA_SEPERATOR = '|';
		public const char INNER_DATA_SEPERATOR = ':';

		public const int READ_BUFFER_SIZE = 8191;
	}
}