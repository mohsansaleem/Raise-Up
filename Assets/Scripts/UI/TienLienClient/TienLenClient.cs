using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using TienLen.Core.Models;
using TienLen.Core.Misc;
using TienLen.Utils;
using TienLen.Utils.Enums;
using TienLen.Utils.Constants;
using TienLen.Utils.Helpers;
using TienLen.Managers;
using TienLen.View;
using Client.Misc;
using Client.Utils.Enums;
using Newtonsoft.Json;
using Client.Utils;
using Zems.Common.Domain.Messages;
using Game;
using Game.UI;
using Global;
using Game.Managers;
using Game.Enums;

namespace TienLen.Core.GamePlay
{
	[System.Serializable]
	public class ImageHolders
	{

		public string playerName;
		public Image[] playerCards;
		[NonSerialized]
		public int
			cardsPlayed;
	}

	public class TienLenClient : Widget<TienLenClient>
	{
		public GameObject RoomChat;
		public Text[] PlayerCardsCount;
		
		[SerializeField]
		private GameObject
			waitingForResponsePanel;
		[SerializeField]
		private GameObject
			startPanel;
		[SerializeField]
		private GameObject
			inputDisablePanel;
		[SerializeField]
		private GameObject[]
		emojiImage;
		[SerializeField]
		private GameObject
		emojiPanel;
		[SerializeField]
		private RectTransform[]
		emojiPanelPositions;
		[SerializeField]
		private ImageHolders[]
			cardHolders; //Players cards
		[SerializeField]
		private ImageHolders[]
			cardsSlots; // CardsonTable
		[SerializeField]
		private HorizontalLayoutGroup
			playerCardsLayout;
		[SerializeField]
		private Image[]
		playersActionSprites; //Turn nd Skip sprite
		[SerializeField]
		private Image[]
		playersPassSprites; //Turn nd Skip sprite
		[SerializeField]
		private Image[]
			playerCardSlots; // Lower Cards
		[SerializeField]
		private Text[]
			nameLabels;
		[SerializeField]
		private Button
			passButton;
		[SerializeField]
		private Button
			submitButton;
		[SerializeField]
		private List<GameObject>
			sitButtonsList;
		[SerializeField]
		private WWWLoadImage[]
			userImages;
		[SerializeField]
		private List<Image>
			progressBarList;
		[SerializeField]
		private Text
			GameLogs;
		[SerializeField]
		private VictoryPanel
			victoryPanel;
		[SerializeField]
		private MessagePopUp
			messagePopUp;
		[SerializeField]
		private Button
			chatButton;

		public Animator roomChatSwipe;
		public Animator roomChatSwipeBack;
		public GameType gameType;
		private IGame game;
		private Player[] players;

		private List<Image> selectedCards = new List<Image> ();
		private List<Card> cardsInHand = new List<Card> ();
		private int playerturn = -1, playerfield = -1;
		private int currentPlayerNo = -1;
		private bool allowFreeTurn = true, skipEnable = false;

		TienLenGameEngine tienLenGameEngine;
		private object lockObject = new object ();
		private List<Action> threadActionsQueue = new List<Action> ();


		public void OnEmojiButton(Image image) {
			if(gameType != GameType.MultiplayerAI)
				return;

			emojiPanel.SetActive(false);

			Networking.Instance.SendEmoji (((Game.UI.Lobby)Game.UI.Lobby.Instance).SelectedRom.Room.id, 100 ,image.sprite.name, (response) => {
				Debug.Log ("SendEmoji Response Room: " + response);
			}, () => {});

		}

		private void OnEmojiMessageReceived(ChatMessage msg) {
			if(msg.messageType == 100) {

				Room room = ((Lobby)Lobby.Instance).SelectedRom.Room;
				var arr = room.roomUsers.Where (user => user.userType == UserType.Player).ToList ();
				
				User usr = arr.Find (x => x.userID == msg.user.userID);
				int seat = usr.seat;

				emojiImage[seat].GetComponent<Image>().sprite = (Sprite) Resources.Load("Textures/Emojis/New/"+msg.message,typeof(Sprite));
				Animator emojiAnimator = emojiImage[seat].GetComponent<Animator>();
				emojiAnimator.enabled = true;
				emojiAnimator.SetTrigger("Appear");
				emojiImage[seat].GetComponent<EmojiEnabler>().SetAbove();
				Debug.LogError("Emoji recei");
			}
		}

		IEnumerator ExcecuteAction ()
		{
			while (true) {
				yield return new WaitForEndOfFrame ();

				lock (lockObject) {
					if (threadActionsQueue.Count > 0) {
//						Debug.LogError ("Thread List" + threadActionsQueue [0].ToString ());
						try {
							threadActionsQueue [0] ();
						} catch (Exception ex) {
							Debug.LogError ("Queue Execution Error: " + ex.ToString ());
						}
						threadActionsQueue.RemoveAt (0);
					}
				}
			}
		}
		
		void AddActionForExecution (Action actionToAdd)
		{
			lock (lockObject) {
				threadActionsQueue.Add (actionToAdd);
			}
		}
		
		void OnDestroy ()
		{
			threadActionsQueue.Clear ();
			threadActionsQueue = null;
		}

		private void EndGame (Player[] players)
		{
			Debug.LogError("Game End called");
			StopCoroutine ("StartTurnTime");
			ResetTurnProgress ();

			TableStateChanged (new PlayersStatus (players));
			Logger.Log ("Game Ended", LogType.Error);

			string resultName = "";
			string resultScores = "";
			string result = "Result";
			foreach (Player player in players) {
				Debug.LogError("ID = "+player.ID+" DB ID = "+DataBank.Instance.user.userID);
				Debug.LogError("gametype = check1"+(gameType == GameType.MultiplayerAI));
//				Debug.LogError("gametype = check2"+(player.ID.Equals (DataBank.Instance.user.userID.ToString())));
				Debug.LogError("gametype = check 1 and 2"+(gameType == GameType.MultiplayerAI && player.ID.Equals (DataBank.Instance.user.userID)));
				if ((gameType == GameType.MultiplayerAI && player.ID.Equals (DataBank.Instance.user.userID)) || (gameType != GameType.MultiplayerAI && player.Number == 3)) {
//					DataBank.Instance.user.gameState.wallet.chips += player.Chips;
//					DataBank.Instance.user.gameState.version++;
//					DataBank.Instance.UpdateGameStateOnServer (DataBank.Instance.user);
					DataBank.Instance.AddChipsOnServerAndClient(player.Chips);
					if (gameType != GameType.MultiplayerAI) {
						DataBank.Instance.UpdateGameStateOnServer (DataBank.Instance.user);
					}
					if (player.Results.Place == 4)
						result = "You Lose";
					else
						result = "You Win";
				}

				resultName += player.Name + "\n";
				resultScores += player.Chips + "\n";
			}

			victoryPanel.ReplayButton.onClick.RemoveAllListeners ();
			victoryPanel.ReplayButton.onClick.AddListener (() => {
				PopupManager.Instance.HidePopup ();

				if (gameType == GameType.MultiplayerAI) {
					ResetTable ();
					RefreshTable ();
					//OnExitClick ();
				} else if (gameType == GameType.Offline) {
					int buyIns = 1000;
					GameManager.Instance.StartOffline (buyIns);
				}
			});

			victoryPanel.LobbyButton.onClick.RemoveAllListeners ();
			victoryPanel.LobbyButton.onClick.AddListener (() => {
				victoryPanel.LobbyButton.enabled = false;
				if (gameType == GameType.MultiplayerAI) {
//					UIManager.Instance.GetScreen(GameUI.Lobby).
					Room room = ((Lobby)Lobby.Instance).SelectedRom.Room;
					Networking.Instance.LeaveRoom (DataBank.Instance.user.userID.ToString (), room.id, (reply) => {
						PopupManager.Instance.HidePopup ();
						room = null;
						var roomChatGO = GameObject.FindGameObjectWithTag ("RoomChat");
						if (roomChatGO != null)
							roomChatGO.GetComponentInChildren<ChatUIHandler> ().ClearChat ();
						UIManager.Instance.ShowUI (GameUI.Lobby, PreviousScreenVisibility.HidePreviousIncludingHud);
					}, () => {});
				} else {

					PopupManager.Instance.HidePopup ();
					GameObject.FindGameObjectWithTag ("RoomChat").GetComponentInChildren<ChatUIHandler> ().ClearChat ();
					UIManager.Instance.ShowUI (GameUI.Lobby, PreviousScreenVisibility.HidePreviousIncludingHud);
				}
				victoryPanel.LobbyButton.enabled = true;
				//UIManager.Instance.ShowUI (GameUI.Login, PreviousScreenVisibility.DequeuePreviousExcludingHud);
			});

			victoryPanel.gameObject.SetActive (true);
			victoryPanel.PopulateData (resultName, resultScores, result);

			WebSocket_Client.Client.Instance.roomChatPushDelegate -= OnEmojiMessageReceived;
			foreach(Text text in PlayerCardsCount) {
				text.gameObject.SetActive(false);
			}

		}

		public void ResetTable ()
		{
			victoryPanel.gameObject.SetActive (false);
			GameLogs.text = "";

			ResetTurnProgress ();

			ResetPlayerActionSprites ();
			RemoveAllPlayerSlotCards ();
			ResetHolderCards ();
			ResetPlayerCardsSlots ();

			waitingForResponsePanel.SetActive (false);

			cardsInHand.Clear ();
			selectedCards.Clear ();
			if (game != null)
				((TienLenGame)game).FieldType = FieldType.None;

			foreach (var btn in sitButtonsList)
				btn.SetActive (false);

			threadActionsQueue.Clear ();

	
			WebSocket_Client.Client.Instance.roomChatPushDelegate += OnEmojiMessageReceived;
		}

		private void RestartGame ()
		{
			ResetTable ();

			tienLenGameEngine.InitGame ();
		}

		private void SetPlayerActionSprite (ActionType type)
		{
			SetPlayerActionSprite (playerturn, type);
		}

		private void SetPlayerActionSprite (int playerNo, ActionType type)
		{
			playersActionSprites [playerNo].enabled = false;
			playersPassSprites [playerNo].enabled = false;
			switch (type) {
			case ActionType.Turn:
				playersActionSprites [playerNo].sprite = TienLen.Utils.ResourceHelper.TryFetchResource<Sprite> (GameConstants.MISC_PATH + GameConstants.TURN_CARD);
				playersActionSprites [playerNo].enabled = true;
				break;
				
			case ActionType.Skipped:
				playersPassSprites [playerNo].sprite = TienLen.Utils.ResourceHelper.TryFetchResource<Sprite> (GameConstants.MISC_PATH + GameConstants.PASSED_CARD);
				playersPassSprites [playerNo].enabled = true;
				break;
				
			case ActionType.TurnCompleted:
				playersActionSprites [playerNo].enabled = false;
				playersPassSprites [playerNo].enabled = false;
				break;
			}
		}

		void SetPlayerCards (Player player)
		{
			cardHolders [player.Number - 1].cardsPlayed = GameConstants.CARDS_PER_PLAYER - player.Cards.Count;

			int i = 0;
			for (i = 0; i < cardHolders [player.Number - 1].cardsPlayed; i++) {
				cardHolders [(player.Number - 1)].playerCards [i].gameObject.SetActive (false);
			}

			while (i < GameConstants.CARDS_PER_PLAYER) {
				cardHolders [(player.Number - 1)].playerCards [i++].gameObject.SetActive (true);
			}

			while (i < cardHolders [(player.Number - 1)].playerCards.Count())
				cardHolders [(player.Number - 1)].playerCards [i++].gameObject.SetActive (false);

			for (i = 0; i < player.CardsOnTable.Count; i++) {
				cardsSlots [player.Number - 1].playerCards [i].gameObject.SetActive (true);
				cardsSlots [player.Number - 1].playerCards [i].sprite = TienLen.Utils.ResourceHelper.TryFetchResource<Sprite> (GameConstants.CARDS_PATH + Helper.GetCardPath (player.CardsOnTable [i]));
			}

			if (gameType == GameType.Offline)
				currentPlayerNo = 2;
			else if (DataBank.Instance != null && DataBank.Instance.user != null) {
				if (player.ID == DataBank.Instance.user.userID.ToString ())
					currentPlayerNo = player.Number - 1;
			}

			if (player.SkippedTurn)
				SetPlayerActionSprite (player.Number - 1, ActionType.Skipped);

			if (currentPlayerNo == player.Number - 1)
				SetPlayerCards (player.Cards);
		}

		private void ResetPlayerActionSprites ()
		{
			foreach (Image actionSprite in playersActionSprites) {
				actionSprite.enabled = false;
			}
			foreach (Image passSprite in playersPassSprites) {
				passSprite.enabled = false;
			}
		}

		private void RemoveAllPlayerSlotCards ()
		{
			foreach (ImageHolders cardHolders in cardsSlots) {

				foreach (Image cardSlot in cardHolders.playerCards) {
					cardSlot.gameObject.SetActive (false);
				}
			}
		}

		private void RemoveCurrentPlayerSlots ()
		{
//			Debug.LogError ("RemoveCurrentPlayerSlots");
			foreach (Image cardSlot in cardsSlots[playerturn].playerCards) {
//				Debug.LogError ("Card Removing: " + cardSlot.name);
				cardSlot.gameObject.SetActive (false);
			}
		}

		private void ResetHolderCards ()
		{
			foreach (ImageHolders cardsHolder in cardHolders) {

				cardsHolder.cardsPlayed = 0;
				foreach (Image cardImage in cardsHolder.playerCards) {
					cardImage.gameObject.SetActive (true);
				}
			}
		}

		private void RemoveHolderCards (string[] cardNames = null)
		{
			//Debug.LogError ("Remove Cards: " + JsonConvert.SerializeObject (cardNames));


			RemoveCurrentPlayerSlots ();

			int append = cardHolders [playerturn].cardsPlayed;

			int i;
			for (i = 0; i < cardNames.Length; i++) {
				cardHolders [playerturn].playerCards [i + append].gameObject.SetActive (false);
				cardsSlots [playerturn].playerCards [i].gameObject.SetActive (true);
				cardsSlots [playerturn].playerCards [i].sprite = TienLen.Utils.ResourceHelper.TryFetchResource<Sprite> (GameConstants.CARDS_PATH + cardNames [i]);
			}

			cardHolders [playerturn].cardsPlayed += cardNames.Length;
//			Debug.LogError ("Cards Played: " + cardHolders [playerturn].cardsPlayed + ", CardsRemoved: " + cardNames.Length);
//			while (i < GameConstants.CARDS_PER_PLAYER && cardsSlots [playerturn].playerCards [i].gameObject.activeSelf)
//				cardsSlots [playerturn].playerCards [i++].gameObject.SetActive (false);
//
//			for (i = GameConstants.CARDS_PER_PLAYER - cardHolders [playerturn].cardsPlayed; i < GameConstants.CARDS_PER_PLAYER; i++)
//				cardHolders [playerturn].playerCards [i].gameObject.SetActive (false);

		}

		private void RemoveAllSelectedCards ()
		{
//			Debug.LogError ("RemoveAllSelectedCards");
			foreach (Image cardImage in selectedCards) {
//				Debug.LogError ("Removing Card: " + cardImage.name);
				cardImage.gameObject.SetActive (false);
			}

			selectedCards.Clear ();
		}

		private void ResetPlayerCardsSlots ()
		{
			foreach (Image playerCard in playerCardSlots) {

				playerCard.sprite = TienLen.Utils.ResourceHelper.TryFetchResource<Sprite> (GameConstants.CARDS_PATH + GameConstants.COVER_CARD);
				playerCard.color = Color.white;
				playerCard.gameObject.SetActive (true);
			}
		}

		private List<Card> GetSelectedCards ()
		{
			List<Card> cardsData = new List<Card> (GameConstants.CARDS_PER_PLAYER);

			foreach (Image cardImage in selectedCards) {
				cardsData.Add (cardImage.GetCardInfo ().Card);
			}

			return cardsData;
		}

		private string GetPlayerTurn ()
		{
			return "It is " + (playerturn == 0 ? "your" : players [playerturn].Name + "'s") +
				(players [playerturn].FreeTurn ? " free" : null) + " turn.";
		}

		private void SetPlayersInfo (string[] playerNames = null)
		{
			for (int i = 0; i < playerNames.Length &&( players == null || i < players.Length); i++) {
				nameLabels [i].text = playerNames [i];
				bool activeStatus = sitButtonsList [i].activeSelf;
				sitButtonsList [i].SetActive (playerNames [i].Trim ().Equals (""));
				if (activeStatus && !sitButtonsList [i].activeSelf)
					AudioManager.Instance.PlaySound (SoundType.PlayerJoining);
			}
		}

		private void SetPlayerCards (List<Card> cards)
		{
			string cardName = string.Empty;

			int i = 0;
			for (i = 0; i < playerCardSlots.Length && i < cards.Count; i++) {
				cardName = Helper.GetCardPath (cards [i]);
				playerCardSlots [i].name = cardName;
				playerCardSlots [i].sprite = TienLen.Utils.ResourceHelper.TryFetchResource<Sprite> (GameConstants.CARDS_PATH + cardName);

				CardInfo info = playerCardSlots [i].gameObject.GetComponent<CardInfo> ();
				info = info == null ? playerCardSlots [i].gameObject.AddComponent<CardInfo> () : info;
				if (info != null)
					info.Card = cards [i];
			}

			while (i < playerCardSlots.Count())
				playerCardSlots [i++].gameObject.SetActive (false);
		}

		private void ToggleUserGUI (bool enable)
		{
			passButton.interactable = submitButton.interactable = enable;
		}

		#region Events

		public void SubmitCards ()
		{
			//TODO Don't remove
			inputDisablePanel.SetActive (true);
			AudioManager.Instance.PlaySound (SoundType.CardPlayed);

			List<Card> cards = GetSelectedCards ();

			switch (gameType) {
			case GameType.Offline:
				if (cards.Count > 0) {
					SubmitCards submitCardsOffline = new SubmitCards (cards);
					tienLenGameEngine.ProcessClientRequest (submitCardsOffline);
					//TODO Don't remove
					inputDisablePanel.SetActive (false);
				} else {
					Action action = delegate {
						PopupManager.Instance.HidePopup ();
						//TODO Don't remove
						inputDisablePanel.SetActive (false);
					};
					ButtonData buttonData = new ButtonData ("Ok", true, action);
					
					List<ButtonData> bList1 = new List<ButtonData> ();
					bList1.Add (buttonData);
					
					PopupData popupData = new PopupData ("No Card Selected.", false, bList1);
					
					
					PopupManager.Instance.ShowPopup (PopupType.OneButton, popupData, Vector3.zero);
				}

				break;
			case GameType.MultiplayerAI:
				if (cards.Count > 0) {
					SubmitCards submitCards = new SubmitCards (cards);
					Networking.Instance.SentMessageToGameEngine (DataBank.Instance.user.userID.ToString (), submitCards, (str) => {
						Debug.Log ("SubmitCards Message Sent: " + str);

					}, () => {
						//TODO Don't remove
						inputDisablePanel.SetActive (false);
					});
				} else {
					Action action = delegate {
						PopupManager.Instance.HidePopup ();
						//TODO Don't remove
						inputDisablePanel.SetActive (false);
					};
					ButtonData buttonData = new ButtonData ("Ok", true, action);
					
					List<ButtonData> bList1 = new List<ButtonData> ();
					bList1.Add (buttonData);
					
					PopupData popupData = new PopupData ("No Card Selected.", false, bList1);
					
					
					PopupManager.Instance.ShowPopup (PopupType.OneButton, popupData, Vector3.zero);
				}
				break;
			}
		}

		public void SkipTurn ()
		{
			//TODO Don't remove
			inputDisablePanel.SetActive (true);

			switch (gameType) {
			case GameType.Offline:
				ToggleUserGUI (false);
				UnSelectAllCards ();
				tienLenGameEngine.EndTurn (true);
				inputDisablePanel.SetActive (false);
				break;
			case GameType.MultiplayerAI:
				ToggleUserGUI (false);
				UnSelectAllCards ();
				Networking.Instance.SentMessageToGameEngine (DataBank.Instance.user.userID.ToString (), new SkipTurn (), (str) => {
					Debug.Log ("SkipTurn Message Sent: " + str);
				
				}, () => {
					//TODO Don't Remove
					inputDisablePanel.SetActive (false);
				});
				//ClientManager.Instance.SkipTurn ();
				break;
			}
		}

		public void GetGameState ()
		{
			switch (gameType) {
			case GameType.Offline:
				ToggleUserGUI (false);
				UnSelectAllCards ();
				tienLenGameEngine.ProcessClientRequest (new GetGameState (""));
				break;
			case GameType.MultiplayerAI:
				ToggleUserGUI (false);
				UnSelectAllCards ();
				Networking.Instance.SentMessageToGameEngine (DataBank.Instance.user.userID.ToString (), new GetGameState (DataBank.Instance.user.userID.ToString ()), (str) => {
					Debug.Log ("GetGameState Message Sent: " + str);}, () => {
					//inputDisablePanel.SetActive (false);
				});
				break;
			}
		}

		public void SortPlayerCards ()
		{

//			List<Transform> list = playerCardsLayout.transform.
				playerCardsLayout.transform.Rotate(new Vector3(180, 180, 0));


			playerCardsLayout.enabled = false;
			this.PerformActionWithDelay (GameConfig.SORT_DELAY, () => {
				playerCardsLayout.enabled = true;
			});
		}

		public void CardSelected (Image cardImage)
		{

			CardInfo info = cardImage.GetCardInfo ();
			if (info != null) {
				info.Card.IsSelected = !info.Card.IsSelected;
//				Debug.LogError ("Card Selected: " + cardImage.name);
				if (info.Card.IsSelected) {
					cardImage.color = Color.yellow;
					selectedCards.Add (cardImage);
				} else {
					cardImage.color = Color.white;
					selectedCards.Remove (cardImage);
				}
			} else {
				Logger.Log ("Unable to find 'CardInfo' component on " + cardImage);
			}
		}

		private void ClearSelectedCards ()
		{
			foreach (Image cardImage in selectedCards) {
				cardImage.color = Color.white;
				cardImage.GetCardInfo ().Card.IsSelected = false;
			}
			selectedCards.Clear ();
		}

		public void UnSelectAllCards ()
		{
			foreach (Image cardImage in selectedCards) {
				cardImage.color = Color.white;
				cardImage.GetCardInfo ().Card.IsSelected = false;
			}

			selectedCards.RemoveRange (0, selectedCards.Count);
		}

		private void GameEvents_ClientPropChangeCalled (object sender, ClientPropChangeArgs e)
		{
			switch (e.PropChangeType) {
			case ClientPropChange.PlayerTurn:
				playerturn = (int)e.Info;
				break;
				
			case ClientPropChange.Skippable:
				skipEnable = (bool)e.Info;
				break;
				
			case ClientPropChange.AllowFreeTurn:
				allowFreeTurn = (bool)e.Info;
				break;
			}
		}



		public void OnTableStateChanged (IActionMessage innerMessage)
		{
			//Debug.LogError ("Adding to Queue: " + JsonConvert.SerializeObject (innerMessage));
			AddActionForExecution (() => TableStateChanged (innerMessage));
		}

		public void TableStateChanged (IActionMessage innerMessage)
		//public void Listen(TableStateEventArgs e)
		{
//			Debug.LogError ("Inner Message(" + innerMessage.GetType () + "): " + JsonConvert.SerializeObject (innerMessage));
			Debug.Log ("Inner Message(" + innerMessage.GetType () + "): " + JsonConvert.SerializeObject (innerMessage));
			if (innerMessage is GetGameStateResponse) {

				GetGameStateResponse getGameStateResponse = (GetGameStateResponse)innerMessage;

				Debug.LogError ("GetGameState: " + JsonConvert.SerializeObject (getGameStateResponse));

				ClearSelectedCards ();
				RemoveAllPlayerSlotCards ();
				ResetPlayerActionSprites ();
				ResetPlayerCardsSlots ();

				TableStateChanged (new PlayerNames (getGameStateResponse.Players.Select (player => player.Name).ToArray ()));
				players = getGameStateResponse.Players;

				foreach (Player player in getGameStateResponse.Players) {
					SetPlayerCards (player);
				}

				TableStateChanged (new PlayerTurn (getGameStateResponse.PlayerTurn, ""));
				TableStateChanged (new PlayerAction (ActionType.Turn));
				TableStateChanged (new PlayersStatus (getGameStateResponse.Players));

				foreach (Player player in players) {
					if (gameType == GameType.Offline && player.Number - 1 == 2)
						TableStateChanged (new PlayerState (player));
					else if (gameType == GameType.MultiplayerAI && player.ID == DataBank.Instance.user.userID.ToString ())
						TableStateChanged (new PlayerState (player));
				}

			} else if (innerMessage is PlayerTurn) {
				playerturn = ((PlayerTurn)innerMessage).PlayersTurn;

				foreach(Text text in PlayerCardsCount) {
					text.gameObject.SetActive(true);
				}

				StopCoroutine ("StartTurnTime");
				cardCount();
				StartCoroutine ("StartTurnTime");

				if (currentPlayerNo == playerturn) {
					inputDisablePanel.SetActive (false);
					ToggleUserGUI (true);
					AudioManager.Instance.PlaySoundWithDelay (SoundType.TurnNotification, 0.3f);
				}
			} else if (innerMessage is PlayersStatus) {
				var positions = ((PlayersStatus)innerMessage).GamePlayersStatus;
				GameLogs.text = "";
			
				foreach (var plyr in positions) {
					GameLogs.text += plyr.Name + ": " + plyr.Place.ToString () + " : " + plyr.Chips.ToString () + "\n";
				}
			} else if (innerMessage is PlayerAction) {
				ActionType actionType = ((PlayerAction)innerMessage).PlayerActionType;
				SetPlayerActionSprite (actionType);
	
				if(actionType == ActionType.TurnCompleted)
					AudioManager.Instance.PlaySound (SoundType.CardPlayed);

			} else if (innerMessage is PlayerState) {
				//Debug.Log("Before Receiving: e = "+e.Param.ToString());
				Player receiveData = ((PlayerState)innerMessage).Player;
				//Debug.Log("After Receiving.");
				//TODO: Get Players from server.

				players [receiveData.Number - 1] = receiveData;
				SetPlayerCards (receiveData.Cards);
				currentPlayerNo = receiveData.Number - 1;
			} else if (innerMessage is PlayerNames) {
//				Debug.LogError ("PlayerNames: " + JsonConvert.SerializeObject (innerMessage));
				string[] playerNames = ((PlayerNames)innerMessage).GamePlayerNames;
				SetPlayersInfo (playerNames);
			} else if (innerMessage is RemoveAllCardsOnTable) {
				RemoveAllPlayerSlotCards ();
			} else if (innerMessage is RemovePlayerCardsOnTable) {
				RemoveCurrentPlayerSlots ();
			} else if (innerMessage is RemovePlayerSelectedCards) {
				RemoveHolderCards (((RemovePlayerSelectedCards)innerMessage).CardNames);
			} else if (innerMessage is ResetAllPlayerActions) {
				ResetPlayerActionSprites ();
			} else if (innerMessage is ResetAllPlayerCards) {
				ResetHolderCards ();
			} else if (innerMessage is ResetPlayerCards) {
				ResetPlayerCardsSlots ();
			} else if (innerMessage is GameFinished) {
				EndGame ((innerMessage as GameFinished).Players);
			} else if (innerMessage is DoSubmitCards) {
				if (((DoSubmitCards)innerMessage).SubmitCards) {
					ToggleUserGUI (false);

					RemoveAllSelectedCards ();
					inputDisablePanel.SetActive (false);
				} else {
					ToggleUserGUI (true);

					Action action = delegate {
						PopupManager.Instance.HidePopup ();
						inputDisablePanel.SetActive (false);
					};
					ButtonData buttonData = new ButtonData ("Ok", true, action);
					
					List<ButtonData> bList1 = new List<ButtonData> ();
					bList1.Add (buttonData);
					
					PopupData popupData = new PopupData ("Invalid Move", false, bList1);
					
					PopupManager.Instance.ShowPopup (PopupType.OneButton, popupData, Vector3.zero);
				}
			}
		}

		#endregion

		public void InitParams (IGame _game, Player[] _players, GameType _gameType, int buyIns, TienLenGameEngine tienLenGameEngine = null)
		{
			StartCoroutine (ExcecuteAction ());

			game = _game;
			players = _players;
			gameType = _gameType;

			GameLogs.gameObject.SetActive (true/*gameType != GameType.Offline*/);


			Debug.Log ("Players: " + players.Length);

			GameEvents.ClientPropChangeCalled += new EventHandler<ClientPropChangeArgs> (GameEvents_ClientPropChangeCalled);
			game.Initialised (new GameInfo (players, gameType, buyIns));

			selectedCards = new List<Image> (GameConstants.CARDS_PER_PLAYER);

			cardsInHand = new List<Card> (GameConstants.CARDS_PER_PLAYER);

			switch (gameType) {
			case GameType.Offline:
				this.tienLenGameEngine = tienLenGameEngine;

				
				Action action = delegate {
					OnStartClickOffline();
					PopupManager.Instance.HidePopup ();
					
				};
				ButtonData buttonData = new ButtonData ("Ok", true, action);
				
				List<ButtonData> bList1 = new List<ButtonData> ();
				bList1.Add (buttonData);
				
				PopupData popupData = new PopupData ("Start Game", false, bList1);
				
				
				PopupManager.Instance.ShowPopup (PopupType.OneButton, popupData, Vector3.zero);


				break;

			case GameType.MultiplayerAI:
				// TODO change
				//ClientEvents.TableStateChanged += OnTableStateChanged;
				break;
			}
		}

		public void OnExitClick ()
		{
			if (gameType == GameType.MultiplayerAI) {
				Action action = delegate {
					Room room = ((Lobby)Lobby.Instance).SelectedRom.Room;
//					Networking.Instance.LeaveRoom (DataBank.Instance.user.userID.ToString (), room.id, (reply) => {
						PopupManager.Instance.HidePopup ();
						room = null;
						GameObject.FindGameObjectWithTag ("RoomChat").GetComponentInChildren<ChatUIHandler> ().ClearChat ();
						UIManager.Instance.ShowUI (GameUI.Lobby, PreviousScreenVisibility.HidePreviousIncludingHud);
						WebSocket_Client.Client.Instance.roomChatPushDelegate -= OnEmojiMessageReceived;
//					}, () => {});
//					RoomChat.GetComponent<Animator> ().enabled = false;
				};

				List<ButtonData> bList1 = new List<ButtonData> ();

				ButtonData buttonData = new ButtonData ("Yes(Leave)", true, action);
				bList1.Add (buttonData);

				ButtonData buttonDataHide = new ButtonData ("No", true, () => PopupManager.Instance.HidePopup ());
				bList1.Add (buttonDataHide);
						
				PopupData popupData = new PopupData ("Do You Want To Leave the Room?", true, bList1);
						
			
				PopupManager.Instance.ShowPopup (PopupType.OneButton, popupData, Vector3.zero);
			} else {
				Action action = delegate {
					PopupManager.Instance.HidePopup ();
					GameObject.FindGameObjectWithTag ("RoomChat").GetComponentInChildren<ChatUIHandler> ().ClearChat ();
					UIManager.Instance.ShowUI (GameUI.Lobby, PreviousScreenVisibility.HidePreviousIncludingHud);
					RoomChat.GetComponent<Animator> ().enabled = false;
				};
				
				List<ButtonData> bList1 = new List<ButtonData> ();
				
				ButtonData buttonData = new ButtonData ("Yes(Leave)", true, action);
				bList1.Add (buttonData);
				
				ButtonData buttonDataHide = new ButtonData ("No", true, () => PopupManager.Instance.HidePopup ());
				bList1.Add (buttonDataHide);
				
				PopupData popupData = new PopupData ("Do You Want To Leave the Game?", true, bList1);
				
				
				PopupManager.Instance.ShowPopup (PopupType.OneButton, popupData, Vector3.zero);
			}
		}

		public void OnStartClick ()
		{
			RoomChat.GetComponent<Animator> ().enabled = false;
			waitingForResponsePanel.SetActive (true);
			startPanel.SetActive (false);
			Networking.Instance.StartGameClientRequest ((((Lobby)Lobby.Instance).SelectedRom.Room.id), (str) => {
				startPanel.SetActive (false);
				waitingForResponsePanel.SetActive (false);
				foreach(Text text in PlayerCardsCount) {
					text.gameObject.SetActive(true);
				}
				if(game == null) {
					Debug.LogError("game is null");
				}
				if(game.GetGameInfo() == null) {
					Debug.LogError("gameinfo is null");
				}
				DataBank.Instance.AddChipsLocally(-1*game.GetGameInfo().BuyIns);
				Debug.LogError("Starting game now - add locally - "+game.GetGameInfo().BuyIns);
			},
			() => {
				startPanel.SetActive (true);
				waitingForResponsePanel.SetActive (false);
			});
		}

		public void OnStartClickOffline() {
			this.tienLenGameEngine.InitGame ();
			foreach(Text text in PlayerCardsCount) {
				text.gameObject.SetActive(true);
			}
		}

		public void ShowStartButton ()
		{
			startPanel.SetActive (true);
		}

		public void ShowUserProfile (int userNo)
		{
			Room room = ((Lobby)Lobby.Instance).SelectedRom.Room;
			var arr = room.roomUsers.Where (user => user.userType == UserType.Player).ToList ();
			
			foreach (var user in arr)
				Debug.Log ("Players seat: " + user.seat + " PLayerName: " + user.username);

			User usr = arr.Find (x => x.seat == userNo);
			//Debug.Log ("UserProfile: " + usr.username + " Chips:" + usr.gameState.games.Count);// + " Wins:" + usr.gameState.games.Count);
			if (usr != null) {
				if(usr.userProfile.userID == DataBank.Instance.user.userID) {
					emojiPanel.SetActive(true);
					RectTransform emojiRect = emojiPanel.GetComponent<RectTransform>();
					emojiRect.pivot = emojiPanelPositions[userNo].pivot;
					emojiRect.position = emojiPanelPositions[userNo].position;
					return;
				}

				if (WebSocket_Client.Client.Instance.friendsID != null && WebSocket_Client.Client.Instance.friendsID.Count != 0) {
					if (WebSocket_Client.Client.Instance.friendsID.Contains (usr.userID)) {
						UIManager.Instance.ShowUI (GameUI.UserProfile, PreviousScreenVisibility.DoNothing, usr, Profile.ProfileType.Friend);
					} else {
						UIManager.Instance.ShowUI (GameUI.UserProfile, PreviousScreenVisibility.DoNothing, usr, Profile.ProfileType.Other);
					}
				} else {
					Networking.Instance.ViewFriendsList ((response) =>
					{
						FriendsResponse friendsResponse = JsonConvert.DeserializeObject<FriendsResponse> (response);
						WebSocket_Client.Client.Instance.friendsID = friendsResponse.friends.Select (x => x.userID).ToList ();
						if (WebSocket_Client.Client.Instance.friendsID != null) {
							if (WebSocket_Client.Client.Instance.friendsID.Count != 0) {
								ShowUserProfile (userNo);
							} else {
								Debug.Log ("User Has No Friend!");
								UIManager.Instance.ShowUI (GameUI.UserProfile, PreviousScreenVisibility.DoNothing, usr, Profile.ProfileType.Other);
							}
						} else {
							Debug.Log ("FriendsList Null! Show Profile as Other");
							UIManager.Instance.ShowUI (GameUI.UserProfile, PreviousScreenVisibility.DoNothing, usr, Profile.ProfileType.Other);
						}
						Debug.Log ("ViewFriendsList Response: " + response);
					}
					, () => {});
				}
			}
		}

		override public void Show (params object[] param)
		{
			base.Show (param);


			startPanel.SetActive (false);

			if ((RoomChat.GetComponent<CanvasGroup> ().interactable) ) {
				onHideChatClick ();
			//	RoomChat.GetComponent<Animator> ().enabled = false;
			}
			resetCardNumber ();
			ResetTable ();
			GetGameState();
		}

	/*	override public void Hide()
		{
			base.Hide ();
			roomChatSwipe.enabled = restrue;
		}*/

		public void RefreshTable ()
		{
			if (Lobby.Instance == null || ((Lobby)Lobby.Instance).SelectedRom == null)
				return;
	
			if (gameType == GameType.Offline) {
				Debug.LogError ("Can't Refresh when game is offline.");
				return;
			}


			Room room = ((Lobby)Lobby.Instance).SelectedRom.Room;

			string[] names = new string[4];
			for (int i = 0; i < 4; i++)
				names [i] = "";

			var arr = room.roomUsers.Where (user => user.userType == UserType.Player).ToList ();

			foreach (var user in arr)
				names [user.seat] = user.username;

			foreach(Text text in PlayerCardsCount) {
				text.gameObject.SetActive(false);
			}

			foreach(WWWLoadImage userImage in userImages) {
				userImage.SetDefault();
			}

			foreach (var user in arr) {
				//WWWLoadImage userImage = nameLabels[user.seat].gameObject.GetComponentInChildren<WWWLoadImage>();
				WWWLoadImage userImage = userImages [user.seat];
				if (userImage != null) {
					if (user.userProfile.customFields != null) {
						if (user.userProfile.customFields.fbId != null) {
							userImage.LoadWithFBID (user.userProfile.customFields.fbId);
							Debug.LogError ("Setting FB Profile Picture");
						} else {
							userImage.SetDefaultProfilePicture();
						}
					} else {
						userImage.SetDefaultProfilePicture();
					}
				} else {
					Debug.LogError ("userImage null!");
//					userImage.SetDefault();
				}
			}

			var usr = room.roomUsers.Where (User => User.userID == DataBank.Instance.user.userID).SingleOrDefault ();

			if (usr != null && usr.seat != -1 && room.status == RoomStatus.InActive)
				startPanel.SetActive (!usr.startVote);
			else
				startPanel.SetActive (false);

			if (room.status == RoomStatus.InActive)
				TableStateChanged (new PlayerNames (names));
		}



		public void OnSitButtonClick (int seatNumber)
		{

			if (Lobby.Instance == null || ((Lobby)Lobby.Instance).SelectedRom == null)
				return;
			//sitButtonsList [seatNumber].SetActive (false);

			waitingForResponsePanel.SetActive (true);

			Networking.Instance.OccupySeatOnTable (seatNumber.ToString (), ((Lobby)Lobby.Instance).SelectedRom.Room.id, (reply) => {
				var response = JsonConvert.DeserializeObject<Dictionary<string,object>> (reply);

				var status = response ["status"].ToString ();
				if (status.Trim ().Equals ("1")) {
					List<ButtonData> bList1 = new List<ButtonData> ();
					
					ButtonData buttonDataHide = new ButtonData ("Ok", true, () => PopupManager.Instance.HidePopup ());
					bList1.Add (buttonDataHide);

					PopupData popupData = new PopupData ("Seat Already Occuppied . . .", false, bList1);

					PopupManager.Instance.ShowPopup (PopupType.OneButton, popupData, Vector3.zero);
				}

				waitingForResponsePanel.SetActive (false);

//				if (status.Trim ().Equals ("0")) {
//					ShowStartButton ();
//				}
			}, () => {
				//sitButtonsList [seatNumber].SetActive (true);
				waitingForResponsePanel.SetActive (false);
			});
		}

		void ResetTurnProgress ()
		{
			foreach (var prog in progressBarList)
				prog.fillAmount = 0f;
		}

		public virtual IEnumerator StartTurnTime ()
		{
			ResetTurnProgress ();

			float spentTime = (float)GameConfig.TURN_TIME;
			
			while (spentTime > 0) {
				yield return new WaitForEndOfFrame ();
				spentTime -= UnityEngine.Time.deltaTime;
				
				progressBarList [playerturn].fillAmount = spentTime / GameConfig.TURN_TIME;
			}

			if (gameType == GameType.Offline && currentPlayerNo == playerturn)
				SkipTurn ();
		}

		public void ShowMessage (ChatMessage chatMessage)
		{
			foreach (var name in nameLabels) {
				if (name.text.Equals (chatMessage.user.username)) {
					MessagePopUp messageWindow = Instantiate (messagePopUp);
					messageWindow.SetMessage = chatMessage.message;

					messageWindow.transform.position = name.transform.position;

					messageWindow.transform.SetParent (transform, true);
					messageWindow.transform.SetAsLastSibling ();
					messageWindow.transform.localScale = Vector3.one;
				}
			}
		}

		public void onShowChatClick()
		{
			RoomChat.GetComponentInChildren<ChatUIHandler> ().ShowChatBox ();
			chatButton.gameObject.SetActive(false);
		}

		public void onHideChatClick()
		{
			RoomChat.GetComponentInChildren<ChatUIHandler> ().HideChatBox();
			chatButton.gameObject.SetActive(true);
		}

		public void cardCount()
		{
			int i = 0;
			foreach (var cardHolder in cardHolders) {
				int count = 0;
				foreach( var card in cardHolder.playerCards )
					count += card.gameObject.activeSelf ? 1:0;
				PlayerCardsCount[i++].text = count.ToString();
			}
	}

		public void resetCardNumber()
		{
			for(int i=0;i<PlayerCardsCount.Length;i++) {
				PlayerCardsCount[i].text = "13";
			}
		}

		private void OnNotificationReceived(string message) {
			UIManager.Instance.ShowNotificationMessage(message);

		}
}
}