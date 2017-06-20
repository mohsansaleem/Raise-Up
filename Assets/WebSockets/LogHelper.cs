using UnityEngine;
using System.Collections;
using WebSocket_Client;

namespace LogMessages
{
	public static class LogHelper
	{
		public static void Log<T> (this T GO, string StringToPrint) where T : class
		{
			if (WebSocket_Client.Client.PrintLogs)
				Debug.Log (StringToPrint);
		}

	}
}