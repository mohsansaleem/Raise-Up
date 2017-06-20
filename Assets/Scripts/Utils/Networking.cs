using UnityEngine;
using System.Collections;
using TienLen.Utils.Enums;
using WebSocket_Client;
using System.Collections.Generic;
using System;
using Newtonsoft.Json;
using LogMessages;
using WebSocket_Client;
using Zems.Common.Domain.Messages;
using Game.Managers;
using Game.Enums;
using TienLen.Core.GamePlay;
using TienLen.Managers;
using Game.UI;
using Global;

public class Networking : MonoBehaviour
{
	[SerializeField]
	private bool
		Enabled;

	public static Networking Instance;

	private static DateTime BackgroundTime = DateTime.MinValue;

	public Queue<Action> ResponseProcessRequests;
    
	private const string SERVER_ASSEMBLY_REFERENCE = "TienLen.Engine";
	private const string CLIENT_ASSEMBLY_REFERENCE = "Assembly-CSharp";

	public Action OnConnected;

	[HideInInspector]
	public bool
		Connected = false;

	#region CALLS CONSTANTS
	private const string SERVER_PING = "1003";

	private const string REGISTER_USER = "1050";
	private const string LOGIN_USER = "1001";
	private const string GET_META_DATA = "1250";
	private const string VIEW_LOBBY = "40100";
	private const string JOIN_ROOM = "30100";
	private const string LEAVE_ROOM = "30500";
	private const string GET_ROOM_USERS = "30400";
	private const string OCCUPY_SEAT_ON_TABLE = "50300";
	private const string GET_GAME_STATE = "1052";
	private const string SAVE_GAME_STATE = "1067";
	private const string SEND_MESSAGE_TO_GAME_ENGINE = "60100";
	private const string START_GAME_CLIENT_REQUEST = "50100";
	private const string IS_ON_ACTIVE_TABLE = "1065";
	private const string GLOBAL_CHAT_REQUEST = "70000";
	private const string ROOM_CHAT_REQUEST = "70100";
	private const string GLOBAL_CHAT_HISTORY_REQUEST = "70300";
	private const string ROOM_CHAT_HISTORY_REQUEST = "70200";
	private const string UPDATE_FB_PROFILE = "1068";
	private const string GET_USER_PROFILE = "1066";
	private const string SEND_FRIEND_REQUEST = "1060";
	private const string ACCEPT_FRIEND_REQUEST = "1061";
	private const string VIEW_FRIEND_REQUESTS = "1062";
	private const string REMOVE_FRIEND = "1063";
	private const string SEND_CHIPS = "2000";
	private const string VIEW_FRIENDS_LIST = "1064";


	// Push Messages 
	private const string UPDATE_USER_SERVER_PUSH = "100600";
	private const string UPDATE_ROOM_SERVER_PUSH = "100300";
	private const string START_GAME_SERVER_PUSH = "100500";
	private const string CAN_START_GAME_SERVER_PUSH = "100900";
	private const string GAME_ENGINE_RESPONSE_SERVER_BROADCAST = "100400";
	private const string GAME_ENGINE_RESPONSE_SERVER_PUSH = "200200";
	private const string GLOBAL_CHAT_PUSH = "100001";
	private const string ROOM_CHAT_PUSH = "100002";



    #endregion




	IEnumerator ProcessRequests ()
	{
		while (true) {   
			//            Debug.Log("Networking.ProcessRequests");
			yield return new WaitForEndOfFrame ();
			if (ResponseProcessRequests.Count > 0) {
				ResponseProcessRequests.Dequeue ().Invoke ();
			}
		}
	}

	public void Awake ()
	{
		Instance = this;
	}

	// Use this for initialization
	void Start ()
	{
		Connect ();
	}

	public void Send (string Json)
	{
//        Client.Instance.Send(Json);
	}

	public void Connect ()
	{
		WebSocket_Client.Client.OnConnected -= OnConnection;
		WebSocket_Client.Client.OnConnected += OnConnection;
		WebSocket_Client.Client.OnDisconnect -= OnDisconnet;
		WebSocket_Client.Client.OnDisconnect += OnDisconnet;
		
		ResponseProcessRequests = new Queue<Action> ();
		StartCoroutine ("ProcessRequests");

		WebSocket_Client.Client.Instance.Connect ();
	}

	void LogIn ()
	{
		Debug.LogError ("Socket Connect!");
		DataBank.Instance.LogIn (() => {
			var screen = UIManager.Instance.GetTop ();
			if (screen is Lobby)
				UIManager.Instance.ShowUI (GameUI.Lobby, PreviousScreenVisibility.DoNothing);
		});
	}

	void OnApplicationPause (bool pause)
	{
		if (!pause) {
			var time = (DateTime.Now - BackgroundTime);
			//Debug.LogError ("Time Interval: " + time.TotalSeconds);
			if (!BackgroundTime.Equals (DateTime.MinValue) && (DateTime.Now - BackgroundTime).TotalMinutes >= 1) {
				WebSocket_Client.Client.Instance.Disconnect ();
				Connected = false;
			}

			var screen = UIManager.Instance.GetTop ();

			while ((screen != null) && ((screen is Friends) || (screen is Settings) || (screen is Profile))) {
				UIManager.Instance.DequeueUI (PreviousScreenVisibility.DequeuePreviousExcludingHud);
				screen = UIManager.Instance.GetTop ();
			}

			if (screen == null) {
				Debug.LogError ("Screen is null!!!");
				return;
			}

			Debug.LogError ("Connected Value: " + Connected);

			if (Connected && screen is TienLenClient && (screen as TienLenClient).gameType != GameType.Offline) {
				var lobby = UIManager.Instance.GetScreen (GameUI.Lobby);
				if (lobby != null && (lobby as Lobby).SelectedRom != null && (lobby as Lobby).SelectedRom.Room.status == RoomStatus.Active)
					(screen as TienLenClient).GetGameState ();
				else
					(lobby as Lobby).RefreshTable ();
			}

			if (!Connected) {
				OnConnected = LogIn;
				Debug.LogError ("Trying To Connect Again.");
				Connect ();
			} else {
				if (screen is Lobby)
					UIManager.Instance.ShowUI (GameUI.Lobby, PreviousScreenVisibility.DoNothing);
			}
		} else {
			BackgroundTime = DateTime.Now;
			//Debug.LogError ("Background Time: " + BackgroundTime.ToString ());

		}
	}

	void OnGameEnginePush (InBoundMessage inBoundMessage)
	{
		var response = JsonConvert.DeserializeObject<Dictionary<string,object>> (inBoundMessage.DataAsString);
		string msgString = ((string)response ["message"]).Replace (SERVER_ASSEMBLY_REFERENCE, CLIENT_ASSEMBLY_REFERENCE);
		//Debug.Log ("Kuch: " + msgString);
		IActionMessage msg = (JsonConvert.DeserializeObject<IActionMessage> (msgString, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All }));

		//Debug.LogError ("Before getting screen");
		var widget = UIManager.Instance.GetScreen (Game.Enums.GameUI.TienLenClient);
//		var widget = UIManager.Instance.GetTop();

		if (widget is TienLenClient) {
			((TienLenClient)widget).OnTableStateChanged (msg);
//			Debug.LogError ("Added To Queue");
		}
		widget = UIManager.Instance.GetTop();
		if( widget is Lobby && msg is GameFinished )
		{
			// TODO: Do the thing . . .
			IWidget currentUI = UIManager.Instance.GetTop();
			if(currentUI is Lobby) {
				Room room = ((Lobby)Lobby.Instance).SelectedRom.Room;
				Networking.Instance.LeaveRoom (DataBank.Instance.user.userID.ToString (), room.id, (reply) => {
					Debug.LogError("calling leave Room at the end of match - becuase am in lobby");
					room = null;
					((Lobby)widget).GameFinished();
				}, () => {});
			}
		}
//		else
//			Debug.LogError ("Error: TienLenClient Widget is null...");
	}

	DateTime startTime;
	DateTime finalTime;

	IEnumerator CheckDisconnection() {
		while (true) {
			finalTime = DateTime.Now;
			if((finalTime - startTime).TotalSeconds > 60) {
				WebSocket_Client.Client.Instance.Disconnect();
				Debug.LogError("Calling DIsconnect due to inactivity");
				yield break;
			}
			yield return new WaitForSeconds(1.0f);
		}
	}

	public void OnConnection ()
	{
		Connected = true;
		if (OnConnected != null)
			ResponseProcessRequests.Enqueue (OnConnected);

		WebSocket_Client.Client.Instance.On (SERVER_PING, (inBoundMessage) => {
			//Debug.Log ("SERVER_PING_PUSH: " + inBoundMessage.DataAsString);

			var args = new Dictionary<string, object> ();
			args.Add ("correlationId", Guid.NewGuid ().ToString ());
			

			args.Add ("action", SERVER_PING);

			startTime = DateTime.Now;
			StopCoroutine(CheckDisconnection());
			StartCoroutine(CheckDisconnection());
			
			string json = JsonConvert.SerializeObject (args);
			
			GeneralQuery ("PingBack", SERVER_PING, json, (str) => {
				//Debug.Log ("Ping Back: " + str);
			}, () => {
				Debug.LogError ("Ping Back Failure.");});
		});

		WebSocket_Client.Client.Instance.On (UPDATE_USER_SERVER_PUSH, (inBoundMessage) => {
			Debug.LogError ("UPDATE_USER_SERVER_PUSH: " + inBoundMessage.DataAsString);
			
			//{"buyIn":10000,"game":null,"roomQuantity":2,"roomSize":4,"id":"tienLien_10000_4","matches":null,"users":[{"_id":null,"gameState":{"wallet":{},"games":[]},"userID":20,"username":"invisible_chip"}],"roomName":"Intermediate Room"}}}
			
			//var response = JsonConvert.DeserializeObject<Dictionary<string,object>> (inBoundMessage.DataAsString);
			UpdateGameStatePush updateGameStatePush = JsonConvert.DeserializeObject<UpdateGameStatePush> (inBoundMessage.DataAsString);

			if (updateGameStatePush.success) {
				DataBank.Instance.UpdateGameStateLocally (updateGameStatePush.user);
			}
			if(updateGameStatePush.messageType == UpdateGameStatePush.MessageType.FriendRequestAccepted) {
				WebSocket_Client.Client.Instance.NotificationReceivedFromServer(updateGameStatePush.message);
			}
			if(updateGameStatePush.messageType == UpdateGameStatePush.MessageType.FriendRequestReceived) {
				WebSocket_Client.Client.Instance.NotificationReceivedFromServer(updateGameStatePush.message);
			}
			if(updateGameStatePush.messageType == UpdateGameStatePush.MessageType.ChipsReceived) {
				WebSocket_Client.Client.Instance.NotificationReceivedFromServer(updateGameStatePush.message);
			}
		});

		WebSocket_Client.Client.Instance.On (UPDATE_ROOM_SERVER_PUSH, (inBoundMessage) => {
			Debug.LogError ("UPDATE_ROOM_SERVER_PUSH: " + inBoundMessage.DataAsString);
		
			//{"buyIn":10000,"game":null,"roomQuantity":2,"roomSize":4,"id":"tienLien_10000_4","matches":null,"users":[{"_id":null,"gameState":{"wallet":{},"games":[]},"userID":20,"username":"invisible_chip"}],"roomName":"Intermediate Room"}}}
		
			//var response = JsonConvert.DeserializeObject<Dictionary<string,object>> (inBoundMessage.DataAsString);
			UpdateRoomResponse updateRoomResponse = JsonConvert.DeserializeObject<UpdateRoomResponse> (inBoundMessage.DataAsString);

			Debug.LogError ("Room: " + JsonConvert.SerializeObject (updateRoomResponse.room));

			var lobby = UIManager.Instance.GetScreen (GameUI.Lobby);
			if (lobby != null) {
				((Lobby)lobby).UpdateRoom (updateRoomResponse.room);
			}
		});

		WebSocket_Client.Client.Instance.On (START_GAME_SERVER_PUSH, (inBoundMessage) => {
			Debug.LogError ("START_GAME_SERVER_PUSH: " + inBoundMessage.DataAsString);

			PopupManager.Instance.HidePopup ();

			int buyIns = 0;
			var lobby = UIManager.Instance.GetScreen (GameUI.Lobby);
			if (lobby != null) {
				buyIns = ((Lobby)lobby).SelectedRom.Room.buyIn;
			}

			GameManager.Instance.StartOnline (buyIns);
		});

		WebSocket_Client.Client.Instance.On (GAME_ENGINE_RESPONSE_SERVER_PUSH, (inBoundMessage) => {
			Debug.LogError ("GAME_ENGINE_RESPONSE_SERVER_PUSH: " + inBoundMessage.DataAsString);
			
			OnGameEnginePush (inBoundMessage);
		});

		WebSocket_Client.Client.Instance.On (GAME_ENGINE_RESPONSE_SERVER_BROADCAST, (inBoundMessage) => {
			Debug.LogError ("GAME_ENGINE_RESPONSE_SERVER_PUSH: " + inBoundMessage.DataAsString);
		
			OnGameEnginePush (inBoundMessage);
		});


		WebSocket_Client.Client.Instance.On (CAN_START_GAME_SERVER_PUSH, (inBoundMessage) => {
			Debug.LogError ("CAN_START_GAME_SERVER_PUSH: " + inBoundMessage.DataAsString);

			var widget = UIManager.Instance.GetScreen (GameUI.TienLenClient);

			if (widget != null) {
				(widget as TienLenClient).ShowStartButton ();
			}
		});


		WebSocket_Client.Client.Instance.On (GLOBAL_CHAT_PUSH, (inBoundMessage) => {
			Debug.LogError ("GLOBAL_CHAT_PUSH Message: " + inBoundMessage.DataAsString);
			ChatMessage cm = JsonConvert.DeserializeObject<ChatMessage> (inBoundMessage.DataAsString);
			Debug.LogError ("GLOBAL_CHAT_PUSH: " + cm.message);
			WebSocket_Client.Client.Instance.GlobalChatMessageRecieved (cm);
			
			//OnGameEnginePush (inBoundMessage);
		});
		
		WebSocket_Client.Client.Instance.On (ROOM_CHAT_PUSH, (inBoundMessage) => {
			Debug.LogError ("ROOM_CHAT_PUSH Message: " + inBoundMessage.DataAsString);
			ChatMessage cm = JsonConvert.DeserializeObject<ChatMessage> (inBoundMessage.DataAsString);
			Debug.LogError ("ROOM_CHAT_PUSH: " + cm.message);
			WebSocket_Client.Client.Instance.RoomChatMessageRecieved (cm);
			
			//OnGameEnginePush (inBoundMessage);
		});

		
		//		var args = new Dictionary<string,object> ();
//		args.Add ("correlationId", Guid.NewGuid ().ToString ());
//		args.Add ("userName", "KoiOrNaam");
//		args.Add ("password", "123234");
//		args.Add ("deviceIdentifier", 1);
//		args.Add ("deviceCode", 1);
//		args.Add ("action", 1050);
//
//		string json = JsonConvert.SerializeObject (args);
//        
//		new WebSocket_Client.Request ("1050", json)
//            .OnReply ((reply) => {
//
//			this.Log ("YO");
//
//		}).Send (); 
	}

	void OnDisconnet ()
	{
		ResponseProcessRequests.Clear ();
		Connected = false;
		Debug.LogError ("Disconnected");

//		if(Application.isPlaying) {
//			Connect();
//			Debug.LogError ("connecting again");
//		}
	}

	public void RegisterNewUser (string _username, string _password, Action<string> OnSuccess = null, Action OnFailure = null)
	{
		var args = new Dictionary<string, object> ();
		args.Add ("correlationId", Guid.NewGuid ().ToString ());
		args.Add ("userName", _username);
		args.Add ("password", _password);
		args.Add ("deviceIdentifier", 1);
		args.Add ("deviceCode", 1);
		args.Add ("action", REGISTER_USER);

		string json = JsonConvert.SerializeObject (args);

		GeneralQuery ("RegisterNewUser", REGISTER_USER, json, OnSuccess, OnFailure);
	}

	public void LogInUser (string _username, string _password, Action<string> OnSuccess = null, Action OnFailure = null)
	{
		var args = new Dictionary<string, object> ();
		args.Add ("correlationId", Guid.NewGuid ().ToString ());
		args.Add ("userName", _username);
		args.Add ("password", _password);

		args.Add ("action", LOGIN_USER);
		
		string json = JsonConvert.SerializeObject (args);
		
		GeneralQuery ("LogInUser", LOGIN_USER, json, OnSuccess, OnFailure);
	}

	public void GetMetaData (Action<string> OnSuccess = null, Action OnFailure = null)
	{
		var args = new Dictionary<string, object> ();
		args.Add ("correlationId", Guid.NewGuid ().ToString ());
		
		args.Add ("action", GET_META_DATA);
		
		string json = JsonConvert.SerializeObject (args);
		
		GeneralQuery ("GetMetaData", GET_META_DATA, json, OnSuccess, OnFailure);
	}

	public void GetGameState (Action<string> OnSuccess = null, Action OnFailure = null)
	{
		var args = new Dictionary<string, object> ();
		args.Add ("correlationId", Guid.NewGuid ().ToString ());
		
		args.Add ("action", GET_GAME_STATE);
		
		string json = JsonConvert.SerializeObject (args);
		
		GeneralQuery ("GetGameState", GET_GAME_STATE, json, OnSuccess, OnFailure);
	}

	public void UpdateGameState (User user, Action<string> OnSuccess = null, Action OnFailure = null)
	{
		var args = new Dictionary<string, object> ();
		args.Add ("correlationId", Guid.NewGuid ().ToString ());
		args.Add ("user", user);
		
		args.Add ("action", SAVE_GAME_STATE);
		
		string json = JsonConvert.SerializeObject (args);
		
		GeneralQuery ("UpdateGameState", SAVE_GAME_STATE, json, OnSuccess, OnFailure);
	}

	public void StartGameClientRequest (string _roomId, Action<string> OnSuccess = null, Action OnFailure = null)
	{
		var args = new Dictionary<string, object> ();
		args.Add ("correlationId", Guid.NewGuid ().ToString ());
		
		args.Add ("action", START_GAME_CLIENT_REQUEST);

		args.Add ("roomId", _roomId);
		
		string json = JsonConvert.SerializeObject (args);
		
		GeneralQuery ("StartGameClientRequest", START_GAME_CLIENT_REQUEST, json, OnSuccess, OnFailure);
	}


	public void ViewLobby (Action<string> OnSuccess = null, Action OnFailure = null)
	{
		var args = new Dictionary<string, object> ();
		args.Add ("correlationId", Guid.NewGuid ().ToString ());
		
		args.Add ("action", VIEW_LOBBY);
		
		string json = JsonConvert.SerializeObject (args);
		
		GeneralQuery ("ViewLobby", VIEW_LOBBY, json, OnSuccess, OnFailure);
	}

	public void JoinRoom (string _userId, string _roomId, UserType userType, Action<string> OnSuccess = null, Action OnFailure = null)
	{
		var args = new Dictionary<string, object> ();
		args.Add ("correlationId", Guid.NewGuid ().ToString ());
		args.Add ("userId", _userId);
		args.Add ("roomId", _roomId);
		args.Add ("userType", userType);

		args.Add ("action", JOIN_ROOM);
		
		string json = JsonConvert.SerializeObject (args);
		
		GeneralQuery ("JoinRoom", JOIN_ROOM, json, OnSuccess, OnFailure);
	}

	public void LeaveRoom (string _userId, string _roomId, Action<string> OnSuccess = null, Action OnFailure = null)
	{
		var args = new Dictionary<string, object> ();
		args.Add ("correlationId", Guid.NewGuid ().ToString ());
		args.Add ("userId", _userId);
		args.Add ("roomId", _roomId);
		
		args.Add ("action", LEAVE_ROOM);
		
		string json = JsonConvert.SerializeObject (args);
		
		GeneralQuery ("LeaveRoom", LEAVE_ROOM, json, OnSuccess, OnFailure);
	}

	public void GetRoomUsers (string _roomId, Action<string> OnSuccess = null, Action OnFailure = null)
	{
		var args = new Dictionary<string, object> ();
		args.Add ("correlationId", Guid.NewGuid ().ToString ());

		args.Add ("roomId", _roomId);
		
		args.Add ("action", GET_ROOM_USERS);
		
		string json = JsonConvert.SerializeObject (args);
		
		GeneralQuery ("GetRoomUsers", GET_ROOM_USERS, json, OnSuccess, OnFailure);
	}

	public void OccupySeatOnTable (string _seatNumber, string _roomId, Action<string> OnSuccess = null, Action OnFailure = null)
	{
		var args = new Dictionary<string, object> ();
		args.Add ("correlationId", Guid.NewGuid ().ToString ());

		args.Add ("seat", _seatNumber);

		args.Add ("roomId", _roomId);
		
		args.Add ("action", OCCUPY_SEAT_ON_TABLE);
		
		string json = JsonConvert.SerializeObject (args);
		
		GeneralQuery ("GetRoomUsers", OCCUPY_SEAT_ON_TABLE, json, OnSuccess, OnFailure);
	}

	public void SentMessageToGameEngine (string playerId, IActionMessage message, Action<string> OnSuccess = null, Action OnFailure = null)
	{
		var args = new Dictionary<string, object> ();
		args.Add ("correlationId", Guid.NewGuid ().ToString ());
		
		args.Add ("playerId", playerId);
		args.Add ("message", JsonConvert.SerializeObject (message, Formatting.None, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All }).Replace (CLIENT_ASSEMBLY_REFERENCE, SERVER_ASSEMBLY_REFERENCE));

		args.Add ("action", SEND_MESSAGE_TO_GAME_ENGINE);
		
		string json = JsonConvert.SerializeObject (args);

		Debug.LogError ("SentMessageToGameEngine: " + json);



		GeneralQuery ("SentMessageToGameEngine", SEND_MESSAGE_TO_GAME_ENGINE, json, OnSuccess, OnFailure);
	}

	public void IsOnActiveTable (Action<string> OnSuccess = null, Action OnFailure = null)
	{
		var args = new Dictionary<string, object> ();
		args.Add ("correlationId", Guid.NewGuid ().ToString ());

		args.Add ("action", IS_ON_ACTIVE_TABLE);

		string json = JsonConvert.SerializeObject (args);
		
		Debug.LogError ("IsOnActiveTable: " + json);
		
		GeneralQuery ("IsOnActiveTable", IS_ON_ACTIVE_TABLE, json, OnSuccess, OnFailure);
	}

	public void GlobalChatRequest (string message, Action<string> OnSuccess = null, Action OnFailure = null)
	{
		var args = new Dictionary<string, object> ();
		args.Add ("correlationId", Guid.NewGuid ().ToString ());
		
		//args.Add ("playerId", playerId);
		//args.Add ("message", JsonConvert.SerializeObject (message, Formatting.None, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All }).Replace (CLIENT_ASSEMBLY_REFERENCE, SERVER_ASSEMBLY_REFERENCE));
		args.Add ("message", message);
		
		args.Add ("action", GLOBAL_CHAT_REQUEST);
		
		string json = JsonConvert.SerializeObject (args);
		
		Debug.LogError ("GlobalChatRequest: " + json);
		
		GeneralQuery ("GlobalChatRequest", GLOBAL_CHAT_REQUEST, json, OnSuccess, OnFailure);
	}

	public void GlobalChatHistoryRequest (Action<string> OnSuccess = null, Action OnFailure = null)
	{
		var args = new Dictionary<string, object> ();
		args.Add ("correlationId", Guid.NewGuid ().ToString ());
		
		//args.Add ("playerId", playerId);
		//args.Add ("message", JsonConvert.SerializeObject (message, Formatting.None, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All }).Replace (CLIENT_ASSEMBLY_REFERENCE, SERVER_ASSEMBLY_REFERENCE));
		//args.Add ("message", message);
		
		args.Add ("action", GLOBAL_CHAT_HISTORY_REQUEST);
		
		string json = JsonConvert.SerializeObject (args);
		
		Debug.LogError ("GlobalChatHistoryRequest: " + json);
		
		GeneralQuery ("GlobalChatHistoryRequest", GLOBAL_CHAT_HISTORY_REQUEST, json, OnSuccess, OnFailure);
	}
	
	
	public void RoomChatRequest (string roomId, string message, Action<string> OnSuccess = null, Action OnFailure = null)
	{
		var args = new Dictionary<string, object> ();
		args.Add ("correlationId", Guid.NewGuid ().ToString ());
		
		//args.Add ("playerId", playerId);
		//args.Add ("message", JsonConvert.SerializeObject (message, Formatting.None, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All }).Replace (CLIENT_ASSEMBLY_REFERENCE, SERVER_ASSEMBLY_REFERENCE));
		args.Add ("roomId", roomId);
		args.Add ("message", message);
		args.Add ("messageType", 0);

		args.Add ("action", ROOM_CHAT_REQUEST);
		
		string json = JsonConvert.SerializeObject (args);
		
		Debug.LogError ("RoomChatRequest: " + json);
		
		GeneralQuery ("RoomChatRequest", ROOM_CHAT_REQUEST, json, OnSuccess, OnFailure);
	}

	public void RoomChatHistoryRequest (string roomId, Action<string> OnSuccess = null, Action OnFailure = null)
	{
		var args = new Dictionary<string, object> ();
		args.Add ("correlationId", Guid.NewGuid ().ToString ());
		
		//args.Add ("playerId", playerId);
		//args.Add ("message", JsonConvert.SerializeObject (message, Formatting.None, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All }).Replace (CLIENT_ASSEMBLY_REFERENCE, SERVER_ASSEMBLY_REFERENCE));
		args.Add ("roomId", roomId);
		//args.Add ("message", message);
		
		args.Add ("action", ROOM_CHAT_HISTORY_REQUEST);
		
		string json = JsonConvert.SerializeObject (args);
		
		Debug.LogError ("RoomChatHistoryRequest: " + json);
		
		GeneralQuery ("RoomChatHistoryRequest", ROOM_CHAT_HISTORY_REQUEST, json, OnSuccess, OnFailure);
	}

	public void UpdateFacebookProfile (string facebookId, string email, Action<string> OnSuccess = null, Action OnFailure = null)
	{
		var args = new Dictionary<string, object> ();
		args.Add ("correlationId", Guid.NewGuid ().ToString ());
		
		//args.Add ("playerId", playerId);
		//args.Add ("message", JsonConvert.SerializeObject (message, Formatting.None, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All }).Replace (CLIENT_ASSEMBLY_REFERENCE, SERVER_ASSEMBLY_REFERENCE));
		args.Add ("fbId", facebookId);
		args.Add ("email", email);
		//args.Add ("message", message);
		
		args.Add ("action", UPDATE_FB_PROFILE);
		
		string json = JsonConvert.SerializeObject (args);
		
		Debug.LogError ("UpdateFacebookProfile: " + json);
		
		GeneralQuery ("UpdateFacebookProfile", UPDATE_FB_PROFILE, json, OnSuccess, OnFailure);
	}

	public void GetUserProfile (long userId, Action<string> OnSuccess = null, Action OnFailure = null)
	{
		var args = new Dictionary<string, object> ();
		args.Add ("correlationId", Guid.NewGuid ().ToString ());
		
		//args.Add ("playerId", playerId);
		//args.Add ("message", JsonConvert.SerializeObject (message, Formatting.None, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All }).Replace (CLIENT_ASSEMBLY_REFERENCE, SERVER_ASSEMBLY_REFERENCE));
		args.Add ("userId", userId);
		//args.Add ("message", message);
		
		args.Add ("action", GET_USER_PROFILE);
		
		string json = JsonConvert.SerializeObject (args);
		
		Debug.LogError ("GetUserProfile: " + json);
		
		GeneralQuery ("GetUserProfile", GET_USER_PROFILE, json, OnSuccess, OnFailure);
	}

	public void SendFriendRequest (long userId, Action<string> OnSuccess = null, Action OnFailure = null)
	{
		var args = new Dictionary<string, object> ();
		args.Add ("correlationId", Guid.NewGuid ().ToString ());
		
		//args.Add ("playerId", playerId);
		//args.Add ("message", JsonConvert.SerializeObject (message, Formatting.None, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All }).Replace (CLIENT_ASSEMBLY_REFERENCE, SERVER_ASSEMBLY_REFERENCE));
		args.Add ("userId", userId);
		//args.Add ("message", message);
		
		args.Add ("action", SEND_FRIEND_REQUEST);
		
		string json = JsonConvert.SerializeObject (args);
		
		Debug.LogError ("SendFriendRequest: " + json);
		
		GeneralQuery ("SendFriendRequest", SEND_FRIEND_REQUEST, json, OnSuccess, OnFailure);
	}

	public void AcceptFriendRequest (long userId, Action<string> OnSuccess = null, Action OnFailure = null)
	{
		var args = new Dictionary<string, object> ();
		args.Add ("correlationId", Guid.NewGuid ().ToString ());
		
		//args.Add ("playerId", playerId);
		//args.Add ("message", JsonConvert.SerializeObject (message, Formatting.None, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All }).Replace (CLIENT_ASSEMBLY_REFERENCE, SERVER_ASSEMBLY_REFERENCE));
		args.Add ("userId", userId);
		//args.Add ("message", message);
		
		args.Add ("action", ACCEPT_FRIEND_REQUEST);
		
		string json = JsonConvert.SerializeObject (args);
		
		Debug.LogError ("AcceptFriendRequest: " + json);
		
		GeneralQuery ("AcceptFriendRequest", ACCEPT_FRIEND_REQUEST, json, OnSuccess, OnFailure);
	}


	public void ViewFriendRequests (Action<string> OnSuccess = null, Action OnFailure = null)
	{
		var args = new Dictionary<string, object> ();
		args.Add ("correlationId", Guid.NewGuid ().ToString ());
		
		//args.Add ("playerId", playerId);
		//args.Add ("message", JsonConvert.SerializeObject (message, Formatting.None, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All }).Replace (CLIENT_ASSEMBLY_REFERENCE, SERVER_ASSEMBLY_REFERENCE));
		//args.Add ("userId", userId);
		//args.Add ("message", message);
		
		args.Add ("action", VIEW_FRIEND_REQUESTS);
		
		string json = JsonConvert.SerializeObject (args);
		
		Debug.LogError ("ViewFriendRequests: " + json);
		
		GeneralQuery ("ViewFriendRequests", VIEW_FRIEND_REQUESTS, json, OnSuccess, OnFailure);
	}

	public void RemoveFriend (long userId, Action<string> OnSuccess = null, Action OnFailure = null)
	{
		var args = new Dictionary<string, object> ();
		args.Add ("correlationId", Guid.NewGuid ().ToString ());
		
		//args.Add ("playerId", playerId);
		//args.Add ("message", JsonConvert.SerializeObject (message, Formatting.None, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All }).Replace (CLIENT_ASSEMBLY_REFERENCE, SERVER_ASSEMBLY_REFERENCE));
		args.Add ("userId", userId);
		//args.Add ("message", message);
		
		args.Add ("action", REMOVE_FRIEND);
		
		string json = JsonConvert.SerializeObject (args);
		
		Debug.LogError ("RemoveFriend: " + json);
		
		GeneralQuery ("RemoveFriend", REMOVE_FRIEND, json, OnSuccess, OnFailure);
	}

	public void ViewFriendsList (Action<string> OnSuccess = null, Action OnFailure = null)
	{
		var args = new Dictionary<string, object> ();
		args.Add ("correlationId", Guid.NewGuid ().ToString ());
		
		//args.Add ("playerId", playerId);
		//args.Add ("message", JsonConvert.SerializeObject (message, Formatting.None, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All }).Replace (CLIENT_ASSEMBLY_REFERENCE, SERVER_ASSEMBLY_REFERENCE));
		//args.Add ("userId", userId);
		//args.Add ("message", message);
		
		args.Add ("action", VIEW_FRIENDS_LIST);
		
		string json = JsonConvert.SerializeObject (args);
		
		Debug.LogError ("ViewFriendsList: " + json);
		
		GeneralQuery ("ViewFriendsList", VIEW_FRIENDS_LIST, json, OnSuccess, OnFailure);
	}

	public void SendChipsToFriend (long userId, long chips, Action<string> OnSuccess = null, Action OnFailure = null)
	{
		var args = new Dictionary<string, object> ();
		args.Add ("correlationId", Guid.NewGuid ().ToString ());
		
		//args.Add ("playerId", playerId);
		//args.Add ("message", JsonConvert.SerializeObject (message, Formatting.None, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All }).Replace (CLIENT_ASSEMBLY_REFERENCE, SERVER_ASSEMBLY_REFERENCE));
		args.Add ("userId", userId);
		args.Add ("chips", chips);
		
		args.Add ("action", SEND_CHIPS);
		
		string json = JsonConvert.SerializeObject (args);
		
		Debug.LogError ("SendChipsToFriend: " + json);
		
		GeneralQuery ("SendChipsToFriend", SEND_CHIPS, json, OnSuccess, OnFailure);
	}
	
	public void SendEmoji (string roomId, int messageType, string emojiname, Action<string> OnSuccess = null, Action OnFailure = null)
	{
		var args = new Dictionary<string, object> ();
		args.Add ("correlationId", Guid.NewGuid ().ToString ());
		
		//args.Add ("playerId", playerId);
		//args.Add ("message", JsonConvert.SerializeObject (message, Formatting.None, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All }).Replace (CLIENT_ASSEMBLY_REFERENCE, SERVER_ASSEMBLY_REFERENCE));
		args.Add ("roomId", roomId);
		args.Add ("message", emojiname);
		args.Add ("messageType", messageType);

		args.Add ("action", ROOM_CHAT_REQUEST);
		
		string json = JsonConvert.SerializeObject (args);
		
		Debug.LogError ("SendEmoji: " + json);
		
		GeneralQuery ("SendEmoji", ROOM_CHAT_REQUEST, json, OnSuccess, OnFailure);
	}
	


	private void GeneralQuery (string debugString, string actionID, string json, Action<string> OnSuccess = null, Action OnFailure = null)
	{
		if (Enabled) {

			new WebSocket_Client.Request (actionID, json)
				.OnReply ((reply) => {
				//try {
				//if (!debugString.Equals ("PingBack"))
				Debug.Log ("~~~ Successfully " + debugString + " on Server: " + reply.DataAsString);
				var response = JsonConvert.DeserializeObject<Dictionary<string,object>> (reply.DataAsString);
				bool success = false;
				if (response.ContainsKey ("success"))
					success = (bool)response ["success"];
				if (/*success && */OnSuccess != null) {
					OnSuccess (reply.DataAsString);
				} else if (OnFailure != null) {
					OnFailure ();
				}
//				} catch (Exception _ex) {
//					Debug.LogError ("GeneralQuery " + debugString + "-Exception:  " + _ex.Message);
//				}
                    
			})
//					.OnFailed ((AMQP.OutboundMessage message) => {
//						Debug.Log ("... Failed " + debugString + " on Server: " + message.DataAsString);
//						if (OnFailure != null) {
//							OnFailure ();
//                            GameManager.instance.ShowSyncPopUp ();
//                        }
//                    })
					.Send (); 
		} else 
			Debug.LogError ("Networking is Disabled.");
	}


}
