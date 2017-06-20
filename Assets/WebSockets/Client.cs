using UnityEngine;
using System.Collections;
using WebSocket4Net;
using System;
using SuperSocket.ClientEngine;
using System.Collections.Generic;

using Newtonsoft.Json;
using LogMessages;
using Game.Managers;

namespace WebSocket_Client
{
	public class Client : MonoBehaviour
	{

		public static Client Instance;
		public static bool PrintLogs = true;
		public static event Action OnConnected;
		public static event Action OnDisconnect;

		[HideInInspector]
		private string
			URI;

		[SerializeField]
		bool
			ConnectLocally;

		const string LocalIP = "ws://192.168.9.46:37113/";
//		const string ServerIP = "ws://fec2-52-39-58-82.us-west-2.compute.amazonaws.com";
//				const string ServerIP = "ws://fbomb.cloudapp.net:37113/websocket";
//		const string ServerIP = "ws://tienlien.frag-games.com:37113/websocket";
		const string ServerIP = "ws://ec2-52-39-58-82.us-west-2.compute.amazonaws.com:37113/websocket";
		//"ws://192.168.9.46:37113/";
		//"ws://fbomb.cloudapp.net:37113/websocket";

		private WebSocket
			websocket;
		private const string CORELATIONID = "correlationId";
		Dictionary<string,Request> UnReplied = new Dictionary<string, Request> ();
		Dictionary<string,Action<InBoundMessage>>  Handlers = new Dictionary<string, Action<InBoundMessage>> ();

		public List<Global.ChatMessage> GlobalMessages;
		public List<Global.ChatMessage> RoomMessages;
		public int GlobalMessagesLimit = 50;
		public int RoomMessagesLimit = 50;

		public List<int> friendsID;

		public delegate void RoomChatDelegate (Global.ChatMessage cm);
		public RoomChatDelegate roomChatPushDelegate;
		public delegate void GlobalChatDelegate (Global.ChatMessage cm);
		public GlobalChatDelegate globalChatPushDelegate;
		
//		public delegate void NotificationReceived (string message);
//		public NotificationReceived OnNotificationReceived;
//		
		void Awake ()
		{
			if (ConnectLocally)
				URI = LocalIP;
			else
				URI = ServerIP;

			Instance = this;
			GlobalMessages = new List<Global.ChatMessage> ();
			RoomMessages = new List<Global.ChatMessage> ();
			friendsID = new List<int> ();
		}

		public void Connect ()
		{
			this.Log ("[WS] Trying to connect");
			Debug.LogError ("URI: " + URI);
			websocket = new WebSocket (URI);
			websocket.Opened += new EventHandler (websocket_Opened);
			websocket.Error += new EventHandler<ErrorEventArgs> (websocket_Error);
			websocket.Closed += new EventHandler (websocket_Closed);
			websocket.MessageReceived += new EventHandler<MessageReceivedEventArgs> (websocket_MessageReceived);
			websocket.Open ();
		}

		private void websocket_Opened (object sender, EventArgs e)
		{
			if (OnConnected != null) {
				Debug.LogError ("[WS] Connected");
				OnConnected ();
			} else
				Debug.LogError ("OnConnected not Registered!!!");
		}

		void websocket_Error (object sender, EventArgs e)
		{
			Hashtable Ex = (e as ErrorEventArgs).Exception.Data as Hashtable;
			Debug.Log (Ex.Count);
			foreach (string key in Ex.Keys) {
				this.Log (String.Format ("{0}: {1}", key, Ex [key]));
			}
			this.Log ("Error: " + (e as ErrorEventArgs).Exception);

		}

		void websocket_Closed (object sender, EventArgs e)
		{
			this.Log ("Socket Closed: " + (e as ClosedEventArgs).Reason);
			UnReplied.Clear ();
			Handlers.Clear ();
		}

		void websocket_MessageReceived (object sender, EventArgs e)
		{
			string Response = (e as MessageReceivedEventArgs).Message;
			this.Log ("[Received] " + Response);
			var ResponseDict = JsonConvert.DeserializeObject<Dictionary<string,object>> (Response);
			string CorelationID = null;
			if (ResponseDict.ContainsKey (CORELATIONID)) {
				this.Log ("[Received] ");
				CorelationID = ResponseDict [CORELATIONID].ToString ();
				Request Req = null;
				if (UnReplied.TryGetValue (CorelationID, out Req)) {
					this.Log ("[Received] 1");
					if (Req.onReply != null) {
						this.Log ("[Received] 2 : " + Req.onReply);
//                        CorelationID = ResponseDict [CORELATIONID].ToString();
						Networking.Instance.ResponseProcessRequests.Enqueue (() => {
							Req.onReply (new Reply (CorelationID, null, Response));
						});
						// Req.onReply(new Reply(CorelationID, ResponseDict ["action"].ToString(), Response));

					}
				}
			} else {
				this.Log ("[WS] Push Received");
				string action = null;
				if (ResponseDict.ContainsKey ("action")) {
					this.Log ("[WS] Push Received 1");
					Action<InBoundMessage> CallBack = null;
					action = ResponseDict ["action"].ToString ();
					
					this.Log (Handlers.Count.ToString ());
					
					if (Handlers.TryGetValue (action, out CallBack)) {
						this.Log ("[WS] Push Received 2");
						Networking.Instance.ResponseProcessRequests.Enqueue (() => {
							CallBack (new InBoundMessage (action, Response));
						});
						
					}
				}
			}
		}

		public void Disconnect ()
		{
			if (OnDisconnect != null)
				OnDisconnect ();

			UnReplied.Clear ();
			Handlers.Clear ();

			websocket.Close ();
		}

		public void On (string ActionNo, Action<InBoundMessage> CallBack)
		{
			if (!Handlers.ContainsKey (ActionNo))
				Handlers.Add (ActionNo, CallBack);
			else
				Debug.LogError (ActionNo + " already exists!");
		}

		public void Send (string Json, Request req)
		{

			var MessageDict = JsonConvert.DeserializeObject<Dictionary<string,object>> (Json);
			string Temp = MessageDict [CORELATIONID].ToString ();
			this.Log (Temp);
			UnReplied.Add (Temp, req);

			this.Log ("[WS] Sent: " + Json);
			if (websocket.State == WebSocketState.Open)
				websocket.Send (Json);
		}

		void OnDestroy ()
		{
			Disconnect ();
		}

		public void GlobalChatMessageRecieved (Global.ChatMessage cm)
		{
			if(cm.messageType == 0) {
				GlobalMessages.Add (cm);
				if (GlobalMessages.Count > GlobalMessagesLimit) {
					GlobalMessages.RemoveAt (0);
				}
			}
			Debug.Log ("GlobalMessage Added to List. Count: " + GlobalMessages.Count);
			if (globalChatPushDelegate != null) {
				globalChatPushDelegate (cm);
			}
			
		}
		
		public void RoomChatMessageRecieved (Global.ChatMessage cm)
		{
			if(cm.messageType == 0) {
				RoomMessages.Add (cm);
				if (RoomMessages.Count > RoomMessagesLimit) {
					RoomMessages.RemoveAt (0);
				}
			}
			Debug.Log ("RoomMessage Added to List. Count: " + RoomMessages.Count);
			if (roomChatPushDelegate != null) {
				roomChatPushDelegate (cm);
			}
		}

		public void NotificationReceivedFromServer(string message) {
			UIManager.Instance.ShowNotificationMessage(message);
//			OnNotificationReceived(message);
		}

	}
}
