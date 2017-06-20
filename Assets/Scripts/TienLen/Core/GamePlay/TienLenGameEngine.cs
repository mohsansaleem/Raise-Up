using System;
using System.Collections;
using System.Collections.Generic;
using TienLen.Core.Models;
using TienLen.Core.Misc;
using TienLen.Utils;
using TienLen.Utils.Enums;
using TienLen.Utils.Constants;
using TienLen.Utils.Helpers;
using System.Threading;
using Client.Utils.Enums;
using Newtonsoft.Json;
using Client.Utils;
using Zems.Common.Domain.Messages;
using System.Linq;

namespace TienLen.Core.GamePlay
{
	public class TienLenGameEngine : GameEngine
	{
		public List<Card> cardsInHand;
		private int playerturn = -1, playerfield = -1;
		private bool allowFreeTurn = true, skipEnable = false;
		private int xplace = 0; // Temporary place position for winner
		
		private bool clockwiseTurn = true;

		private Action<IActionMessage> tableActionChanged;

		public TienLenGameEngine (IGame _game, Player[] _players, GameType _gameType, int butIns, Action<IActionMessage> actionChanged)
		//public TienLenGameEngine(IGame _game, Player[] _players, GameType _gameType, TienLenClient tienLenClient = null)
		{
			xplace = 0;

			game = _game;
			players = _players;

			gameType = _gameType;
			//this.tienLenClient = tienLenClient;
			tableActionChanged = actionChanged;
			
			GameEvents.ClientPropChangeCalled += new EventHandler<ClientPropChangeArgs> (GameEvents_ClientPropChangeCalled);
			game.Initialised (new GameInfo (players, gameType, butIns));
			
			//selectedCards = new List<Image>(GameConstants.CARDS_PER_PLAYER);
			cardsInHand = new List<Card> (GameConstants.CARDS_PER_PLAYER);

			if (gameType == GameType.Realtime || gameType == GameType.MultiplayerAI)
				InitGame ();
		}

		public void InitGame ()
		{
			SetPlayersInfo ();
			List<Card> deck = BuildDeck ();

			for (int i = 0; i < players.Length; i++) {
				List<Card> cards = GenerateCards (deck);
				Helper.SortCards (game, cards);

				SetCardsInfo (cards, i);
				players [i].Cards = cards;

				if (players [i].Type == UserType.Computer)
					players [i].AIFunc.Initialised (players [i].Cards, game);
				else if (players [i].Type == UserType.Host || players [i].Type == UserType.Player) {
					//tableActionChanged(TableAction.PlayerState, players[i]);
					tableActionChanged (new PlayerState (players [i]));
				}
			}

			//***********************
			// Testing Sequence Cards 
			//***********************
//			try {
//				players [2].Cards.Clear ();
//			
//				//players [2].Cards.Add (new Card (Suite.Clubs, Value.Three));
//				players [2].Cards.Add (new Card (Suite.Hearts, Value.Three));
//				//players [2].Cards.Add (new Card (Suite.Clubs, Value.Four));
//				//players [2].Cards.Add (new Card (Suite.Hearts, Value.Four));
//				//players [2].Cards.Add (new Card (Suite.Clubs, Value.Five));
//				players [2].Cards.Add (new Card (Suite.Hearts, Value.Five));
//				//				players [2].Cards.Add (new Card (Suite.Spades, Value.Five));
//				players [2].Cards.Add (new Card (Suite.Clubs, Value.Six));
//				//players [2].Cards.Add (new Card (Suite.Hearts, Value.Six));
//				players [2].Cards.Add (new Card (Suite.Clubs, Value.Seven));
//				//players [2].Cards.Add (new Card (Suite.Diamonds, Value.Seven));
//				players [2].Cards.Add (new Card (Suite.Hearts, Value.Eight));
//				//players [2].Cards.Add (new Card (Suite.Clubs, Value.Four));
//				//players [2].Cards.Add (new Card (Suite.Hearts, Value.Four));
//				//players [2].Cards.Add (new Card (Suite.Clubs, Value.Five));
//				players [2].Cards.Add (new Card (Suite.Hearts, Value.Jack));
//				//				players [2].Cards.Add (new Card (Suite.Spades, Value.Five));
//				players [2].Cards.Add (new Card (Suite.Clubs, Value.Queen));
//				//players [2].Cards.Add (new Card (Suite.Hearts, Value.Six));
//				players [2].Cards.Add (new Card (Suite.Hearts, Value.King));
//
//			
//				List<Card>[] crds = players [2].GetCardsList (game);
//				Debug.LogError ("Cards: \n" + JsonConvert.SerializeObject (crds));
//			
//				Debug.LogError ("Four Seq: " + players [2].HasSequence (4, crds, game, false));
//				Debug.LogError ("Five Seq: " + players [2].HasSequence (5, crds, game, false));
//				Debug.LogError ("Three Seq: " + players [2].HasSequence (3, crds, game, false));
//			
//				Debug.LogError ("GetRelativeSequence: " + JsonConvert.SerializeObject (players [2].GetRelativeSequence (4, crds, game, null, false)));
//				Debug.LogError ("GetRelativeSequence: " + JsonConvert.SerializeObject (players [2].GetRelativeSequence (3, crds, game, new Card (Suite.Spades, Value.King), false)));
//				Debug.LogError ("GetRelativeSequence: " + JsonConvert.SerializeObject (players [2].GetRelativeSequence (3, crds, game, new Card (Suite.Hearts, Value.Queen), false)));
//				Debug.LogError ("GetRelativeSequence: " + JsonConvert.SerializeObject (players [2].GetRelativeSequence (3, crds, game, null, false)));
//			
//				Debug.LogError ("Cards: \n" + JsonConvert.SerializeObject (crds));
//			} catch (Exception ex) {
//				Debug.LogError ("Error: " + ex.ToString ());
//			}
//			
//			return;
			//***********************
			// Testing Cards 
			//***********************


			//***********************
			// Testing Pair Sequence Cards 
			//***********************
//			try {
//				players [2].Cards.Clear ();
//
//				//players [2].Cards.Add (new Card (Suite.Clubs, Value.Three));
//				players [2].Cards.Add (new Card (Suite.Hearts, Value.Three));
//				players [2].Cards.Add (new Card (Suite.Clubs, Value.Four));
//				players [2].Cards.Add (new Card (Suite.Hearts, Value.Four));
//				players [2].Cards.Add (new Card (Suite.Clubs, Value.Five));
//				players [2].Cards.Add (new Card (Suite.Hearts, Value.Five));
////				players [2].Cards.Add (new Card (Suite.Spades, Value.Five));
//				players [2].Cards.Add (new Card (Suite.Clubs, Value.Six));
//				players [2].Cards.Add (new Card (Suite.Hearts, Value.Six));
//				players [2].Cards.Add (new Card (Suite.Clubs, Value.Seven));
//				players [2].Cards.Add (new Card (Suite.Diamonds, Value.Seven));
//
//				List<Card>[] crds = players [2].GetCardsList (game);
//				Debug.LogError ("Cards: \n" + JsonConvert.SerializeObject (crds));
//
//				Debug.LogError ("Four Pair Seq: " + players [2].HasPairSequence (4, crds, game, false));
//				Debug.LogError ("Five Pair Seq: " + players [2].HasPairSequence (5, crds, game, false));
//				Debug.LogError ("Three Pair Seq: " + players [2].HasPairSequence (3, crds, game, false));
//
//				Debug.LogError ("GetRelativePairSequence: " + JsonConvert.SerializeObject (players [2].GetRelativePairSequence (4, crds, game, null, false)));
//				Debug.LogError ("GetRelativePairSequence: " + JsonConvert.SerializeObject (players [2].GetRelativePairSequence (4, crds, game, new Card (Suite.Hearts, Value.Seven), true)));
//				Debug.LogError ("GetRelativePairSequence: " + JsonConvert.SerializeObject (players [2].GetRelativePairSequence (4, crds, game, new Card (Suite.Clubs, Value.Seven), true)));
//				Debug.LogError ("GetRelativePairSequence: " + JsonConvert.SerializeObject (players [2].GetRelativePairSequence (3, crds, game, null, false)));
//
//				Debug.LogError ("Cards: \n" + JsonConvert.SerializeObject (crds));
//			} catch (Exception ex) {
//				Debug.LogError ("Error: " + ex.ToString ());
//			}
//
//			return;
			//***********************
			// Testing Cards 
			//***********************


			//***********************
			// Testing Instant Wins 
			//***********************
//			while (players[0].GetCardsList(game)[GameConstants.CARDS_PER_PLAYER - 1].Count != 4)
//				players [0].GetCardsList (game) [GameConstants.CARDS_PER_PLAYER - 1].Add (new Card (Suite.Clubs, Value.Two));
			//***********************
			// Testing Instant Wins 
			//***********************

			bool instantWin = false;

			// For instant win, 9 points is received in addiontion to applying rule 2 to rule 6 on remain loosers.
			foreach (Player player in players) {

				List<Card>[] cardList = player.GetCardsList (game);

				if (player.HasPairSequence (6, cardList, game) || cardList [GameConstants.CARDS_PER_PLAYER - 1].Count == 4) {
					player.Cards.Clear ();

					player.Chips += 9 * (game as TienLenGame).GetGameInfo ().BuyIns;

					foreach (Player plyr in players) {
//						Debug.LogError ("Cards: " + JsonConvert.SerializeObject (plyr.Cards));
						if (plyr.Number != player.Number) {
							cardList = plyr.GetCardsList (game);

							int chips = (game as TienLenGame).GetGameInfo ().BuyIns;//GetRemainingCardsCurrency (plyr);
							plyr.Chips -= chips;

							player.Chips += chips;
						}
					}

					instantWin = true;
					break;
				}
			}

			if (instantWin) {
				tableActionChanged (new GameFinished (players));
				return;
			}

			if (playerturn == -1) {
				playerturn = game.GetFirstPlayerTurn ();
				//tableActionChanged(TableAction.PlayerTurn, playerturn);
				tableActionChanged (new PlayerTurn (playerturn, players [playerturn].ID));
				tableActionChanged (new PlayersStatus (players));
				//GameEvents.OnTurnChanged (players [playerturn]);
			}

			//GameServer.Instance.Phase = GamePhase.Initialzed;
			BeginGame ();
		}

		public void ProcessClientRequest (IActionMessage message)
		{

			if (message is SubmitCards) {
				List<Card> cards = ((SubmitCards)message).Cards;

				// *************************
				// Testing Cards Validation 
				// *************************

				//TestSingleCard ();

//				TestSequence ();

				//TestPairSequence ();

				// *************************
				// Testing Cards Validation 
				// *************************

				// TODO: Uncomment this line after Testing Cards Validation
				bool isValid = ParseField (cards);

				// TODO: Comment this line after testing
//				var isValid = false;

				//string responseString = string.Concat(isValid.ToString(), ServerConfigs.INNER_DATA_SEPERATOR, players[playerturn].Name);
				tableActionChanged (new DoSubmitCards (isValid, playerturn));

				if (isValid) {

					UpdateCurrency ();

					players [playerturn].CardsOnTable = cards;

					cardsInHand.Clear ();

					foreach (Card card in cards) {
						Card tmpCard = players [card.Player].Cards.Find (tempCard => tempCard.Suite == card.Suite && tempCard.Value == card.Value);
							
						cardsInHand.Add (tmpCard);
					}

					EndTurn (false);
				}
			} else if (message is SkipTurn) {
				EndTurn (true);
			} else if (message is GetGameState) {
				tableActionChanged (new  GetGameStateResponse (((GetGameState)message).PlayerId, playerturn, players));
			}
		}

		// If a player has an unused cards in his/her hand at the end of the game, penalty points is calculated by rule 2 to rule 6 and subtract from player's score .
		int GetRemainingCardsCurrency (Player player)
		{
			int chips = 0;
			List<Card>[] cardList = player.GetCardsList (game);

			// For every sequence of four pairs is killed,6 points is received.
			if (player.HasPairSequence (4, cardList, game, true))
				chips += 2 * (game as TienLenGame).GetGameInfo ().BuyIns;
//				chips += 6 * (game as TienLenGame).GetGameInfo ().BuyIns;
			//Debug.LogError ("Chips: " + chips);
			// For every a four of a kind is killed,5 points is received.
			while (player.IfFourOfKindExist(cardList,game,true))
				chips += 2 * (game as TienLenGame).GetGameInfo ().BuyIns;
				//chips += 5 * (game as TienLenGame).GetGameInfo ().BuyIns;
			//Debug.LogError ("Chips: " + chips);
			// For every sequence of three pairs is killed,3 points is received.
			while (player.HasPairSequence(3,cardList,game,true ))
				chips += 2 * (game as TienLenGame).GetGameInfo ().BuyIns;
				//chips += 3 * (game as TienLenGame).GetGameInfo ().BuyIns;
			//Debug.LogError ("Chips: " + chips);
			// For every black 2 is killed,2 points is received.
			// For every red 2 is killed,3 points is received.
			foreach (Card card in cardList[GameConstants.CARDS_PER_PLAYER-1])
				chips += (int)(((card.Suite == Suite.Hearts || card.Suite == Suite.Diamonds) ? 1.5 : 1) * ((game as TienLenGame).GetGameInfo ().BuyIns));
				//chips += ((card.Suite == Suite.Hearts || card.Suite == Suite.Diamonds) ? 3 : 2) * ((game as TienLenGame).GetGameInfo ().BuyIns);
			cardList [GameConstants.CARDS_PER_PLAYER - 1].Clear ();
			//Debug.LogError ("Chips: " + chips);
			return chips;
		}

		public void UpdateCurrency ()
		{
			if (cardsInHand.Count > 0) {
				//	For every black 2 is killed,2 points is received.
				//	For every red 2 is killed,3 points is received.
				if (cardsInHand [0].Value == Value.Two && ((game as TienLenGame).FieldType == FieldType.Bomb || (game as TienLenGame).FieldType == FieldType.FourKind)) {
					//cardsInHand[0].
					
					foreach (Card card in cardsInHand) {
						int Chips = (int)(((card.Suite == Suite.Hearts || card.Suite == Suite.Diamonds) ? 1.5 : 1) * ((game as TienLenGame).GetGameInfo ().BuyIns));
						//int Chips = (((card.Suite == Suite.Hearts || card.Suite == Suite.Diamonds) ? 3 : 2) * ((game as TienLenGame).GetGameInfo ().BuyIns));
						
						players [card.Player].Chips -= Chips;
						
						players [playerturn].Chips += Chips; 
					}
				}
				//	For every sequence of three pairs is killed,3 points is received.
				else if ((game as TienLenGame).FieldType == FieldType.Bomb && cardsInHand.Count == 6) {
					int Chips = 2 * ((game as TienLenGame).GetGameInfo ().BuyIns);
					//int Chips = 3 * ((game as TienLenGame).GetGameInfo ().BuyIns);
					
					players [cardsInHand [0].Player].Chips -= Chips;
					
					players [playerturn].Chips += Chips; 
				}
				//	For every sequence of four pairs is killed,6 points is received.
				else if ((game as TienLenGame).FieldType == FieldType.Bomb && cardsInHand.Count >= 8) {
					int Chips = 2 * ((game as TienLenGame).GetGameInfo ().BuyIns);
					//int Chips = 6 * ((game as TienLenGame).GetGameInfo ().BuyIns);
					
					players [cardsInHand [0].Player].Chips -= Chips;
					
					players [playerturn].Chips += Chips; 
				}
				//	For every a four of a kind is killed,5 points is received.
				else if ((game as TienLenGame).FieldType == FieldType.FourKind) {
					int Chips = 2 * ((game as TienLenGame).GetGameInfo ().BuyIns);
					//int Chips = 5 * ((game as TienLenGame).GetGameInfo ().BuyIns);
					
					players [cardsInHand [0].Player].Chips -= Chips;
					
					players [playerturn].Chips += Chips; 
				}

				tableActionChanged (new PlayersStatus (players));
			}
		}

		public void BeginGame ()
		{
//			Debug.Log ("BeginGame");
			ChangeTurn (true);
		}

		public void EndGame ()
		{
			// Adding Final Chips for the Positions
			foreach (Player player in players) {
				if (player.Results.Place > 4 || player.Results.Place <= 0)
					continue;
//				Logger.Log ("player Ended "+player.Name );

				// First place gets 3 points,2nd gets 2 points and 3rd gets 1 points.
				player.Chips += (4 - player.Results.Place) * (game as TienLenGame).GetGameInfo ().BuyIns;

				// If a player has 13 unused cards in his/her hand after 1st winner finish his/her game, 3 penalty points is added in addition to rule 7 and subtract from player's score.
				if (player.FirstWinPenalty)
					player.Chips -= (3 * (game as TienLenGame).GetGameInfo ().BuyIns);

				// If a player has an unused cards in his/her hand at the end of the game, penalty points is calculated by rule 2 to rule 6 and subtract from player's score .
				int chips = GetRemainingCardsCurrency (player);
				player.Chips -= chips;

				// The 3rd winner can received points when apply rule 7 on 4th place player.
//				if (player.Results.Place == 4) {
//					foreach (Player plyr in players) {
//						if (plyr.Results.Place == 3)
//							plyr.Chips += chips;
//					}
//				}
			}

//			Logger.Log ("Game Ended", LogType.Error);
//			Debug.LogError();
			tableActionChanged (new GameFinished (players));
		}

		public void RestartGame ()
		{
			ResetPlayerActionSprites ();
			RemoveAllPlayerSlotCards ();
			ResetHolderCards ();
			ResetPlayerCardsSlots ();

			cardsInHand.Clear ();
			//selectedCards.Clear();
			((TienLenGame)game).FieldType = FieldType.None;

			InitGame ();
		}

		public void ChangeTurn (bool starting)
		{
			if (!starting) {
				players [playerturn].FreeTurn = false;
//				Debug.LogError ("Here 4");

				if (players.Length - 1 == xplace) {
					var index = GetNextPlayersTurn ();
					if (index != -1)
						players [index].Results.Place = players.Length;
					else {
						foreach (Player player in players)
							if (player.Results.Place == 0)
								player.Results.Place = players.Length;
					}

					tableActionChanged (new PlayersStatus (players));


					//tableActionChanged (new PlayersStatus (players));

//					Debug.LogError ("GameEnded");
					EndGame ();
//					Debug.LogError ("GameEnded after");

					return;
				}

				playerturn = GetNextPlayersTurn ();
//				Debug.LogError ("Here 5");
//				if (players [playerturn].SkippedTurn) {
//					ChangeTurn (false);
//					return;
//				}
//				Debug.LogError ("Here 6");

				if ((allowFreeTurn && playerfield == playerturn) || playerturn == -1) {
					cardsInHand.Clear ();
					RemoveAllPlayerSlotCards ();
					ResetPlayerActionSprites ();

//					Debug.LogError ("Player Turn 1: " + playerturn);

					//Debug.LogError ("Here 7");
					for (int i = 0; i < players.Length; i++) {
						players [i].SkippedTurn = false;
					}

					if (playerfield != -1) {
						if (players [playerfield].Cards.Count != 0)
							playerturn = playerfield;
						else {
							for (int i = 1; i < players.Length; i++) {
								var tmp = (playerfield + i) % players.Length;
								if (players [tmp].Results.Place == 0) {
									playerturn = tmp;
									break;
								}
							}
						}
					} else {
						for (int i = 0; i < players.Length; i++) {
							if (players [i].Results.Place == 0) {
								playerturn = i;
								break;
							}
						}
					}

					(game as TienLenGame).FieldType = FieldType.None;
				}

//				Debug.LogError ("Player Turn 2: " + playerturn);
//				Debug.LogError ("Here 8");

				players [playerturn].FreeTurn = allowFreeTurn && playerfield == playerturn;
				//CoreClient.CallOnEvent(OnEventCallType.TurnChanged, players[playerturn - 1]);

				//players [playerturn].FreeTurn = allowFreeTurn && playerfield == playerturn;
				tableActionChanged (new PlayerTurn (playerturn, players [playerturn].ID));
				
				//tableActionChanged (new PlayersStatus (players));

				GameEvents.OnTurnChanged (players [playerturn]);
			}

//			Logger.Log (GetPlayerTurn ());
			SetPlayerActionSprite (ActionType.Turn);

			// For AI player
			if (players [playerturn].Type == UserType.Computer) {


//				// Test Case
//				cardsInHand = new List<Card> ();
//				List<Card> cards = new List<Card> ();
//				
//
//				(game as TienLenGame).FieldType = FieldType.Sequence;
//				cardsInHand = new List<Card> ();
//
//				cardsInHand.Add (new Card (Suite.Spades, Value.Five));
//				cardsInHand.Add (new Card (Suite.Spades, Value.Six));
//				cardsInHand.Add (new Card (Suite.Spades, Value.Seven));
//				
//				cards.Add (new Card (Suite.Hearts, Value.Four));
//				cards.Add (new Card (Suite.Clubs, Value.Five));
//				cards.Add (new Card (Suite.Clubs, Value.Six));
//				Debug.LogError ("***** MyDeck: " + JsonConvert.SerializeObject (cards));
//				Debug.LogError ("***** Table: " + JsonConvert.SerializeObject (cardsInHand));
//								
//
//				((TienLenGame)game).FieldType = FieldType.Sequence;
//				((TienLenAI)(players[playerturn].AIFunc)).Initialised(cards, game);

//				TestAISequence ();

//				Debug.LogError(((TienLenGame)game).FieldType+ " Cards in Hand : "+cardsInHand);
//				Debug.LogError("AI card list : "+JsonConvert.SerializeObject( ((TienLenAI)players [playerturn].AIFunc).cardList));
				List<Card> playcards = players [playerturn].AIFunc.Process (cardsInHand, game, players [playerturn]);
//				Debug.LogError ("***** Move : " + JsonConvert.SerializeObject (playcards));


//				Debug.LogError ("Here 9");
				if (playcards.Count == 0){}
//					Logger.Log (players [playerturn].Name + " has decided to skip the turn.");
				else {
					UpdateCurrency ();
					cardsInHand = playcards;

					players [playerturn].CardsOnTable.Clear ();
					foreach (Card card in playcards)
						players [playerturn].CardsOnTable.Add (card);
				}
//				Debug.LogError ("Here 10");
				// TODO next turn
				new Thread (new ThreadStart (() => {
					Thread.Sleep (2000);
					EndTurn (playcards.Count == 0);
				})).Start ();
			}
		}

		public void EndTurn (bool isSkipped)
		{
			bool finished = false;

			if (cardsInHand.Count != 0 && !isSkipped) {
				RemoveHolderCards ();

				foreach (Card card in cardsInHand) {
					players [card.Player].Cards.Remove (card);
				}

				playerfield = playerturn;
			}


			if (players [playerturn].CardsLeft == 0) {

//				var plyrs = players.Where (player => player.CardsLeft != 0);
//
//				if (plyrs != null && plyrs.Count () == 1) {
//					finished = true;
//					EndGame ();
//					
//					return;
//				}

				players [playerturn].Results.Place = ++xplace;

				if (players [playerturn].Results.Place == 1) {
					foreach (Player player in players) {
						if (player.CardsLeft == GameConstants.CARDS_PER_PLAYER)
							player.FirstWinPenalty = true;
					}
				}

				tableActionChanged (new PlayersStatus (players));

				// If a player has 13 unused cards in his/her hand after 1st winner finish his/her game, 3 penalty points is added in addition to rule 7 and subtract from player's score.
				if (xplace == 1) {
					foreach (Player player in players) {

					}
				}

//				Logger.Log (players [playerturn].Name + " has played all the cards!");

				//playerfield = GetNextPlayersTurn ();
				//finished = true;
			}

//			Debug.LogError ("Here 1");

			players [playerturn].Results.Turns++;
			players [playerturn].SkippedTurn = isSkipped;


//			Debug.LogError ("Here 2");

			if (isSkipped) {
				SetPlayerActionSprite (ActionType.Skipped);
				RemoveCurrentPlayerSlots ();
			} else {
				SetPlayerActionSprite (ActionType.TurnCompleted);
			}

//			Debug.LogError ("Here 3");

//			// TODO: Implement Play till last player.
//			while (players[(playerturn+1)%players.Length].CardsLeft == 0)
//				playerturn = (playerturn + 1) % players.Length;

			ChangeTurn (false);
			GameEvents.OnTurnEnded (players [playerturn], finished);
		}

		public List<Card> BuildDeck ()
		{

			List<Card> deck = new List<Card> ();

			for (int i = 1; i <= GameConstants.SUIT_PER_GAME; i++)
				for (int j = 1; j <= GameConstants.CARDS_PER_PLAYER; j++)
					deck.Add (new Card ((Suite)i, (Value)j));

			return deck;
		}

		public List<Card> GenerateCards (List<Card> deck)
		{
			System.Random rand = new System.Random (DateTime.Now.Millisecond);
			int number = 0;
			List<Card> cards = new List<Card> (GameConstants.CARDS_PER_PLAYER);
			
			for (int i = 0; i < GameConstants.CARDS_PER_PLAYER; i++) {
				number = rand.Next (0, deck.Count);
				Card card = deck [number];
				cards.Add (card);
				deck.Remove (card);
			}
			
			return cards;
		}

		private void SetPlayerActionSprite (ActionType type)
		{
			// TODO set the player of 'playerturn' action sprite (pass, turn etc)
			tableActionChanged (new PlayerAction (type));
		}

		private void ResetPlayerActionSprites ()
		{
			// TODO reset/set invisible all player action sprites (pass, turn etc)
			tableActionChanged (new ResetAllPlayerActions ());
		}

		private void RemoveAllPlayerSlotCards ()
		{
			foreach (Player player in players)
				player.CardsOnTable.Clear ();

			// TODO remove/set invisible the cards in Slots (slots where cards show up on move)
			tableActionChanged (new RemoveAllCardsOnTable ());
		}

		private void RemoveCurrentPlayerSlots ()
		{
			players [playerturn].CardsOnTable.Clear ();
			// TODO remove/set invisible the cards of current player 'i.e. playerturn' in Slots (slots where cards show up on move)
			tableActionChanged (new RemovePlayerCardsOnTable ());
		}

		private void ResetHolderCards ()
		{
			// TODO reset/set visible the cards of each players (cards shows up against each player
			tableActionChanged (new ResetAllPlayerCards ());
		}

		private void RemoveHolderCards ()
		{
			RemoveCurrentPlayerSlots ();

			// TODO remove the 'cardsInHand' of 'playerturn' from its cards collection and show on table
			string cardNames = string.Empty;

			foreach (Card card in cardsInHand) {
				players [playerturn].CardsOnTable.Add (card);
				cardNames = string.Concat (cardNames, ClientConfigs.INNER_DATA_SEPERATOR, Helper.GetCardPath (card));
			}

			cardNames = cardNames.TrimStart (ClientConfigs.INNER_DATA_SEPERATOR);
			cardNames = cardNames.TrimEnd (ClientConfigs.INNER_DATA_SEPERATOR);

			tableActionChanged (new RemovePlayerSelectedCards (cardNames.Split (ClientConfigs.INNER_DATA_SEPERATOR)));
		}

		public void ResetPlayerCardsSlots ()
		{
			// TODO reset the cards of player of its own to 'covers', shown below
			tableActionChanged (new ResetPlayerCards ());
		}

		public bool ParseField (List<Card> cards)
		{
			cards.Sort (delegate(Card c1, Card c2) {
				if (Helper.GetGCV (game, c1.Value).CompareTo (Helper.GetGCV (game, c2.Value)) == 0)
					return c1.Suite.CompareTo (c2.Suite);
				else
					return Helper.GetGCV (game, c1.Value).CompareTo (Helper.GetGCV (game, c2.Value));
			});

			return game.ValidateCards (cards, cardsInHand);
		}

		public int GetNextPlayersTurn ()
		{
			//return ++playerturn % players.Length;
			for (int i = 1; i < players.Length; i++) {
				var tempUse = (playerturn + i) % players.Length;
				
				if (players [tempUse].Results.Place == 0 && !players [tempUse].SkippedTurn)
					return tempUse;
			}

			return -1;
		}

		public string GetPlayerTurn ()
		{
			return "It is " + (playerturn == 0 ? "your" : players [playerturn].Name + "'s") +
				(players [playerturn].FreeTurn ? " free" : null) + " turn.";
		}

		public void SetCardsInfo (List<Card> cards, int player)
		{
			for (int i = 0; i < cards.Count; i++) {
				cards [i].Player = player;
				cards [i].CardOrder = i + 1; // TODO look in to
			}
		}

		private void SetPlayersInfo ()
		{
			// TODO set the names of 'players'
			string playerNames = string.Empty;

			foreach (Player player in players) 
				playerNames = string.Concat (playerNames, ClientConfigs.INNER_DATA_SEPERATOR, player.Name);

			playerNames = playerNames.TrimStart (ClientConfigs.INNER_DATA_SEPERATOR);
			playerNames = playerNames.TrimEnd (ClientConfigs.INNER_DATA_SEPERATOR);

			tableActionChanged (new PlayerNames (playerNames.Split (ClientConfigs.INNER_DATA_SEPERATOR)));
		}

		public Action<IActionMessage> TableActionChanged {
			set {
				tableActionChanged = value;
			}
			get {
				return tableActionChanged;
			}
		}

		//************************
		// Logic Validation Tests
		//************************
		#region LOGIC_VALIDATION_TESTS
		void TestSingleCard ()
		{
			(game as TienLenGame).FieldType = FieldType.Single;
			cardsInHand = new List<Card> ();
			List<Card> cards = new List<Card> ();
			
			cardsInHand.Add (new Card (Suite.Spades, Value.Three));

			cards.Add (new Card (Suite.Diamonds, Value.Three));
			
			bool isValid = ParseField (cards);

//			Debug.LogError ("Old: " + JsonConvert.SerializeObject (cardsInHand));
//			Debug.LogError ("New: " + JsonConvert.SerializeObject (cards));
//			Debug.LogError ("Accepted: " + isValid);


			cardsInHand = new List<Card> ();
			cards = new List<Card> ();
			
			cardsInHand.Add (new Card (Suite.Diamonds, Value.Three));
			
			cards.Add (new Card (Suite.Spades, Value.Three));
			
			isValid = ParseField (cards);
//			Debug.LogError ("Old: " + JsonConvert.SerializeObject (cardsInHand));
//			Debug.LogError ("New: " + JsonConvert.SerializeObject (cards));
//			Debug.LogError ("Accepted: " + isValid);


			cardsInHand = new List<Card> ();
			cards = new List<Card> ();
			
			cardsInHand.Add (new Card (Suite.Diamonds, Value.Three));
			
			cards.Add (new Card (Suite.Spades, Value.Two));
			
			isValid = ParseField (cards);
//			Debug.LogError ("Old: " + JsonConvert.SerializeObject (cardsInHand));
//			Debug.LogError ("New: " + JsonConvert.SerializeObject (cards));
//			Debug.LogError ("Accepted: " + isValid);


			cardsInHand = new List<Card> ();
			cards = new List<Card> ();
			
			cardsInHand.Add (new Card (Suite.Diamonds, Value.Two));
			
			cards.Add (new Card (Suite.Spades, Value.Three));
			
			isValid = ParseField (cards);
//			Debug.LogError ("Old: " + JsonConvert.SerializeObject (cardsInHand));
//			Debug.LogError ("New: " + JsonConvert.SerializeObject (cards));
//			Debug.LogError ("Accepted: " + isValid);
		}

		void TestSequence ()
		{
			(game as TienLenGame).FieldType = FieldType.None;
			cardsInHand = new List<Card> ();
			List<Card> cards = new List<Card> ();
			
			cards.Add (new Card (Suite.Hearts, Value.Five));
			cards.Add (new Card (Suite.Hearts, Value.Six));
			cards.Add (new Card (Suite.Spades, Value.Seven));
//			cards.Add (new Card (Suite.Hearts, Value.Six));
			
			bool isValid = ParseField (cards);
			
//			Debug.LogError ("Old: " + JsonConvert.SerializeObject (cardsInHand));
//			Debug.LogError ("New: " + JsonConvert.SerializeObject (cards));
//			Debug.LogError ("Accepted: " + isValid);



			(game as TienLenGame).FieldType = FieldType.Sequence;
			cardsInHand = new List<Card> ();
			cards = new List<Card> ();
			
			cardsInHand.Add (new Card (Suite.Hearts, Value.Five));
			cardsInHand.Add (new Card (Suite.Hearts, Value.Six));
			cardsInHand.Add (new Card (Suite.Spades, Value.Seven));
			
			cards.Add (new Card (Suite.Clubs, Value.Four));
			cards.Add (new Card (Suite.Spades, Value.Five));
			cards.Add (new Card (Suite.Diamonds, Value.Six));
			
			isValid = ParseField (cards);
			
//			Debug.LogError ("Old: " + JsonConvert.SerializeObject (cardsInHand));
//			Debug.LogError ("New: " + JsonConvert.SerializeObject (cards));
//			Debug.LogError ("Accepted: " + isValid);

//
//
//
//			(game as TienLenGame).FieldType = FieldType.None;
//			cardsInHand = new List<Card> ();
//			cards = new List<Card> ();
//			
//			cards.Add (new Card (Suite.Clubs, Value.Two));
//			cards.Add (new Card (Suite.Clubs, Value.Three));
//			cards.Add (new Card (Suite.Clubs, Value.Four));
//			cards.Add (new Card (Suite.Hearts, Value.Five));
//			
//			isValid = ParseField (cards);
//			
//			Debug.LogError ("Old: " + JsonConvert.SerializeObject (cardsInHand));
//			Debug.LogError ("New: " + JsonConvert.SerializeObject (cards));
//			Debug.LogError ("Accepted: " + isValid);
//
//
//
//			(game as TienLenGame).FieldType = FieldType.None;
//			cardsInHand = new List<Card> ();
//			cards = new List<Card> ();
//
//			cards.Add (new Card (Suite.Clubs, Value.Three));
//			cards.Add (new Card (Suite.Clubs, Value.Four));
//			cards.Add (new Card (Suite.Hearts, Value.Five));
//			cards.Add (new Card (Suite.Clubs, Value.Seven));
//
//			isValid = ParseField (cards);
//			
//			Debug.LogError ("Old: " + JsonConvert.SerializeObject (cardsInHand));
//			Debug.LogError ("New: " + JsonConvert.SerializeObject (cards));
//			Debug.LogError ("Accepted: " + isValid);
//
//
//
//			(game as TienLenGame).FieldType = FieldType.FourKind;
//			cardsInHand = new List<Card> ();
//			cards = new List<Card> ();
//			
//			cardsInHand.Add (new Card (Suite.Hearts, Value.Three));
//			cardsInHand.Add (new Card (Suite.Clubs, Value.Three));
//			cardsInHand.Add (new Card (Suite.Diamonds, Value.Three));
//			cardsInHand.Add (new Card (Suite.Spades, Value.Three));
//			
//			cards.Add (new Card (Suite.Clubs, Value.Three));
//			cards.Add (new Card (Suite.Clubs, Value.Four));
//			cards.Add (new Card (Suite.Clubs, Value.Five));
//			cards.Add (new Card (Suite.Hearts, Value.Six));
//			
//			isValid = ParseField (cards);
//			
//			Debug.LogError ("Old: " + JsonConvert.SerializeObject (cardsInHand));
//			Debug.LogError ("New: " + JsonConvert.SerializeObject (cards));
//			Debug.LogError ("Accepted: " + isValid);
//
//
//
//			(game as TienLenGame).FieldType = FieldType.FourKind;
//			cardsInHand = new List<Card> ();
//			cards = new List<Card> ();
//			
//			cardsInHand.Add (new Card (Suite.Hearts, Value.Three));
//			cardsInHand.Add (new Card (Suite.Clubs, Value.Three));
//			cardsInHand.Add (new Card (Suite.Diamonds, Value.Three));
//			cardsInHand.Add (new Card (Suite.Spades, Value.Three));
//			
//			cards.Add (new Card (Suite.Clubs, Value.Three));
//			cards.Add (new Card (Suite.Clubs, Value.Four));
//			cards.Add (new Card (Suite.Clubs, Value.Five));
//			cards.Add (new Card (Suite.Hearts, Value.Seven));
//			
//			isValid = ParseField (cards);
//			
//			Debug.LogError ("Old: " + JsonConvert.SerializeObject (cardsInHand));
//			Debug.LogError ("New: " + JsonConvert.SerializeObject (cards));
//			Debug.LogError ("Accepted: " + isValid);
//
//
//
//			(game as TienLenGame).FieldType = FieldType.Sequence;
//			cardsInHand = new List<Card> ();
//			cards = new List<Card> ();
//			
//			cardsInHand.Add (new Card (Suite.Hearts, Value.Three));
//			cardsInHand.Add (new Card (Suite.Clubs, Value.Four));
//			cardsInHand.Add (new Card (Suite.Hearts, Value.Five));
//			cardsInHand.Add (new Card (Suite.Spades, Value.Six));
//			
//			cards.Add (new Card (Suite.Clubs, Value.Three));
//			cards.Add (new Card (Suite.Clubs, Value.Four));
//			cards.Add (new Card (Suite.Clubs, Value.Five));
//			cards.Add (new Card (Suite.Hearts, Value.Six));
//			
//			isValid = ParseField (cards);
//			
//			Debug.LogError ("Old: " + JsonConvert.SerializeObject (cardsInHand));
//			Debug.LogError ("New: " + JsonConvert.SerializeObject (cards));
//			Debug.LogError ("Accepted: " + isValid);
//			
//			
//			cardsInHand = new List<Card> ();
//			cards = new List<Card> ();
//			
//			cardsInHand.Add (new Card (Suite.Clubs, Value.Three));
//			cardsInHand.Add (new Card (Suite.Clubs, Value.Four));
//			cardsInHand.Add (new Card (Suite.Hearts, Value.Five));
//			cardsInHand.Add (new Card (Suite.Hearts, Value.Six));
//			
//			cards.Add (new Card (Suite.Hearts, Value.Three));
//			cards.Add (new Card (Suite.Clubs, Value.Four));
//			cards.Add (new Card (Suite.Clubs, Value.Five));
//			cards.Add (new Card (Suite.Clubs, Value.Six));
//			
//			isValid = ParseField (cards);
//			Debug.LogError ("Old: " + JsonConvert.SerializeObject (cardsInHand));
//			Debug.LogError ("New: " + JsonConvert.SerializeObject (cards));
//			Debug.LogError ("Accepted: " + isValid);
//			
//			
//			cardsInHand = new List<Card> ();
//			cards = new List<Card> ();
//			
//			cardsInHand.Add (new Card (Suite.Hearts, Value.Three));
//			cardsInHand.Add (new Card (Suite.Clubs, Value.Four));
//			cardsInHand.Add (new Card (Suite.Hearts, Value.Five));
//			cardsInHand.Add (new Card (Suite.Hearts, Value.Six));
//
//			cards.Add (new Card (Suite.Clubs, Value.Four));
//			cards.Add (new Card (Suite.Clubs, Value.Five));
//			cards.Add (new Card (Suite.Hearts, Value.Six));
//			cards.Add (new Card (Suite.Clubs, Value.Seven));
//
//			isValid = ParseField (cards);
//			Debug.LogError ("Old: " + JsonConvert.SerializeObject (cardsInHand));
//			Debug.LogError ("New: " + JsonConvert.SerializeObject (cards));
//			Debug.LogError ("Accepted: " + isValid);
//			
//			
//			cardsInHand = new List<Card> ();
//			cards = new List<Card> ();
//			
//			cardsInHand.Add (new Card (Suite.Hearts, Value.Four));
//			cardsInHand.Add (new Card (Suite.Clubs, Value.Five));
//			cardsInHand.Add (new Card (Suite.Hearts, Value.Six));
//			cardsInHand.Add (new Card (Suite.Hearts, Value.Seven));
//			
//			cards.Add (new Card (Suite.Clubs, Value.Three));
//			cards.Add (new Card (Suite.Clubs, Value.Four));
//			cards.Add (new Card (Suite.Hearts, Value.Five));
//			cards.Add (new Card (Suite.Clubs, Value.Six));
//			
//			isValid = ParseField (cards);
//			Debug.LogError ("Old: " + JsonConvert.SerializeObject (cardsInHand));
//			Debug.LogError ("New: " + JsonConvert.SerializeObject (cards));
//			Debug.LogError ("Accepted: " + isValid);
//
//
//			cardsInHand = new List<Card> ();
//			cards = new List<Card> ();
//			
//			cardsInHand.Add (new Card (Suite.Hearts, Value.Four));
//			cardsInHand.Add (new Card (Suite.Clubs, Value.Five));
//			cardsInHand.Add (new Card (Suite.Hearts, Value.Six));
//			cardsInHand.Add (new Card (Suite.Hearts, Value.Seven));
//			
//			cards.Add (new Card (Suite.Clubs, Value.Three));
//			cards.Add (new Card (Suite.Clubs, Value.Four));
//			cards.Add (new Card (Suite.Hearts, Value.Five));
//			cards.Add (new Card (Suite.Clubs, Value.Six));
//			cards.Add (new Card (Suite.Clubs, Value.Seven));
//
//			isValid = ParseField (cards);
//			Debug.LogError ("Old: " + JsonConvert.SerializeObject (cardsInHand));
//			Debug.LogError ("New: " + JsonConvert.SerializeObject (cards));
//			Debug.LogError ("Accepted: " + isValid);
//
//
//			cardsInHand = new List<Card> ();
//			cards = new List<Card> ();
//			
//			cardsInHand.Add (new Card (Suite.Hearts, Value.Three));
//			cardsInHand.Add (new Card (Suite.Clubs, Value.Four));
//			cardsInHand.Add (new Card (Suite.Hearts, Value.Five));
//			cardsInHand.Add (new Card (Suite.Hearts, Value.Six));
//			
//			cards.Add (new Card (Suite.Clubs, Value.Five));
//			cards.Add (new Card (Suite.Clubs, Value.Six));
//			cards.Add (new Card (Suite.Hearts, Value.Seven));
//			cards.Add (new Card (Suite.Clubs, Value.Nine));
//			
//			isValid = ParseField (cards);
//			Debug.LogError ("Old: " + JsonConvert.SerializeObject (cardsInHand));
//			Debug.LogError ("New: " + JsonConvert.SerializeObject (cards));
//			Debug.LogError ("Accepted: " + isValid);
		}


		void TestPairSequence ()
		{
			(game as TienLenGame).FieldType = FieldType.None;
			cardsInHand = new List<Card> ();
			List<Card> cards = new List<Card> ();
			
			cards.Add (new Card (Suite.Clubs, Value.Three));
			cards.Add (new Card (Suite.Hearts, Value.Three));
			cards.Add (new Card (Suite.Clubs, Value.Four));
			cards.Add (new Card (Suite.Hearts, Value.Four));
			cards.Add (new Card (Suite.Clubs, Value.Five));
			cards.Add (new Card (Suite.Hearts, Value.Five));
			
			bool isValid = ParseField (cards);
			
//			Debug.LogError ("Old: " + JsonConvert.SerializeObject (cardsInHand));
//			Debug.LogError ("New: " + JsonConvert.SerializeObject (cards));
//			Debug.LogError ("Accepted: " + isValid);


			(game as TienLenGame).FieldType = FieldType.None;
			cardsInHand = new List<Card> ();
			cards = new List<Card> ();

			cards.Add (new Card (Suite.Clubs, Value.Three));
			cards.Add (new Card (Suite.Hearts, Value.Three));
			cards.Add (new Card (Suite.Clubs, Value.Four));
			cards.Add (new Card (Suite.Hearts, Value.Four));
			cards.Add (new Card (Suite.Clubs, Value.Five));
			cards.Add (new Card (Suite.Hearts, Value.Five));
			cards.Add (new Card (Suite.Hearts, Value.Six));
			
			isValid = ParseField (cards);
			
//			Debug.LogError ("Old: " + JsonConvert.SerializeObject (cardsInHand));
//			Debug.LogError ("New: " + JsonConvert.SerializeObject (cards));
//			Debug.LogError ("Accepted: " + isValid);

			
			
			(game as TienLenGame).FieldType = FieldType.None;
			cardsInHand = new List<Card> ();
			cards = new List<Card> ();
			
			cards.Add (new Card (Suite.Clubs, Value.Two));
			cards.Add (new Card (Suite.Hearts, Value.Two));
			cards.Add (new Card (Suite.Clubs, Value.Three));
			cards.Add (new Card (Suite.Hearts, Value.Three));
			cards.Add (new Card (Suite.Clubs, Value.Five));
			cards.Add (new Card (Suite.Hearts, Value.Five));
			
			isValid = ParseField (cards);
			
//			Debug.LogError ("Old: " + JsonConvert.SerializeObject (cardsInHand));
//			Debug.LogError ("New: " + JsonConvert.SerializeObject (cards));
//			Debug.LogError ("Accepted: " + isValid);
			
			
			
			(game as TienLenGame).FieldType = FieldType.None;
			cardsInHand = new List<Card> ();
			cards = new List<Card> ();
			
			cards.Add (new Card (Suite.Clubs, Value.Two));
			cards.Add (new Card (Suite.Hearts, Value.Two));
			cards.Add (new Card (Suite.Clubs, Value.Three));
			cards.Add (new Card (Suite.Hearts, Value.Three));
			cards.Add (new Card (Suite.Clubs, Value.Five));
			cards.Add (new Card (Suite.Hearts, Value.Five));
			cards.Add (new Card (Suite.Clubs, Value.Six));
			
			isValid = ParseField (cards);
			
//			Debug.LogError ("Old: " + JsonConvert.SerializeObject (cardsInHand));
//			Debug.LogError ("New: " + JsonConvert.SerializeObject (cards));
//			Debug.LogError ("Accepted: " + isValid);
			
			
			
			(game as TienLenGame).FieldType = FieldType.FourKind;
			cardsInHand = new List<Card> ();
			cards = new List<Card> ();
			
			cardsInHand.Add (new Card (Suite.Hearts, Value.Three));
			cardsInHand.Add (new Card (Suite.Clubs, Value.Three));
			cardsInHand.Add (new Card (Suite.Diamonds, Value.Three));
			cardsInHand.Add (new Card (Suite.Spades, Value.Three));
			
			cards.Add (new Card (Suite.Clubs, Value.Three));
			cards.Add (new Card (Suite.Hearts, Value.Three));
			cards.Add (new Card (Suite.Clubs, Value.Four));
			cards.Add (new Card (Suite.Hearts, Value.Four));
			cards.Add (new Card (Suite.Clubs, Value.Five));
			cards.Add (new Card (Suite.Hearts, Value.Five));
			
			isValid = ParseField (cards);
			
//			Debug.LogError ("Old: " + JsonConvert.SerializeObject (cardsInHand));
//			Debug.LogError ("New: " + JsonConvert.SerializeObject (cards));
//			Debug.LogError ("Accepted: " + isValid);
			
			
			
			(game as TienLenGame).FieldType = FieldType.FourKind;
			cardsInHand = new List<Card> ();
			cards = new List<Card> ();
			
			cardsInHand.Add (new Card (Suite.Hearts, Value.Three));
			cardsInHand.Add (new Card (Suite.Clubs, Value.Three));
			cardsInHand.Add (new Card (Suite.Diamonds, Value.Three));
			cardsInHand.Add (new Card (Suite.Spades, Value.Three));
			
			cards.Add (new Card (Suite.Clubs, Value.Three));
			cards.Add (new Card (Suite.Hearts, Value.Three));
			cards.Add (new Card (Suite.Clubs, Value.Four));
			cards.Add (new Card (Suite.Hearts, Value.Four));
			cards.Add (new Card (Suite.Clubs, Value.Five));
			cards.Add (new Card (Suite.Hearts, Value.Five));
			cards.Add (new Card (Suite.Clubs, Value.Six));
			cards.Add (new Card (Suite.Hearts, Value.Six));
			
			isValid = ParseField (cards);
			
//			Debug.LogError ("Old: " + JsonConvert.SerializeObject (cardsInHand));
//			Debug.LogError ("New: " + JsonConvert.SerializeObject (cards));
//			Debug.LogError ("Accepted: " + isValid);
			



			(game as TienLenGame).FieldType = FieldType.Double;
			cardsInHand = new List<Card> ();
			cards = new List<Card> ();
			
			cardsInHand.Add (new Card (Suite.Hearts, Value.Two));
			cardsInHand.Add (new Card (Suite.Clubs, Value.Two));

			cards.Add (new Card (Suite.Clubs, Value.Three));
			cards.Add (new Card (Suite.Hearts, Value.Three));
			cards.Add (new Card (Suite.Clubs, Value.Four));
			cards.Add (new Card (Suite.Hearts, Value.Four));
			cards.Add (new Card (Suite.Clubs, Value.Five));
			cards.Add (new Card (Suite.Hearts, Value.Five));
			cards.Add (new Card (Suite.Clubs, Value.Six));
			cards.Add (new Card (Suite.Hearts, Value.Six));
			
			isValid = ParseField (cards);
			
//			Debug.LogError ("Old: " + JsonConvert.SerializeObject (cardsInHand));
//			Debug.LogError ("New: " + JsonConvert.SerializeObject (cards));
//			Debug.LogError ("Accepted: " + isValid);



			(game as TienLenGame).FieldType = FieldType.Single;
			cardsInHand = new List<Card> ();
			cards = new List<Card> ();
			
			cardsInHand.Add (new Card (Suite.Hearts, Value.Two));
			
			cards.Add (new Card (Suite.Clubs, Value.Three));
			cards.Add (new Card (Suite.Hearts, Value.Three));
			cards.Add (new Card (Suite.Clubs, Value.Four));
			cards.Add (new Card (Suite.Hearts, Value.Four));
			cards.Add (new Card (Suite.Clubs, Value.Five));
			cards.Add (new Card (Suite.Hearts, Value.Five));
			cards.Add (new Card (Suite.Clubs, Value.Six));
			cards.Add (new Card (Suite.Hearts, Value.Six));
			
			isValid = ParseField (cards);
			
//			Debug.LogError ("Old: " + JsonConvert.SerializeObject (cardsInHand));
//			Debug.LogError ("New: " + JsonConvert.SerializeObject (cards));
//			Debug.LogError ("Accepted: " + isValid);


			(game as TienLenGame).FieldType = FieldType.Single;
			cardsInHand = new List<Card> ();
			cards = new List<Card> ();
			
			cardsInHand.Add (new Card (Suite.Hearts, Value.Two));
			
			cards.Add (new Card (Suite.Clubs, Value.Three));
			cards.Add (new Card (Suite.Hearts, Value.Three));
			cards.Add (new Card (Suite.Clubs, Value.Four));
			cards.Add (new Card (Suite.Hearts, Value.Four));
			cards.Add (new Card (Suite.Clubs, Value.Five));
			cards.Add (new Card (Suite.Hearts, Value.Five));
			
			isValid = ParseField (cards);
			
//			Debug.LogError ("Old: " + JsonConvert.SerializeObject (cardsInHand));
//			Debug.LogError ("New: " + JsonConvert.SerializeObject (cards));
//			Debug.LogError ("Accepted: " + isValid);

			
			(game as TienLenGame).FieldType = FieldType.Bomb;
			cardsInHand = new List<Card> ();
			cards = new List<Card> ();
			
			cardsInHand.Add (new Card (Suite.Hearts, Value.Three));
			cardsInHand.Add (new Card (Suite.Hearts, Value.Three));
			cardsInHand.Add (new Card (Suite.Clubs, Value.Four));
			cardsInHand.Add (new Card (Suite.Hearts, Value.Four));
			cardsInHand.Add (new Card (Suite.Clubs, Value.Five));
			cardsInHand.Add (new Card (Suite.Clubs, Value.Five));
			
			cards.Add (new Card (Suite.Clubs, Value.Three));
			cards.Add (new Card (Suite.Hearts, Value.Three));
			cards.Add (new Card (Suite.Clubs, Value.Four));
			cards.Add (new Card (Suite.Hearts, Value.Four));
			cards.Add (new Card (Suite.Clubs, Value.Five));
			cards.Add (new Card (Suite.Hearts, Value.Five));
			
			isValid = ParseField (cards);
			
//			Debug.LogError ("Old: " + JsonConvert.SerializeObject (cardsInHand));
//			Debug.LogError ("New: " + JsonConvert.SerializeObject (cards));
//			Debug.LogError ("Accepted: " + isValid);
			
			
			cardsInHand = new List<Card> ();
			cards = new List<Card> ();
			
			cardsInHand.Add (new Card (Suite.Hearts, Value.Three));
			cardsInHand.Add (new Card (Suite.Hearts, Value.Three));
			cardsInHand.Add (new Card (Suite.Clubs, Value.Four));
			cardsInHand.Add (new Card (Suite.Hearts, Value.Four));
			cardsInHand.Add (new Card (Suite.Clubs, Value.Five));
			cardsInHand.Add (new Card (Suite.Hearts, Value.Five));
			
			cards.Add (new Card (Suite.Clubs, Value.Three));
			cards.Add (new Card (Suite.Hearts, Value.Three));
			cards.Add (new Card (Suite.Clubs, Value.Four));
			cards.Add (new Card (Suite.Hearts, Value.Four));
			cards.Add (new Card (Suite.Clubs, Value.Five));
			cards.Add (new Card (Suite.Clubs, Value.Five));
			
			isValid = ParseField (cards);
//			Debug.LogError ("Old: " + JsonConvert.SerializeObject (cardsInHand));
//			Debug.LogError ("New: " + JsonConvert.SerializeObject (cards));
//			Debug.LogError ("Accepted: " + isValid);
			
			
			cardsInHand = new List<Card> ();
			cards = new List<Card> ();
			
			cardsInHand.Add (new Card (Suite.Hearts, Value.Three));
			cardsInHand.Add (new Card (Suite.Hearts, Value.Three));
			cardsInHand.Add (new Card (Suite.Clubs, Value.Four));
			cardsInHand.Add (new Card (Suite.Hearts, Value.Four));
			cardsInHand.Add (new Card (Suite.Clubs, Value.Five));
			cardsInHand.Add (new Card (Suite.Hearts, Value.Five));

			cards.Add (new Card (Suite.Clubs, Value.Four));
			cards.Add (new Card (Suite.Hearts, Value.Four));
			cards.Add (new Card (Suite.Clubs, Value.Five));
			cards.Add (new Card (Suite.Hearts, Value.Five));
			cards.Add (new Card (Suite.Hearts, Value.Six));
			cards.Add (new Card (Suite.Hearts, Value.Six));
			
			isValid = ParseField (cards);
//			Debug.LogError ("Old: " + JsonConvert.SerializeObject (cardsInHand));
//			Debug.LogError ("New: " + JsonConvert.SerializeObject (cards));
//			Debug.LogError ("Accepted: " + isValid);
			
			
			cardsInHand = new List<Card> ();
			cards = new List<Card> ();

			cardsInHand.Add (new Card (Suite.Clubs, Value.Four));
			cardsInHand.Add (new Card (Suite.Hearts, Value.Four));
			cardsInHand.Add (new Card (Suite.Clubs, Value.Five));
			cardsInHand.Add (new Card (Suite.Hearts, Value.Five));
			cardsInHand.Add (new Card (Suite.Clubs, Value.Six));
			cardsInHand.Add (new Card (Suite.Hearts, Value.Six));
			
			cards.Add (new Card (Suite.Clubs, Value.Three));
			cards.Add (new Card (Suite.Hearts, Value.Three));
			cards.Add (new Card (Suite.Clubs, Value.Four));
			cards.Add (new Card (Suite.Hearts, Value.Four));
			cards.Add (new Card (Suite.Hearts, Value.Five));
			cards.Add (new Card (Suite.Clubs, Value.Five));
			
			isValid = ParseField (cards);
//			Debug.LogError ("Old: " + JsonConvert.SerializeObject (cardsInHand));
//			Debug.LogError ("New: " + JsonConvert.SerializeObject (cards));
//			Debug.LogError ("Accepted: " + isValid);
			
			
			cardsInHand = new List<Card> ();
			cards = new List<Card> ();
			
			cardsInHand.Add (new Card (Suite.Hearts, Value.Three));
			cardsInHand.Add (new Card (Suite.Hearts, Value.Three));
			cardsInHand.Add (new Card (Suite.Clubs, Value.Four));
			cardsInHand.Add (new Card (Suite.Hearts, Value.Four));
			cardsInHand.Add (new Card (Suite.Clubs, Value.Five));
			cardsInHand.Add (new Card (Suite.Hearts, Value.Five));
			
			cards.Add (new Card (Suite.Clubs, Value.Four));
			cards.Add (new Card (Suite.Hearts, Value.Four));
			cards.Add (new Card (Suite.Clubs, Value.Five));
			cards.Add (new Card (Suite.Hearts, Value.Five));
			cards.Add (new Card (Suite.Hearts, Value.Six));
			cards.Add (new Card (Suite.Hearts, Value.Six));
			cards.Add (new Card (Suite.Hearts, Value.Seven));
			cards.Add (new Card (Suite.Hearts, Value.Seven));
			
			isValid = ParseField (cards);
//			Debug.LogError ("Old: " + JsonConvert.SerializeObject (cardsInHand));
//			Debug.LogError ("New: " + JsonConvert.SerializeObject (cards));
//			Debug.LogError ("Accepted: " + isValid);
			
			
			cardsInHand = new List<Card> ();
			cards = new List<Card> ();
			
			cardsInHand.Add (new Card (Suite.Hearts, Value.Three));
			cardsInHand.Add (new Card (Suite.Hearts, Value.Three));
			cardsInHand.Add (new Card (Suite.Clubs, Value.Four));
			cardsInHand.Add (new Card (Suite.Hearts, Value.Four));
			cardsInHand.Add (new Card (Suite.Clubs, Value.Five));
			cardsInHand.Add (new Card (Suite.Hearts, Value.Five));
			
			cards.Add (new Card (Suite.Clubs, Value.Four));
			cards.Add (new Card (Suite.Hearts, Value.Four));
			cards.Add (new Card (Suite.Clubs, Value.Five));
			cards.Add (new Card (Suite.Hearts, Value.Five));
			cards.Add (new Card (Suite.Hearts, Value.Seven));
			cards.Add (new Card (Suite.Hearts, Value.Seven));
			
			isValid = ParseField (cards);
//			Debug.LogError ("Old: " + JsonConvert.SerializeObject (cardsInHand));
//			Debug.LogError ("New: " + JsonConvert.SerializeObject (cards));
//			Debug.LogError ("Accepted: " + isValid);
		}
		#endregion


		#region Events

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

		#endregion
	}
}
