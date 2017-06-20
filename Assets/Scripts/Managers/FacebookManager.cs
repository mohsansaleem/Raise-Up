using UnityEngine;
using System.Collections;
using Game;
using Facebook.Unity;
using System.Collections.Generic;
using Global;
using Newtonsoft.Json;

namespace TienLen.Managers
{
	public class FacebookManager : Singleton<FacebookManager>
	{

		//$(PROJECT_DIR)/Frameworks/FacebookSDK/Plugins/iOS 

		public enum Permissions
		{
			public_profile = 1,
			email = 2,
			user_friends = 3
		}

		// Awake function from Unity's MonoBehavior
		void Awake ()
		{
			if (!FB.IsInitialized) {
				// Initialize the Facebook SDK
				FB.Init (InitCallback, OnHideUnity);
				Debug.Log ("FB Init Called");
			} else {
				// Already initialized, signal an app activation App Event
				FB.ActivateApp ();
			}
		}
		
		private void InitCallback ()
		{
			if (FB.IsInitialized) {
				// Signal an app activation App Event
				FB.ActivateApp ();
				Debug.Log ("FB app Activated");
				// Continue with Facebook SDK
				// ...
				//if(Application.platform != RuntimePlatform.OSXEditor)
				//LoginWithReadPermissions();
				if (FB.IsLoggedIn) {
					Debug.Log ("FB User Already Logged in!");
				} else {
					Debug.Log ("FB user needs to Login In.");
				}
			} else {
				Debug.Log ("Failed to Initialize the Facebook SDK");
			}
		}
		
		private void OnHideUnity (bool isGameShown)
		{
			if (!isGameShown) {
				// Pause the game - we will need to hide
				Time.timeScale = 0;
			} else {
				// Resume the game - we're getting focus again
				Time.timeScale = 1;
			}
		}

		public void LoginWithReadPermissions ()
		{
			var perms = new List<string> (){"public_profile", "email", "user_friends"};
			Debug.Log ("FB login called");
			FB.LogInWithReadPermissions (perms, AuthCallback);

		}

		
		private void AuthCallback (ILoginResult result)
		{
			if (FB.IsLoggedIn) {
				// AccessToken class will have session details
				var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
				// Print current access token's User ID
				Debug.Log (aToken.UserId);

				// Print current access token's granted permissions
				foreach (string perm in aToken.Permissions) {
					Debug.Log (perm);
				}
				FB.API ("/me?fields=email", HttpMethod.GET, graphResult =>
				{
					if (string.IsNullOrEmpty (graphResult.Error) == false) {
						Debug.Log ("could not get email address");
						return;
					}
					
					string email = graphResult.ResultDictionary ["email"] as string;
					Debug.Log ("FB User email: " + email);

					Networking.Instance.UpdateFacebookProfile (aToken.UserId, email, (reply) => {
						Debug.Log ("Facebook Profile Updated Successfully! "+reply + " userid = "+aToken.UserId+ " email - "+email);
						Dictionary<string, object> dict = JsonConvert.DeserializeObject<Dictionary<string, object>> (reply);
						bool status = (bool)dict["status"];
//						if(dict.ContainsKey("customField")) {
						if(!status) {
							Debug.LogError("contain customField so old user - "+ dict["customField".ToString()]);
//							CustomFields customField = JsonConvert.DeserializeObject<CustomFields>(dict["customFields"]);
						} else {
							CustomFields customField = new CustomFields();
							customField.fbId = aToken.UserId;
							DataBank.Instance.user.userProfile.customFields = customField;
							DataBank.Instance.SaveGameStateLocally();
							Debug.LogError("does not contain customField so new user attached");
						}
					});
					
				});

				                                          
//				    Networking.Instance.GetUserProfile(DataBank.Instance.user.userID,(reply) => {
//					Debug.Log("User Profile: " + reply);
//				},()=>{});
			} else {
				Debug.Log ("FB Login Failed: " + result.Error);
			}
		}

		void OnGUI ()
		{
			if (Application.platform == RuntimePlatform.OSXEditor) {
				if (GUI.Button (new Rect (0, 100, 100, 30), "Login")) {
					LoginWithReadPermissions ();
				}
			}
		}﻿
		
	}
}
