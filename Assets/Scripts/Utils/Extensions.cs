using UnityEngine;
using System.Collections;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using Newtonsoft.Json.Serialization;

public static class Extensions {

	public class MyContractResolver : Newtonsoft.Json.Serialization.DefaultContractResolver
	{
		protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
		{
			var props = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
				.Select(p => base.CreateProperty(p, memberSerialization))
					.Union(type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
					       .Select(f => base.CreateProperty(f, memberSerialization)))
					.ToList();
			props.ForEach(p => { p.Writable = true; p.Readable = true; });
			return props;
		}
	}

	public static void PerformActionWithDelay(this MonoBehaviour mono, float delay, Action callback = null, Action callBackParam = null)
	{
		mono.StartCoroutine(mono.PerformActionWithDelayRoutine(delay, callback, callBackParam));
	}
	
	private static IEnumerator PerformActionWithDelayRoutine(this MonoBehaviour ienum, float delay, Action callback, Action callBackParam = null)
	{
		yield return new WaitForSeconds(delay);

		if(callback != null)
			callback();

		if(callBackParam != null)
			callBackParam();
	}

	public static CardInfo GetCardInfo(this MonoBehaviour mono)
	{
		return mono.GetComponent<CardInfo>();
	}

	public static string ToJson (this object data)
	{
		return JsonConvert.SerializeObject (data);
	}
	
	public static JObject ToJsonObject(this object data)
	{
		return JObject.Parse(data.ToJson());
	}
	
	public static string ToJsonArray (this object data)
	{
		return JsonConvert.SerializeObject(data, Formatting.None , new JsonSerializerSettings
		                                   {
			TypeNameHandling = TypeNameHandling.None
			//ContractResolver = new MyContractResolver()
		});
	}
	
	public static string ToJson (this object data, TypeNameHandling type)
	{
		return data.ToJson (new JsonSerializerSettings() {
			TypeNameHandling = type
			//ContractResolver = new MyContractResolver()
		});
	}
	
	public static string ToJson (this object data, JsonSerializerSettings settings)
	{
		return JsonConvert.SerializeObject (data, Formatting.None, settings);
	}
	
	public static T ToObject <T> (this string json)
	{
		return JsonConvert.DeserializeObject<T> (json);
	}
	
	public static T ToObject <T> (this string json, TypeNameHandling type)
	{
		return json.ToObject<T> (new JsonSerializerSettings() {
			TypeNameHandling = type
			//ContractResolver = new MyContractResolver()
		});
	}
	
	public static T ToObject <T> (this string json, JsonSerializerSettings settings)
	{
		return JsonConvert.DeserializeObject<T> (json, settings);
	}
	
	public static object ToObject (this string json)
	{
		return JsonConvert.DeserializeObject (json);
	}
	
	public static object ToObject (this string json, TypeNameHandling type)
	{
		return json.ToObject (new JsonSerializerSettings() {
			TypeNameHandling = type
			//ContractResolver = new MyContractResolver()
		});
	}
	
	public static object ToObject (this string json, JsonSerializerSettings settings)
	{
		return JsonConvert.DeserializeObject (json, settings);
	}
}