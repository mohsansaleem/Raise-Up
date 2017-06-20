using System;
using UnityEngine;
using System.Collections;
using Newtonsoft.Json;
using System.IO;
using Game.Managers;
using Game.Enums;
using Game.UI;
using TienLen.Core.GamePlay;
using TienLen.Managers;
using Facebook.Unity;
using Game;

namespace Global
{
	public class DataBank : MonoBehaviour
	{
		[HideInInspector]
		public static DataBank
			Instance;
		[HideInInspector]
		public bool
			LoggedIn = false;

		public Action OnLocalGameStateLoad;

		public Action OnGameStateUpdate;

		public MetaData metaData;
		public User user;

		void Awake ()
		{
			Instance = this;

			//LoadMetaData ();
		}

		// Use this for initialization
		void Start ()
		{
			LoadGameStateLocally ();
			LoadMetaDataLocally ();
//			metaData = new MetaData();
//			metaData.gameLevels = new System.Collections.Generic.Dictionary<int, GameLevel>();
//
//			GameLevel gameLevel = new GameLevel();
//			gameLevel.levelId = 1;
//			gameLevel.levelName = "1st Level";
//			gameLevel.scoreMultiplier = 1;
//			gameLevel.combinationsToShow = 3;
//			gameLevel.scores = new System.Collections.Generic.Dictionary<int, int>();
//			gameLevel.scores.Add (1,100);
//			gameLevel.scores.Add (2,200);
//			gameLevel.scores.Add (3,300);
//
//			metaData.gameLevels.Add(1,gameLevel);
//			metaData.gameLevels.Add(2,gameLevel);
//
//			SaveMetaData(metaData);
		}
	
		// Update is called once per frame
		void Update ()
		{
	
		}

		public void LogIn (Action onLogin = null, Action onLoginFailed = null)
		{
			Networking.Instance.LogInUser (PlayerPrefs.GetString ("userName"), PlayerPrefs.GetString ("password"), (reply) => {
				Debug.Log ("User LogIn: " + reply);
				
				//Debug.LogError ("Used: " + used);
				
				// Patch for Wrong Server Responses

				AudioManager.Instance.PlaySound (SoundType.LogIn);

				DataBank.Instance.LoadMetaData ();
				
				DataBank.Instance.LoadGameState (onLogin);

				LoggedIn = true;
				var globalChat = GameObject.FindGameObjectWithTag ("GlobalChat").GetComponentInChildren<ChatUIHandler> ();
				if(globalChat == null) {
					Debug.LogError("Global chat is null");
				} else {
					globalChat.GetGlobalChatHistory ();
				}
				if (Application.platform != RuntimePlatform.OSXEditor) {
					if (FB.IsLoggedIn) {
						FB.API ("/me?fields=email,id", HttpMethod.GET, graphResult =>
						{
							if (string.IsNullOrEmpty (graphResult.Error) == false) {
								Debug.Log ("could not get email address");
								return;
							}
							
							string email = graphResult.ResultDictionary ["email"] as string;
							Debug.Log ("FB User email: " + email);
							string usrId = graphResult.ResultDictionary ["id"] as string;
							Debug.Log ("FB User email: " + usrId);
							
							Networking.Instance.UpdateFacebookProfile (usrId, email, (response) => {
								Debug.Log ("Facebook Profile Updated Successfully!");
							});
							
						});
					}
				}

			}, () => {
				Debug.LogError ("LogIn Failed!!!");	
				if (onLoginFailed != null)
					onLoginFailed ();
				LoggedIn = false;
				
			});
		}

		void LoadGameStateLocally ()
		{
			if (PlayerPrefs.HasKey ("GameStateLoaded")) {
				string PATH = Application.persistentDataPath + "/GameData/";
				if (!Directory.Exists (PATH)) {
					Directory.CreateDirectory (PATH);
				}
				var streamReader = new StreamReader (File.Open (PATH + "/GameState.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None));
				string sdata = streamReader.ReadToEnd ();
				streamReader.Close ();
				if (user == null) {
					user = JsonConvert.DeserializeObject<User> (sdata);
					if (OnLocalGameStateLoad != null)
						OnLocalGameStateLoad ();
				}
			}
		}

		void LoadMetaDataLocally ()
		{
			if (PlayerPrefs.HasKey ("MetaDataLoaded")) {
				string PATH = Application.persistentDataPath + "/GameData/";
				if (!Directory.Exists (PATH)) {
					Directory.CreateDirectory (PATH);
				}
				var streamReader = new StreamReader (File.Open (PATH + "/MetaData.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None));
				string sdata = streamReader.ReadToEnd ();
				streamReader.Close ();
				if (metaData == null)
					metaData = JsonConvert.DeserializeObject<MetaData> (sdata);
			}
		}


		void SaveMetaData (MetaData metadata)
		{
			string filePath = System.IO.Path.Combine (Application.streamingAssetsPath, Constants.METADATA_FILE_PATH);

			string PATH = Application.persistentDataPath + "/GameData/";
			if (!Directory.Exists (PATH)) {
				Directory.CreateDirectory (PATH);
			}
			string data = JsonConvert.SerializeObject (metadata);
			var streamWriter = new StreamWriter (PATH + "/MetaData.txt");
			streamWriter.Write (data);
			streamWriter.Close ();
			PlayerPrefs.SetInt ("MetaDataLoaded", 1);
			PlayerPrefs.Save ();

			Debug.LogError ("MetaData: " + JsonConvert.SerializeObject (metadata));
		}

		public void SaveGameStateLocally ()
		{
			SaveGameState (user);
		}

		void SaveGameState (User gameState)
		{
			if (OnGameStateUpdate != null)
				OnGameStateUpdate ();

			string filePath = System.IO.Path.Combine (Application.streamingAssetsPath, Constants.GAMESTATE_FILE_PATH);
			
			string PATH = Application.persistentDataPath + "/GameData/";
			if (!Directory.Exists (PATH)) {
				Directory.CreateDirectory (PATH);
			}
			string data = JsonConvert.SerializeObject (gameState);
			var streamWriter = new StreamWriter (PATH + "/GameState.txt");
			streamWriter.Write (data);
			streamWriter.Close ();
			PlayerPrefs.SetInt ("GameStateLoaded", 1);
			PlayerPrefs.Save ();
			
			Debug.LogError ("MetaData: " + JsonConvert.SerializeObject (gameState));
		}

		public void UpdateGameStateOnServer (User user)
		{
			Networking.Instance.UpdateGameState (user, (str) => {
				if (OnGameStateUpdate != null)
					OnGameStateUpdate ();
				Debug.LogError ("GameState Updated");
			}, () => {
				Debug.LogError ("GameState Updated Error.");
			});
		}

		public void UpdateGameStateLocally (User user)
		{
			this.user = user;
			SaveGameStateLocally ();
		}

		public void AddChipsLocally (long chips)
		{
			if (user == null) {
				Debug.LogError ("User is Null!!!");
				return;
			}
			user.gameState.wallet.chips += chips;

			user.gameState.version++;

			SaveGameStateLocally ();
		}

		public void AddChipsOnServer (long chips)
		{
			if (user == null) {
				Debug.LogError ("User is Null!!!");
				return;
			}
			user.gameState.wallet.chips += chips;
			
			user.gameState.version++;
			
			UpdateGameStateOnServer (user);
		}

		public void AddChipsOnServerAndClient (long chips)
		{
			if (user == null) {
				Debug.LogError ("User is Null!!!");
				return;
			}
			user.gameState.wallet.chips += chips;
			
			user.gameState.version++;
			
			UpdateGameStateOnServer (user);
			SaveGameStateLocally ();
		}

		public void LoadGameState (Action OnLoad = null)
		{
			Networking.Instance.GetGameState ((gameState) => {
				var game = JsonConvert.DeserializeObject<GameStateResponse> (gameState);

				if (PlayerPrefs.HasKey ("GameStateLoaded")) {
					if (user != null) {
						Debug.LogError ("Version: " + user.gameState.version + ", Server: " + game.user.gameState.version);
						if (user.gameState.version > game.user.gameState.version) {
							DataBank.Instance.UpdateGameStateOnServer (user);
						} else
							user = game.user;
					}
				} else
					user = game.user;

				
				Debug.LogError ("GetGameState: " + gameState);

				if (OnLoad != null)
					OnLoad ();

				SaveGameState (user);

				// Checking the Online Status and Taking to the table.
				Networking.Instance.IsOnActiveTable ((str) => {
					var response = JsonConvert.DeserializeObject<IsOnActiveTableResponse> (str);
					if (response.Status) {
						Lobby.OnLobbyLoad = (() =>
						{
							var lobby = UIManager.Instance.GetScreen (GameUI.Lobby);

							if (lobby == null)
								UIManager.Instance.ShowUI (GameUI.Lobby, PreviousScreenVisibility.HidePreviousExcludingHud);

							if ((lobby as Lobby).SelectedRom == null || !((lobby as Lobby).SelectedRom.Room.id.Equals (response.roomId))) {
								(lobby as Lobby).SelectRoom (response.roomId);
							}

							GameManager.Instance.StartOnline ((lobby as Lobby).SelectedRom.Room.buyIn);

							(lobby as Lobby).RefreshTable ();
							var table = (UIManager.Instance.GetScreen (GameUI.TienLenClient) as TienLenClient);
							table.GetGameState ();
						});
						
						var screen = UIManager.Instance.GetScreen (GameUI.Lobby);
						if (screen == null) {
							UIManager.Instance.ShowUI (GameUI.Lobby);
						}
					}
					//					else {
					//						var scr = UIManager.Instance.GetTop ();
					//						if (scr != null && scr  is TienLenClient)
					//							UIManager.Instance.ShowUI (GameUI.GameMenu);
					//						
					//					}
				}, () => {
					
				});
			}, () => {});
		}

		public void LoadMetaData ()
		{
			Networking.Instance.GetMetaData ((metaDataReply) => {
				MetadataResponse response = JsonConvert.DeserializeObject<MetadataResponse> (metaDataReply);

				metaData = response.metaData;

				SaveMetaData (metaData);
			
			}, () => {
				Debug.LogError ("Unable to Load MetaData.");});

			//string filePath = System.IO.Path.Combine(Application.streamingAssetsPath, Constants.METADATA_FILE_PATH);

//			ResourceHelper.LoadFromStreamingAssetsAsync (Constants.METADATA_FILE_PATH, (result) => {
//				//string result = System.IO.File.ReadAllText(filePath);
//
//				string data = result.text;
//
//				Debug.Log ("MetaData Loaded:" + data);
//
//				metaData = JsonConvert.DeserializeObject<MetaData> (data);
//
//				MetaDataLoaded = true;
//			});

		}


	}
}
