using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using Global;
using Newtonsoft.Json;
using System.Collections.Generic;
using Game.Managers;
using TienLen.Core.GamePlay;

public class ChatUIHandler : MonoBehaviour
{

	private Text chatText;
	public bool GlobalChat = true;
	public Color[] colorArray;
	private int colorArrayCounter = 0;
	private IDictionary<string,string> userColorMap = new Dictionary<string, string> ();
	public Animator roomChatSwipeBack;
	public GameObject RoomChat;
	public Animator roomChatSwipe;

	// Use this for initialization
	void Start ()
	{
		//userColorMap = new IDictionary<string,string> ();
		//colorArray = new List<string> ();
		if (GlobalChat) {
			WebSocket_Client.Client.Instance.globalChatPushDelegate += GlobalChatMessageRecieved;
		} else {
			WebSocket_Client.Client.Instance.roomChatPushDelegate += RoomChatMessageRecieved;
		}
		//Color.
		//colorArray[0] = (Color.red.ToHexStringRGB());
		//Debug.Log ("colorvalue: " + GetColor());
		chatText = gameObject.GetComponentInChildren<Text> ();
	
	}

	public void GetRoomChatHistory ()
	{
		Networking.Instance.RoomChatHistoryRequest (((Game.UI.Lobby)Game.UI.Lobby.Instance).SelectedRom.Room.id, (response) =>
		{
			Debug.Log ("Chat History Response Room: " + response);
			string[] chatHistory = JsonConvert.DeserializeObject<ChatHistoryResponse> (response).history;
			Array.Reverse (chatHistory);
			foreach (string msg in chatHistory) {
				string[] divided = msg.Split (':');
				divided [0] = divided [0].Remove (divided [0].Length - 1, 1);
				divided [1] = divided [1].Remove (0, 1);
				string color = GetColorForName (divided [0]);
				chatText.text += Environment.NewLine + "<color=#" + color + ">" + divided [0] + "</color>" + " : " + divided [1];
			}
		}, () => {});
	}

	public void GetGlobalChatHistory ()
	{
		Networking.Instance.GlobalChatHistoryRequest ((response) =>
		{
			Debug.Log ("Chat History Response Global: " + response);
			string[] chatHistory = JsonConvert.DeserializeObject<ChatHistoryResponse> (response).history;
			Array.Reverse (chatHistory);
			foreach (string msg in chatHistory) {
				string[] divided = msg.Split (':');
				divided [0] = divided [0].Remove (divided [0].Length - 1, 1);
				divided [1] = divided [1].Remove (0, 1);
				string color = GetColorForName (divided [0]);
				chatText.text += Environment.NewLine + "<color=#" + color + ">" + divided [0] + "</color>" + " : " + divided [1]; 
			}
		}, () => {});
	}
	
	public void RoomChatMessageRecieved (ChatMessage msg)
	{
		if(msg.messageType == 100)
			return;
		string color = GetColorForName (msg.user.username);
		chatText.text += (Environment.NewLine + "<color=#" + color + ">" + msg.user.username + "</color>" + " : " + msg.message);
		Debug.Log ("ChatMsg: " + chatText.text);
		Debug.LogError("ChatMessage type = "+msg.messageType);

		var screen = UIManager.Instance.GetTop ();
		if (screen != null && screen is TienLenClient)
			(screen as TienLenClient).ShowMessage (msg);

		//msg.text = "";
		//gameObject.GetComponent<ScrollRect> ().verticalNormalizedPosition = 0;
		Invoke ("RepositionScrollRect", 0.1f);
	}

	public void GlobalChatMessageRecieved (ChatMessage msg)
	{
		if(msg.messageType == 100)
			return;

		string color = GetColorForName (msg.user.username);
		chatText.text += (Environment.NewLine + "<color=#" + color + ">" + msg.user.username + "</color>" + " : " + msg.message);
		Debug.Log ("ChatMsg: " + chatText.text);
		//msg.text = "";
		//gameObject.GetComponent<ScrollRect> ().verticalNormalizedPosition = 0;
		Invoke ("RepositionScrollRect", 0.1f);
	}

	public void RepositionScrollRect ()
	{
		gameObject.GetComponent<ScrollRect> ().verticalNormalizedPosition = 0;
	}

	public void SendGlobalMessage (InputField gm)
	{
		if (gm.text == "")
			return;
		//UIManager.Instance.ShowUI (Game.Enums.GameUI.Lobby, Game.Enums.PreviousScreenVisibility.HidePreviousExcludingHud);
		Networking.Instance.GlobalChatRequest (gm.text, (response) =>
		{
			Debug.Log ("Chat Response Global: " + response);
		}, () => {});
		gm.text = "";
	}
	
	public void SendRoomMessage (InputField rm)
	{
		if (rm.text == "")
			return;
		//UIManager.Instance.ShowUI (Game.Enums.GameUI.Lobby, Game.Enums.PreviousScreenVisibility.HidePreviousExcludingHud);
		Networking.Instance.RoomChatRequest (((Game.UI.Lobby)Game.UI.Lobby.Instance).SelectedRom.Room.id, rm.text, (response) =>
		{
			Debug.Log ("Chat Response Room: " + response);
			//RoomChat = GameObject.FindGameObjectWithTag("RoomChat");

			//roomChatSwipeBack = RoomChat.GetComponent<Animator> ();
			//roomChatSwipeBack.Play ("RoomChatSwipeBack");
			//roomChatSwipeBack.enabled=false;

		}, () => {});
		rm.text = "";

		Networking.Instance.GetRoomUsers (((Game.UI.Lobby)Game.UI.Lobby.Instance).SelectedRom.Room.id, (response) =>
		{
			Debug.Log ("Room Users Response: " + response);
		}, () => {});

	
	}

	public void ClearChat ()
	{
		chatText.text = "";
		userColorMap.Clear ();
	}

	private String GetColorForName (string name)
	{
		if (userColorMap.ContainsKey (name)) {
			return userColorMap [name];
		} else {
			string colorForUser = GetUniqueColor ();
			userColorMap.Add (name, colorForUser);
			return colorForUser;
		}
	}

	private String GetUniqueColor ()
	{
		if (colorArray == null)
			return null;
		int count = colorArray.Length;
		colorArrayCounter++;
		if (colorArrayCounter >= count) {
			colorArrayCounter = 0;
		}
		return ColorUtility.ToHtmlStringRGB(colorArray[colorArrayCounter]);
//		return colorArray [colorArrayCounter].ToHexStringRGB ();x
	}

	public void ShowChatBox()
	{
		RoomChat = GameObject.FindGameObjectWithTag ("RoomChat");
		roomChatSwipe = RoomChat.GetComponent<Animator> ();
		RoomChat.GetComponent<CanvasGroup> ().interactable = true;
		roomChatSwipe.enabled=true;
		roomChatSwipe.Play ("RoomChatSwipe");
		Debug.LogError ("Inside show chat box");
		
	}
	
	public void HideChatBox()
	{
		RoomChat = GameObject.FindGameObjectWithTag ("RoomChat");
		roomChatSwipe = RoomChat.GetComponent<Animator> ();
		RoomChat.GetComponent<CanvasGroup> ().interactable = false;
		roomChatSwipe.enabled=true;
		roomChatSwipe.Play ("RoomChatSwipeBack");
		//roomChatSwipeBack.enabled = false;
		Debug.LogError ("Inside Hide chat box");
		//RoomChat.GetComponent<CanvasGroup> ().interactable = false;
	}



}
