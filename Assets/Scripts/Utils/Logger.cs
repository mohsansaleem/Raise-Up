using UnityEngine;
using System;

public enum LogType { Normal, Warning, Error, Exception}

public static class Logger  {
	
	public static bool LogEnable = true;
	
	//public static void Log(this MonoBehaviour contextObject, object message, LogType logType = LogType.Normal) 
	public static void Log(object message, LogType logType = LogType.Normal) 
	{
		if(LogEnable)
		{
			switch(logType)
			{
			case LogType.Normal:
				//Debug.Log(message, ReferenceEquals(contextObject, null) ? contextObject.gameObject : null);
				Debug.Log(message);
				break;
				
			case LogType.Warning:
				//Debug.LogWarning(message, ReferenceEquals(contextObject, null) ? contextObject.gameObject : null);
				Debug.LogWarning(message);
				break;
				
			case LogType.Error:
				//Debug.LogError(message, ReferenceEquals(contextObject, null) ? contextObject.gameObject : null);
				Debug.LogError(message);
				break;
				
			case LogType.Exception:
				//Debug.LogException((Exception) message, ReferenceEquals(contextObject, null) ? contextObject.gameObject : null);
				Debug.LogException((Exception)message);
				break;
			}
		}
	}
}
