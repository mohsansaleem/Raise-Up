using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace TienLen.Utils
{
	public static class ResourceHelper {
		
		private static Dictionary<string, object> _cacheDictionary;
		
		public static GameObject FetchAndInstatiateResource (string path, Vector3 position, Quaternion rotation, Transform parent)
		{
			GameObject obj = TryFetchResource<GameObject> (path);
			
			if(obj != null)
			{
				GameObject tempObj = GameObject.Instantiate(obj, position, rotation) as GameObject;
				tempObj.transform.parent = parent;
				tempObj.transform.localPosition = position;
				tempObj.transform.localRotation = rotation;
				return tempObj;
			}
			
			return obj;
		}
		
		public static GameObject FetchAndInstatiateResource (string path, Vector3 position, Quaternion rotation)
		{
			GameObject obj = TryFetchResource<GameObject> (path);
			
			if(obj != null)
			{
				return GameObject.Instantiate(obj, position, rotation) as GameObject;
			}
			
			return obj;
		}
		
		public static GameObject FetchAndInstatiateResource (string path)
		{
			GameObject obj = TryFetchResource<GameObject> (path);
			
			if(obj != null)
			{
				return GameObject.Instantiate(obj) as GameObject;
			}
			
			return obj;
		}
		
		public static T TryFetchResource<T> (string path) where T : UnityEngine.Object
		{
			T obj = FetchResource<T>(path);
			
			if(obj)
			{
				return obj;
			}
			else
			{
				obj = TryLoadResource<T>(path);
				
				if(obj)
				{
					_cacheDictionary.Add(path, obj);
					return obj;
				}
				
				return obj;
			}
		}
		
		private static T FetchResource<T> (string path) where T : UnityEngine.Object
		{
			object objectToFind = null;
			
			if(_cacheDictionary == null)
			{
				_cacheDictionary = new Dictionary<string, object>();
				return default (T);
			}
			
			if(_cacheDictionary.TryGetValue(path, out objectToFind))
			{
				return (T)objectToFind;
			}
			
			return default (T);
		}
		
		private static T TryLoadResource<T> (string path) where T : UnityEngine.Object
		{
			try
			{
				T obj = LoadResource<T> (path);
				return obj;
			}
			catch(Exception e)
			{
				Debug.LogException(e);
				return default (T);
			}
		}
		
		private static T LoadResource<T> (string path) where T : UnityEngine.Object
		{
			T obj = Resources.Load<T>(path) as T;
			
			return obj;
		}
	}
}