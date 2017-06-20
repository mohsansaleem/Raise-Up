using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using Game;
//using System.Text;
//using System.Security.Cryptography;

namespace Global
{
	public class ResourceHelper : Singleton<ResourceHelper>
	{
		public static Dictionary<string, UnityEngine.Object> CachedResources = new Dictionary<string, UnityEngine.Object> ();

		protected static IEnumerator LoadAsyncInternal (string path, Action<WWW> onLoad)
		{
			WWW www = new WWW (LocalToURLPath (path));
			yield return www;
			onLoad (www);
		}

		public static T LoadCached<T> (string filePath) where T : UnityEngine.Object
		{
			if (! CachedResources.ContainsKey (filePath))
				CachedResources.Add (filePath, Resources.Load<T> (filePath));
			return CachedResources [filePath] as T;
		}

		public static void LoadAsync (string filePath, Action<WWW> onLoad)
		{
			Instance.StartCoroutine (LoadAsyncInternal (filePath, onLoad));
		}

		public static void LoadFromStreamingAssetsAsync (string file, Action<WWW> onLoad)
		{
			LoadAsync (GetStreamingAssetsPath (file), onLoad);
		}

		public static void LoadFromPersistentDataAsync (string file, Action<WWW> onLoad)
		{
			LoadAsync (GetPersistentDataPath (file), onLoad);
		}

		public static string Read (string path)
		{
			return File.ReadAllText (path);
		}
	
		public static T Read<T> (string path)
		{
			return DecodeObject<T> (Read (path));
		}
	
		public static string ReadFromPersistentData (string file)
		{
			return Read (GetPersistentDataPath (file));
		}
	
		public static T ReadFromPersistentData<T> (string file)
		{
			return Read<T> (GetPersistentDataPath (file));
		}
	
		public static string ReadFromStreamingAssets (string file)
		{
			return Read (GetStreamingAssetsPath (file));
		}
	
		public static T ReadFromStreamingAssets<T> (string file)
		{
			return Read<T> (GetStreamingAssetsPath (file));
		}

		public static void ReadAsync (string path, Action<string> onRead)
		{
			LoadAsync (path, (WWW www) => {
				onRead (www.text);
			});
		}

		public static void ReadAsync<T> (string path, Action<T> onRead)
		{
			ReadAsync (path, (string content) => {
				onRead (DecodeObject<T> (content));
			});
		}

		public static void ReadFromPersistentDataAsync (string file, Action<string> onRead)
		{
			ReadAsync (GetPersistentDataPath (file), onRead);
		}

		public static void ReadFromPersistentDataAsync<T> (string file, Action<T> onRead)
		{
			ReadAsync<T> (GetPersistentDataPath (file), onRead);
		}

		public static void ReadFromStreamingAssetsAsync (string file, Action<string> onRead)
		{
			ReadAsync (GetStreamingAssetsPath (file), onRead);
		}

		public static void ReadFromStreamingAssetsAsync<T> (string file, Action<T> onRead)
		{
			ReadAsync<T> (GetStreamingAssetsPath (file), onRead);
		}

		public static void Save (string path, string contents)
		{
			File.WriteAllText (path, contents);
		}

		public static void Save (string path, object contents)
		{
			Save (path, EncodeObject (contents));
		}

		public static void SaveToPersistentData (string file, string contents)
		{
			Save (GetPersistentDataPath (file), contents);
		}
	
		public static void SaveToPersistentData (string file, object contents)
		{
			Save (GetPersistentDataPath (file), contents);
		}

		public static void SaveToStreamingAssets (string file, string contents)
		{
			Save (GetStreamingAssetsPath (file), contents);
		}

		public static void SaveToStreamingAssets (string file, object contents)
		{
			Save (GetStreamingAssetsPath (file), contents);
		}

		public static bool Exists (string path)
		{
			return File.Exists (path);
		}

		public static bool ExistsInPersistentData (string file)
		{
			return Exists (GetPersistentDataPath (file));
		}

		public static bool ExistsInStreamingAssets (string file)
		{
			return Exists (GetStreamingAssetsPath (file));
		}

		public static void Delete (string path)
		{
			File.Delete (path);
		}

		public static void DeleteFromPersistentData (string file)
		{
			Delete (GetPersistentDataPath (file));
		}

		public static void DeleteFromStreamingAssets (string file)
		{
			Delete (GetStreamingAssetsPath (file));
		}
	
		public static string GetPersistentDataPath (string file)
		{
			#if UNITY_EDITOR
			return Path.Combine (Application.streamingAssetsPath, file);
//		Debug.Log("Unity Editor");
		
			#elif UNITY_IPHONE || UNITY_ANDROID
		return Path.Combine(Application.persistentDataPath, file);
//		Debug.Log("Unity iPhone");
		
			#else
			Debug.Log ("Any other platform");
			return Path.Combine (Application.persistentDataPath, file);
		
			#endif
		
//		return Path.Combine(Application.persistentDataPath, file);
		}

		public static string GetStreamingAssetsPath (string file)
		{
			return Path.Combine (Application.streamingAssetsPath, file);
		}

		public static string LocalToURLPath (string path)
		{
			if (! path.Contains ("file://"))
				path = "file://" + path;
			return path;
		}

		public static T DecodeObject<T> (string encoded)
		{
			return JsonConvert.DeserializeObject<T> (encoded);
		}
	
		public static string EncodeObject (object data)
		{
			return JsonConvert.SerializeObject (data, Formatting.Indented);
		}
	
	
	#region FILE READ and WRITE
		public static void CopyFileFromStreamingToPersistentPath (string debugString, string debugStringLong, string fileName)
		{
			try {
				if (ExistsInPersistentData (fileName)) {
				} else { // file not exists in persistent data, save it there
					Debug.Log (debugStringLong + ": Copied " + debugString + " from streamingAssets to persistent data path");
					// reading from local file should be Asynchronous call
					ReadFromStreamingAssetsAsync (fileName, (readData) => {
						SaveToPersistentData (fileName, readData); });
				}
			} catch (System.Exception e) {
				Debug.Log (debugStringLong + ": Exception occured on Copying " + debugString + ": " + e.Message);
			}
		}
	#endregion FILE READ and WRITE
	
	#region ENCRYPT and DECRYPT
/*	void Start () {
		string str = "This is a book{}:\"";
/*		str = Base64Encode(str);
		Debug.Log("Encode: " + str);
		this.PerformActionWithDelay(1f, () => {
									str = Base64Decode(str);
									Debug.Log("Decode: " + str);
								});* /
		str = Caesar(str, 1);
		Debug.Log("Encode: " + str);
		this.PerformActionWithDelay(1f, () => {
											str = Caesar(str, -1);
											Debug.Log("Decode: " + str);
										});
/*		str = Encrypt(str);
		Debug.Log("Encode: " + str);
		this.PerformActionWithDelay(1f, () => {
											str = Decrypt(str);
											Debug.Log("Decode: " + str);
										});* /
	}*/
		private static string Caesar (string value, int shift)
		{ // pass +1 to Encrypt and -1 to Decrypt
//		Debug.Log("Caesar-1: " + value + "   Shift: " + shift.ToString());
//		if (shift > 0) { value = Encrypt(value); }	// encrypt
//		if (shift > 0) { value = Base64Encode(value); }	// encode
//		Debug.Log("Caesar-2: " + value + "   Shift: " + shift.ToString());
		
			char[] buffer = value.ToCharArray ();
			for (int i = 0; i < buffer.Length; i++) {
				// Letter.
				char letter = buffer [i];
//			Debug.Log(shift + " Letter-1: " + letter);
				// Add shift to all.
				letter = (char)(letter + shift);
//			Debug.Log(shift + " Letter-2: " + letter);
			
				buffer [i] = letter;
			}
		
//		Debug.Log("Caesar-3: " + new string(buffer) + "   Shift: " + shift.ToString());
//		if (shift < 0) { value = Decrypt( new string(buffer) ); }	// decode
//		if (shift < 0) { value = Base64Decode(value); }	// decode
//		Debug.Log("Caesar-4: " + value + "   Shift: " + shift.ToString());
			return new string (buffer);
//		return value;
		}
/*	private static string Base64Encode(string plainText) {
		var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
		return System.Convert.ToBase64String(plainTextBytes);
	}
	private static string Base64Decode(string base64EncodedData) {
		var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
		return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
	}
	
	// Reference: http://www.aspsnippets.com/Articles/AES-Encryption-Decryption-Cryptography-Tutorial-with-example-in-ASPNet-using-C-and-VBNet.aspx
	private static string Encrypt(string clearText)
	{
		string EncryptionKey = "MAKV2SPBNI99212";
		byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
		using (Aes encryptor = Aes.Create())
		{
			Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
			encryptor.Key = pdb.GetBytes(32);
			encryptor.IV = pdb.GetBytes(16);
			using (MemoryStream ms = new MemoryStream())
			{
				using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
				{
					cs.Write(clearBytes, 0, clearBytes.Length);
					cs.Close();
				}
				clearText = Convert.ToBase64String(ms.ToArray());
			}
		}
		return clearText;
	}
	private static string Decrypt(string cipherText)
	{
		string EncryptionKey = "MAKV2SPBNI99212";
		byte[] cipherBytes = Convert.FromBase64String(cipherText);
		using (Aes encryptor = Aes.Create())
		{
			Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
			encryptor.Key = pdb.GetBytes(32);
			encryptor.IV = pdb.GetBytes(16);
			using (MemoryStream ms = new MemoryStream())
			{
				using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
				{
					cs.Write(cipherBytes, 0, cipherBytes.Length);
					cs.Close();
				}
				cipherText = Encoding.Unicode.GetString(ms.ToArray());
			}
		}
		return cipherText;
	}*/
	#endregion ENCRYPT and DECRYPT
	}
}