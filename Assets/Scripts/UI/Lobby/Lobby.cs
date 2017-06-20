using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Newtonsoft.Json;
using Game.Managers;
using Game.Enums;
using Global;
using TienLen.Managers;
using TienLen.Core.GamePlay;
using System.Linq;

namespace Game.UI
{
	public class Lobby : Widget<Lobby>
	{
		public GameObject RoomChat;
		public TienLenClient scriptT;

		[SerializeField]
		private Button
			_homeButton;
		[SerializeField]
		private Button
			_chatButton;
		[SerializeField]
		private Button
			_joinTableButton;
		[SerializeField]
		private Button
			_playerProfileButton;
		[SerializeField]
		private Text
			_currencyLabel;
		[SerializeField]
		private Text
			_playerNameLabel;

		[SerializeField]
		GameObject
			roomListNode;
		[SerializeField]
		GameObject
			playerListNode;

		[SerializeField]
		GameObject
			roomScrollView;
		[SerializeField]
		GameObject
			playersScrollView;
		[SerializeField]
		GameObject
			joinWaitingPanel;

		public static Action OnLobbyLoad;

		private List<RoomRow> RoomRowsList = new List<RoomRow> ();
		private List<PlayerRow> PlayerRowsList = new List<PlayerRow> ();

		private List<Room> rooms;

		private List<Room> Rooms {
			get {
				return rooms;
			}

			set {
				rooms = value;

				PopulateRoomNodes (rooms);
			}
		}

		private string activeRoomID;

		private RoomRow selectedRom;

		public RoomRow SelectedRom {
			get {
				return selectedRom;
			}
			set {
				//				if (selectedRom != null)
				//					selectedRom.Select (false);
				if (value == null) {
					//PopulatePlayerNodes (new List<User> ());
					return;
				} else {
					selectedRom = value;
					//					selectedRom.Select (true);
					//PopulatePlayerNodes (selectedRom.Room.roomUsers);
				}
				
				if(selectedRom.Room.id.Equals(activeRoomID))
					selectedRom.Select(true);
				else
					selectedRom.Select(false);
			}
		}

		public void UpdateRoom (Room room)
		{
			if (room == null)
				return;
			for (int i = 0; i < rooms.Count; i++) {
				if (rooms [i].id == room.id) {
					rooms [i] = room;
					//Refresh()
				}
			}

			Refresh ();
		}

		public void SelectRoom (string roomId)
		{
			var room = RoomRowsList.Where (row => row.Room.id.Equals (roomId)).SingleOrDefault ();
			if (room != null) {
				SelectedRom = room;
				Debug.LogError ("Selected Room Changed");
			}
		}

		public void SelectRoom (Room roomData)
		{
			var room = RoomRowsList.Where (row => row.Room.id.Equals (roomData.id)).SingleOrDefault ();
			if (room != null)
				SelectedRom = room;
		}

		public void RoomSelectFirstNodeOrSetNull ()
		{
			if (RoomRowsList.Count == 0)
				SelectedRom = null;
			else
				SelectedRom = RoomRowsList [RoomRowsList.Count - 1];
		}

		public void PopulateRoomNode (Room roomData)
		{
			RoomRow roomRow = (Instantiate (roomListNode) as GameObject).GetComponent<RoomRow> ();

			roomRow.Room = roomData;
			if(roomData.id.Equals(activeRoomID)) {
				roomRow.Select(true);
			} else {
				roomRow.Select(false);
			}

			roomRow.transform.SetParent (roomScrollView.transform, false);
			roomRow.transform.localPosition = new Vector3 (roomRow.transform.localPosition.x, roomRow.transform.localPosition.y, 0);
			roomRow.transform.localScale = Vector3.one;
			roomRow.transform.SetAsFirstSibling ();
			roomScrollView.GetComponent<ScrollContent> ().RefreshContent ();
			RoomRowsList.Add (roomRow);
		}

		public void PopulateRoomNodeValues (RoomRow roomNode, Room roomItem)
		{
			roomNode.Room = roomItem;
		}

		public void PopulateRoomNodes (List<Room> roomsData)
		{
			Debug.LogError ("PopulateInventoryNodes: " + roomsData.Count);
			int indexNode = 0;
			
			roomsData.ForEach (item => {
				if (indexNode == RoomRowsList.Count)
					PopulateRoomNode (item);
				else
					PopulateRoomNodeValues (RoomRowsList [indexNode], item);
				indexNode++;
			});
			while (indexNode < RoomRowsList.Count)
				DeleteRoomNode (RoomRowsList [indexNode]);

			if (SelectedRom != null) {
//				RoomSelectFirstNodeOrSetNull ();
//			else {
				var room = RoomRowsList.Where (row => row.Room.id == SelectedRom.Room.id).SingleOrDefault ();
				if (room != null)
					SelectedRom = room;
			}
		}
		
		public void DeleteRoomNode (RoomRow roomRom)
		{
//			if (SelectedRom == roomRom)
//				RoomSelectFirstNodeOrSetNull ();
			RoomRowsList.Remove (roomRom);
			DestroyImmediate (roomRom.gameObject);
		}

		public void PopulatePlayerNode (User userData)
		{
			PlayerRow playerRow = (Instantiate (playerListNode) as GameObject).GetComponent<PlayerRow> ();

			playerRow.User = userData;

			playerRow.transform.SetParent (playersScrollView.transform, false);
			playerRow.transform.localPosition = new Vector3 (playerRow.transform.localPosition.x, playerRow.transform.localPosition.y, 0);
			playerRow.transform.localScale = Vector3.one;
			playerRow.transform.SetAsFirstSibling ();

			playersScrollView.GetComponent<ScrollContent> ().RefreshContent ();
			PlayerRowsList.Add (playerRow);

		}

		public void PopulateRoomNodeValues (PlayerRow playerNode, User user)
		{
			playerNode.User = user;
		}

		public void PopulatePlayerNodes (List<User> usersData)
		{
			Debug.LogError ("PopulateInventoryNodes: " + usersData.Count);
			int indexNode = 0;

			usersData.ForEach (item => {
				if (indexNode == PlayerRowsList.Count)
					PopulatePlayerNode (item);
				else
					PopulateRoomNodeValues (PlayerRowsList [indexNode], item);
				indexNode++;
			});

			while (indexNode < PlayerRowsList.Count)
				DeletePlayerNode (PlayerRowsList [indexNode]);
		}

		public void DeletePlayerNode (PlayerRow playerRow)
		{
			PlayerRowsList.Remove (playerRow);
			DestroyImmediate (playerRow.gameObject);
		}

		public void JoinRoom ()
		{
			var roomId = selectedRom.Room.id;
			bool canJoin = false;
			Debug.LogError("Join Room called - "+roomId+" activeRoom "+activeRoomID);
			if(string.IsNullOrEmpty(activeRoomID)) {
				canJoin = true;
			} else {
				if(activeRoomID.Equals(roomId)) {
					canJoin = true;
				}
			}
			if(!canJoin) {
				Action action = delegate {
					PopupManager.Instance.HidePopup ();
				};
				ButtonData buttonData = new ButtonData ("Ok", true, action);
				
				List<ButtonData> bList1 = new List<ButtonData> ();
				bList1.Add (buttonData);
				
				PopupData popupData = new PopupData ("You are currently on an active room.", false, bList1);
				
				
				PopupManager.Instance.ShowPopup (PopupType.OneButton, popupData, Vector3.zero);
				return;
			}

			_joinTableButton.interactable = false;
			DateTime start = DateTime.Now;
			Networking.Instance.JoinRoom (DataBank.Instance.user.userID.ToString (), roomId, TienLen.Utils.Enums.UserType.Player, (response) => {
				UIManager.Instance.ShowUI (GameUI.TienLenClient, PreviousScreenVisibility.HidePreviousIncludingHud);
				_joinTableButton.interactable = true;
				DateTime final = DateTime.Now;
				Debug.LogError("time taken - "+(final - start).TotalMilliseconds);
				if (SelectedRom.Room.status == TienLen.Utils.Enums.RoomStatus.Active)
					GameManager.Instance.StartOnline (SelectedRom.Room.buyIn);
				else {
					var widget = UIManager.Instance.GetScreen (GameUI.TienLenClient);
					if (widget != null && (widget as TienLenClient).gameObject.activeSelf)
						(widget as TienLenClient).gameType = TienLen.Utils.Enums.GameType.MultiplayerAI;

					RefreshTable ();
				}
				GameObject.FindGameObjectWithTag ("RoomChat").GetComponentInChildren<ChatUIHandler> ().GetRoomChatHistory ();
			},
			() =>
			{
				_joinTableButton.interactable = true;
			});
		}

		override public void Show (params object[] param)
		{
			base.Show (param);

			DataBank.Instance.OnGameStateUpdate += PopulateUserData;

			joinWaitingPanel.SetActive (true);
			_joinTableButton.interactable = false;

//			Debug.LogError("calling isOnactive");
			Networking.Instance.IsOnActiveTable((str) => {
				Debug.LogError("isOnActive - selected room - "+str);
				var response = JsonConvert.DeserializeObject<IsOnActiveTableResponse> (str);
				//				if (response.success) {
				if(!response.Status && !string.IsNullOrEmpty(response.roomId)) {
					Networking.Instance.LeaveRoom (DataBank.Instance.user.userID.ToString (), response.roomId, (reply) => {
						Debug.LogError("calling leave Room cause status was true");
						response.roomId = "";
						ResetLobby(response);
					}, () => {});
				} else {
					ResetLobby(response);
				}
			}, () => {});
			AudioManager.Instance.PlayBackgroundMusic (BackgroundMusicType.LogoTheme);
		}

		private void ResetLobby(IsOnActiveTableResponse response) {
			if(!string.IsNullOrEmpty(response.roomId)) {
				activeRoomID = response.roomId;
				Debug.LogWarning("isOnActive - selected room - "+activeRoomID);
			} else {
				activeRoomID = "";
			}

			if (DataBank.Instance.user != null)
				PopulateUserData ();
			
			if (DataBank.Instance.LoggedIn)
				LoadLobby ();
			else if (!DataBank.Instance.LoggedIn && Networking.Instance.Connected)
				LogIn ();
			else if (!DataBank.Instance.LoggedIn && !Networking.Instance.Connected)
				Networking.Instance.OnConnected = LogIn;
		}

		public override void Hide ()
		{
			base.Hide ();
			DataBank.Instance.OnGameStateUpdate -= PopulateUserData;

			AudioManager.Instance.StopBackgroundMusic ();
		}

		override public void Destroy ()
		{
			DataBank.Instance.OnGameStateUpdate -= PopulateUserData;
			base.Destroy ();

			AudioManager.Instance.StopBackgroundMusic ();
		}

		void LogIn ()
		{
			DataBank.Instance.LogIn (onLogin: () => {
				PopulateUserData ();
				LoadLobby ();
			});
		}

		void PopulateUserData ()
		{
			Debug.LogError ("Lobby.PopulateUserData Called!!!");
			if (gameObject == null)
				return;
			_currencyLabel.text = DataBank.Instance.user.gameState.wallet.chips.ToString ();
			_playerNameLabel.text = DataBank.Instance.user.username;
			if (DataBank.Instance.user.userProfile.customFields != null)
				gameObject.GetComponentInChildren<WWWLoadImage> ().LoadWithFBID (DataBank.Instance.user.userProfile.customFields.fbId);
		}

		private void LoadLobby ()
		{
			Networking.Instance.ViewLobby ((response) =>
			{
				if (gameObject == null)
					return;
				LobbyResponse lobbyResponse = JsonConvert.DeserializeObject<LobbyResponse> (response);
				lobbyResponse.rooms.Sort((room1, room2) => room2.buyIn.CompareTo(room1.buyIn));
				Rooms = lobbyResponse.rooms;
				_joinTableButton.interactable = true;

				joinWaitingPanel.SetActive (false);

				if (OnLobbyLoad != null) {
					OnLobbyLoad ();
					OnLobbyLoad = null;
				}
			}, () => {});

		}


		public void OnHomeButtonClick ()
		{
			UIManager.Instance.ShowUI (GameUI.GameMenu, PreviousScreenVisibility.HidePreviousExcludingHud);
		}

		public void OnClatButton ()
		{

		}

		public void OnShopButtonClick ()
		{
			UIManager.Instance.ShowUI (GameUI.Market, Game.Enums.PreviousScreenVisibility.HidePreviousExcludingHud);
		}

		public void OnOfflineGameButtonClick ()
		{
			var lobby = UIManager.Instance.GetScreen (GameUI.Lobby);

			if (lobby != null)
				(lobby as Lobby).SelectedRom = null;

			int buyIns = 1000;
			GameManager.Instance.StartOffline (buyIns);
		}

		public void RefreshTable ()
		{
			var widget = UIManager.Instance.GetScreen (GameUI.TienLenClient);
			Debug.LogError ("RefreshTable");
			if (widget != null && (widget as TienLenClient).gameObject.activeSelf) {
				(widget as TienLenClient).RefreshTable ();
				Debug.LogError ("Refreshed");
			}
		}

		public void Refresh ()
		{
			Rooms = Rooms;

			RefreshTable ();
		}

		public void GameFinished() {
			activeRoomID = "";
			Rooms = Rooms;
		}

		public void ShowUserProfile ()
		{
			UIManager.Instance.ShowUI (GameUI.UserProfile, PreviousScreenVisibility.DoNothing, DataBank.Instance.user);
		}


	}
}