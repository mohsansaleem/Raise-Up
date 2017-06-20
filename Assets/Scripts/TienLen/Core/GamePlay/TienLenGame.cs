using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TienLen.Core.Models;
using TienLen.Utils.Enums;
using TienLen.Utils.Constants;
using TienLen.Core.Misc;
using System;
using TienLen.Utils.Helpers;
using Newtonsoft.Json;

namespace TienLen.Core.GamePlay
{
	public class TienLenGame : IGame
	{
		public bool IsLowestCardDone = false;
		private Card lowestCard;
		private Card highestCard;
		private GameInfo gameInfo;

		bool started = false;
		bool playerRecentFinished = false;
		int countBeat = 0;
		int playersFinished = 0;

		private FieldType fType = FieldType.None;

		public FieldType FieldType {
			get { return fType; }
			set { fType = value; }
		}

		#region IGame implementation

		public bool ValidateCards (List<Card> chosenCards, List<Card> currentCards)
		{
//			Debug.LogError ("chosenCards: " + JsonConvert.SerializeObject (chosenCards));
//			Debug.LogError ("currentCards: " + JsonConvert.SerializeObject (currentCards));
//			if (IsLowestCardDone == false &&
//				chosenCards.Find (delegate(Card card) { 
//				return card.Value == lowestCard.Value && card.Suite == lowestCard.Suite; 
//			}) == null)
//				return false;

			// If player has a four of a kind, it can be used to chop a single 2.
			if (fType == FieldType.Single && currentCards.Count == 1 && currentCards [0].Value == Value.Two && chosenCards.Count == 4 && EqualCardValues (chosenCards)) {
				//if (fType == FieldType.Single && currentCards.Count == 1 && currentCards [0].Value == Value.Two && chosenCards.Count == 4 && EqualCardValues (chosenCards)){
				fType = FieldType.FourKind;

				Debug.LogError ("Chop 2 with 4 of a kind.");

				goto RETTRUE;
			}
			
			#region Sequence / Bomb
			
			// Check if combo is sequence or a bomb
			if ((fType == FieldType.None && chosenCards.Count > 2) || 
				((fType == FieldType.Sequence || fType == FieldType.Bomb) && chosenCards.Count == currentCards.Count) ||
				(currentCards.Count > 0 && EqualCardValues (currentCards) && currentCards [0].Value == Value.Two)
				|| (fType == FieldType.FourKind && chosenCards.Count == 8)) {
				bool accept = true;
				bool ischop = chosenCards.Count >= 6 && chosenCards [1].Value - chosenCards [0].Value == 0;
				int increment = ischop ? 2 : 1;
				
				// Ensure the last card of the bomb / sequence isn't a 2, and the End of the bomb / sequence value is greater
				// than the current one.
				// Chosen Cards and Current Cards are same. Chosen card don't have 2 in the end.
				if (chosenCards.Count == currentCards.Count && (chosenCards [chosenCards.Count - 1].Value == Value.Two || 
				// Last card of Chosen cards should be greater then current cards
					(Helper.GetGCV (this, chosenCards [chosenCards.Count - 1].Value) < Helper.GetGCV (this, currentCards [chosenCards.Count - 1].Value)) || 
				// If Last card of Chosen cards is same in value then Cuite should be greater then current cards
					((chosenCards [chosenCards.Count - 1].Value == currentCards [chosenCards.Count - 1].Value) && (chosenCards [chosenCards.Count - 1].Suite < currentCards [chosenCards.Count - 1].Suite))))
					accept = false;
				
				for (int i = ischop ? 3 : 1; i < chosenCards.Count; i += increment) {
					if (!accept)
						break;
					
					// Verify if the chop or sequence is valid
					if (ischop)
						accept = chosenCards.Count >= 6 && ((chosenCards [i].Value - chosenCards [i - 1].Value) == 0) && ((Helper.GetGCV (this, chosenCards [i - 1].Value) - Helper.GetGCV (this, chosenCards [i - 2].Value)) == 1) && ((chosenCards [i - 2].Value - chosenCards [i - 3].Value) == 0);
					else
						accept = Helper.GetGCV (this, chosenCards [i].Value) - Helper.GetGCV (this, chosenCards [i - 1].Value) == 1;
				}

				if (ischop && chosenCards.Count % 2 != 0)
					accept = false;

				// If the card is a 2, the combo must be a chop and depending on the quantity of 2's,
				// the number of pairs in the sequence must be powerful enough to beat it.
				// (eg. 1 card of 2 = 3 pairs, 2 cards of 2 = 4 pairs etc...)
				if (currentCards.Count != 0 && currentCards [0].Value == Value.Two && ischop && accept)
					accept = ((4 + currentCards.Count * 2) == chosenCards.Count);

				// If it is a sequence and previos cards are two or 4 of Kind then do not accept
				if (!ischop && currentCards.Count > 0 && (currentCards [0].Value == Value.Two || EqualCardValues (currentCards)))
					accept = false;

				if (accept == true) {

					fType = (ischop ? FieldType.Bomb : FieldType.Sequence);

					Debug.LogError ("Chopping " + (ischop ? FieldType.Bomb : FieldType.Sequence).ToString ());

					goto RETTRUE;
				}
			}
			
			#endregion
			
			// Number of players cards must equal to the number of cards on the field, unless
			// the field is empty.
			if (currentCards.Count != 0 && chosenCards.Count != currentCards.Count)
				return false;
			
			// Check if the players selected cards can beat the cards on the field
			switch (chosenCards.Count) {
			case 1:
				{
					if ((fType == FieldType.None) || Helper.GetGCV (this, chosenCards [0].Value) > Helper.GetGCV (this, currentCards [0].Value) || 
						(Helper.GetGCV (this, chosenCards [0].Value) == Helper.GetGCV (this, currentCards [0].Value) &&
						chosenCards [0].Suite > currentCards [0].Suite)) {
						fType = FieldType.Single;
//						Debug.LogError ("Single Same value and Bigger Suit ");

						goto RETTRUE;
					} else {
						return false;
					}
				
				}
			case 2:
			case 3:
			case 4:
				{
					if (EqualCardValues (chosenCards)) {
						if (fType == FieldType.None ||
							(Helper.GetSuiteSum (chosenCards) > Helper.GetSuiteSum (currentCards) && Helper.GetGCV (this, chosenCards [0].Value) == Helper.GetGCV (this, currentCards [0].Value)) 
							|| Helper.GetGCV (this, chosenCards [0].Value) > Helper.GetGCV (this, currentCards [0].Value)) {
							
//							Debug.LogError ("End Cases");
//							Debug.LogError ("Sum: " + Helper.GetSuiteSum (currentCards) + ", Sum: " + Helper.GetSuiteSum (chosenCards));
//							if (fType != FieldType.None)
//								Debug.LogError ("Chossen Cards GCV: " + Helper.GetGCV (this, chosenCards [0].Value) + ", currentCards: " + Helper.GetGCV (this, currentCards [0].Value));
							
							fType = (FieldType)chosenCards.Count;	

							goto RETTRUE;
						} else
							return false;
					}
				
					break;
				}
			}
			
			return false;
			
			RETTRUE:
			IsLowestCardDone = true;
			return true;
		}

		public void Initialised (GameInfo info)
		{
			bool started = false;
			bool playerRecentFinished = false;
			int countBeat = 0;
			int playersFinished = 0;

			gameInfo = info;
			lowestCard = new Card (Suite.Spades, Value.Three);
			highestCard = new Card (Suite.Hearts, Value.Two);

			IsLowestCardDone = false;

			GameEvents.TurnChanged += new EventHandler<PlayerEventArgs> (OnTurnChanged);
			GameEvents.TurnEnded += new EventHandler<TurnEndedEventArgs> (OnTurnEnded);
		}

		public GameInfo GetGameInfo ()
		{
			return gameInfo;
		}

		public int MaxPlayers ()
		{
			return GameConfig.MAX_PLAYERS;
		}

		public int MinPlayers ()
		{
			return GameConfig.MIN_PLAYERS;
		}

		public Card LowestCard ()
		{
			return lowestCard;
			//return new Card (Suite.Spades, Value.Three);
		}

		public Card HighestCard ()
		{
			return highestCard;
		}

		public bool IsLowestCard (Card card)
		{
			return card.Value == lowestCard.Value && card.Suite == lowestCard.Suite;
		}

		public int GetFirstPlayerTurn ()
		{
			int playerNo = -1;
			foreach (Player player in gameInfo.Players) {
				foreach (Card card in player.Cards) {
					if (IsLowestCard (card)) {
						playerNo = player.Number - 1;
						break;
					}
				}

				if (playerNo != -1)
					break;
			}

			return playerNo;
		}

		#endregion

		#region Events


		private void OnTurnChanged (object sender, PlayerEventArgs e)
		{
			if (e.Player.Number == 0)
				GameEvents.OnClientPropChangeCalled (ClientPropChange.Skippable, !e.Player.FreeTurn);
			
			if (e.Player.FreeTurn)
				fType = FieldType.None;
			
			if (!started) {
				started = true;
				//lowestCard = null;
			}


//			if (e.Player.Number == 1)
//				GameEvents.OnClientPropChangeCalled (ClientPropChange.Skippable, !e.Player.FreeTurn);
//			
//			if (e.Player.FreeTurn)
//				fType = FieldType.None;
		}

		private void OnTurnEnded (object sender, TurnEndedEventArgs e)
		{
			if (e.Finished) {
				GameEvents.OnClientPropChangeCalled (ClientPropChange.AllowFreeTurn, false);
				
				playerRecentFinished = true;
				countBeat = playersFinished;
				playersFinished++;
			} else if (playerRecentFinished) {
				if (!e.Player.SkippedTurn || countBeat == gameInfo.Players.Length) {
					GameEvents.OnClientPropChangeCalled (ClientPropChange.AllowFreeTurn, true);
					
					playerRecentFinished = false;
					countBeat = 0;
				} else
					countBeat++;
			}


//			if (e.Finished) {
//				GameEvents.OnClientPropChangeCalled (ClientPropChange.AllowFreeTurn, false);
//				
//				//playerRecentFinished = true;
//				//countBeat = playersFinished;
//				//playersFinished++;
//			} else {// if (playerRecentFinished)
//				if (!e.Player.SkippedTurn)
//					GameEvents.OnClientPropChangeCalled (ClientPropChange.AllowFreeTurn, true);
//			}
		}
		#endregion

		private bool EqualCardValues (List<Card> list)
		{
			Value chkValue = list [0].Value;
			
			for (int i = 1; i < list.Count; i++) {
				if (list [i].Value != chkValue)
					return false;
			}
			
			return true;
		}
	}
}
