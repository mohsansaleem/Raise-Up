using System.Collections;
using System.Net.Sockets;
using Client.Utils;
using System;
using System.Text;
using System.IO;
using Client.Utils.Enums;
using Client.Misc;
using TienLen.Core.Models;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Client.Core
{
	public class ClientConnection {

//		private byte[] readBuffer = new byte[ClientConfigs.READ_BUFFER_SIZE];
//
//		private string strMessage;
//		private string res;
//		private string pUserName;
//
//		private TcpClient client;
//		private ArrayList lstUsers = new ArrayList();
//		private Dictionary<string, Action<object[]>> actionCallbacks;
//
//		private RoomState _roomState;
//		public RoomState RoomState {
//			get {
//				return _roomState;
//			}
//			set {
//				_roomState = value;
//			}
//		}
//
//		private ConnectionStateDetailed _connectionState;
//		public ConnectionStateDetailed ConnectionState {
//
//			get {
//				return _connectionState;
//			}
//			set {
//
//				_connectionState = value;
//				ClientEvents.OnConnectionStateDetailedChanged(_connectionState);
//			}
//		}
//
//		public bool IsConnected {
//
//			get {
//				return ConnectionState == ConnectionStateDetailed.Connected || client != null ? client.Connected : false;
//			}
//		}
//
//		public ClientConnection()
//		{
//			actionCallbacks = new Dictionary<string, Action<object[]>>();
//		}
//
//		public void TryConnect(string sNetIP, int iPORT_NUM, string sUserName)
//		{
//			try 
//			{
//				ConnectionState = ConnectionStateDetailed.Initializing;
//				pUserName = sUserName;
//				// The TcpClient is a subclass of Socket, providing higher level 
//				// functionality like streaming.
//				client = new TcpClient(sNetIP, iPORT_NUM);
//				//client.BeginConnect(sNetIP, iPORT_NUM, null, null);
//
//				ConnectionState = ConnectionStateDetailed.Initialized;
//				// Start an asynchronous read invoking DoRead to avoid lagging the user
//				// interface.
//				client.GetStream().BeginRead(readBuffer, 0, ClientConfigs.READ_BUFFER_SIZE, new AsyncCallback(DoRead), null);
//				// Make sure the window is showing before popping up connection dialog.
//				
//				AttemptLogin(sUserName);
//				//return "Connection Succeeded";
//			} 
//			catch(Exception ex)
//			{
//				client = null;
//				ConnectionState = ConnectionStateDetailed.Disconnected;
//				//return "Server is not active.  Please start server and try again.      " + ex.ToString();
//				Logger.Log(ex, LogType.Exception);
//			}
//		}
//
//		public void AttemptLogin(string user)
//		{
//			ConnectionState = ConnectionStateDetailed.Connecting;
//			SendData(string.Concat(ClientRequests.CONNECT.ToString(), ClientConfigs.DATA_SEPERATOR, user));
//		}
//		
//		public void ChatMessage(string chat)
//		{
//			SendData(string.Concat(ClientRequests.CHAT.ToString(), ClientConfigs.DATA_SEPERATOR, chat));
//		}
//		
//		public void TryDisconnect()
//		{
//			if(client != null)
//			{
//				if(client.Connected && client.GetStream() != null)
//					SendData(ClientRequests.DISCONNECT.ToString());
//
//				client.Close();
//			}
//		}
//		
//		public void GetListUsers()
//		{
//			SendData(ClientRequests.REQUESTUSERS.ToString());
//		}
//
//		public void JoinGame(string joinMsg, Action<object[]> joinGameCallback, Action<IActionMessage> tableUpdateCallback) 
//		{
//			actionCallbacks.Add(ClientRequests.JOINGAME.ToString(), joinGameCallback);
//			actionCallbacks.Add(ClientRequests.TABLEUPDATE.ToString(), tableUpdateCallback);
//			SendData(string.Concat(ClientRequests.JOINGAME.ToString(), ClientConfigs.DATA_SEPERATOR, joinMsg));
//		}
//
//		public void SubmitCards(string cardsData)
//		{
//			SendData(string.Concat(ClientRequests.TABLEUPDATE.ToString(), ClientConfigs.DATA_SEPERATOR, cardsData));
//		}
//
//		public void SkipTrun(string skipData)
//		{
//			SendData(string.Concat(ClientRequests.TABLEUPDATE.ToString(), ClientConfigs.DATA_SEPERATOR, skipData));
//		}
//
//		private void DoRead(IAsyncResult ar)
//		{ 
//			int BytesRead;
//			try
//			{
//				if(!client.Connected)
//				{
//					res="Disconnected";
//					ConnectionState = ConnectionStateDetailed.Disconnected;
//					return;
//				}
//
//				// Finish asynchronous read into readBuffer and return number of bytes read.
//				BytesRead = client.GetStream().EndRead(ar);
//				if (BytesRead < 1) 
//				{
//					// if no bytes were read server has close.  
//					res="Disconnected";
//					ConnectionState = ConnectionStateDetailed.Disconnected;
//					return;
//				}
//				// Convert the byte array the message was saved into, minus two for the
//				// Chr(13) and Chr(10)
//				strMessage = Encoding.ASCII.GetString(readBuffer, 0, BytesRead - 2);
//				foreach(string message in strMessage.Split((char) 13))
//				{
//					ProcessCommands(message);
//				}
//				// Start a new asynchronous read into readBuffer.
//				client.GetStream().BeginRead(readBuffer, 0, ClientConfigs.READ_BUFFER_SIZE, new AsyncCallback(DoRead), null);
//			} 
//			catch (Exception ex)
//			{
//				res="Disconnected";
//				ConnectionState = ConnectionStateDetailed.Disconnected;
//				client.Close();
//				Logger.Log(ex, LogType.Exception);
//			}
//		}
//		
//		// Process the command received from the server, and take appropriate action.
//		private void ProcessCommands(string strMessage)
//		{
//			Logger.Log(strMessage);
//			string[] dataArray;
//			
//			// Message parts are divided by "|"  Break the string into an array accordingly.
//			dataArray = strMessage.Split(ClientConfigs.DATA_SEPERATOR);
//			// dataArray(0) is the command.
//			ServerResponses response = (ServerResponses)Enum.Parse(typeof(ServerResponses), dataArray[0]);
//			switch(response)
//			{
//			case ServerResponses.CONNECTED:
//				ConnectionState = ConnectionStateDetailed.Connected;
//				res = "Connected";
//				break;
//			case ServerResponses.JOINED:
//				// Server acknowledged login.
//				res= "You have joined the chat";
//				break;
//			case ServerResponses.CHAT:
//				// Received chat message, display it.
//				res=  dataArray[1].ToString();
//				break;
//			case ServerResponses.REFUSED:
//				// Server refused login with this user name, try to log in with another.
//				//AttemptLogin(pUserName);
//				res=  "Attempted Re-Login";
//				break;
//			case ServerResponses.LISTUSERS:
//				// Server sent a list of users.
//				ListUsers(dataArray);
//				break;
//			case ServerResponses.BROADCAST:
//				// Server sent a broadcast message
//				res=  "ServerMessage: " + dataArray[1].ToString();
//				break;
//			case ServerResponses.READYTOSTART:
//				res=  "ServerMessage: " + dataArray[1].ToString();
//				RoomState = RoomState.READY;
//				if(actionCallbacks.ContainsKey(ClientRequests.JOINGAME.ToString()) && actionCallbacks[ClientRequests.JOINGAME.ToString()] != null)
//				{
//					actionCallbacks[ClientRequests.JOINGAME.ToString()](new object[] {dataArray[1]});
//					actionCallbacks.Remove(ClientRequests.JOINGAME.ToString());
//				}
//				//ClientEvents.OnPlayerStateChanged(dataArray[1].ToObject<Player>(TypeNameHandling.All));
//				break;
//			case ServerResponses.TABLEUPDATE:
//				res = "Table Update: " + dataArray[1] + " " + dataArray[2];
//				//TableAction tableAction = (TableAction)Enum.Parse(typeof(TableAction), dataArray[1]);
//				if(actionCallbacks.ContainsKey(ClientRequests.TABLEUPDATE.ToString()) && actionCallbacks[ClientRequests.TABLEUPDATE.ToString()] != null)
//				{
//					actionCallbacks[ClientRequests.TABLEUPDATE.ToString()](new object[] {dataArray[1], dataArray[2]});
//				}
//				//ClientEvents.OnTableStateChanged(tableAction, dataArray[2]);
//				break;
//			}
//
//			//res = "ClientConnection: " + res;
//			//Logger.Log(res);
//		}
//
//		// Use a StreamWriter to send a message to server.
//		private void SendData(string data)
//		{
//			StreamWriter writer = new StreamWriter(client.GetStream());
//			writer.Write(data + (char) 13);
//			writer.Flush();
//		}
//
//		private void ListUsers(string[] users)
//		{
//			int I;
//			lstUsers.Clear();
//			for (I = 1; I <= (users.Length - 1); I++)
//			{
//				lstUsers.Add(users[I]);	
//			}
//		}
	}
}