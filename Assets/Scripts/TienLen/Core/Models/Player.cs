using System.Collections.Generic;
using TienLen.Utils.Enums;
using TienLen.Utils.Constants;
using TienLen.Utils.Helpers;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using TienLen.Core.GamePlay;

namespace TienLen.Core.Models
{
	public class Player
	{
		#region Fields
		
		public  UserType Type;
		public  string Name;
		public  int Number;
		public  IComputer AIFunc;
		public  string ID;
		public  Results Results;
		public  List<Card> Cards;
		public  List<Card> CardsOnTable;
		public  bool FreeTurn;
		public  bool SkippedTurn;
		public  int Chips;
		public bool FirstWinPenalty = false;
		#endregion

		public Player ()
		{
			CardsOnTable = new List<Card> ();

		}

//		[JsonConstructor]
//		public Player (UserType Type, int Number, List<Card> Cards, bool FreeTurn, bool SkippedTurn, string ID, IComputer AIFunc, string Name, Results Results)
//		{
//
//		}

		public Player (string name, UserType type, int playernum, string ID)
		{
			this.Name = name;
			Type = type;
			Number = playernum;
			this.ID = ID;
			Results = new Results ();

			Chips = 0;

			CardsOnTable = new List<Card> ();
			//cards = new List<Card>(GameConstants.CARDS_PER_PLAYER);

		}

		
		/// <summary>
		/// The number of cards the player has left.
		/// </summary>

		public int CardsLeft {
			get { 
				return Cards != null ? Cards.Count : 0; 
			
			}
		}

		
		/// <summary>
		/// [Online] Get if the player has left the game.
		/// </summary>
		/// <remarks>This is the shortcut to getting if the player has left, instead of using <see cref="Player.Type"/>.</remarks>
		public bool LeftGame {
			get { return Type == UserType.Leaver; }
		}

		/// <summary>
		/// Gets the cards list.
		/// </summary>
		/// <returns>The cards list.</returns>
		/// <param name="game">Game.</param>
		public List<Card>[] GetCardsList (IGame game)
		{
			if (this.Type == UserType.Computer)
				return (AIFunc as TienLenAI).cardList;

			List<Card>[] cardsList = new List<Card>[GameConstants.CARDS_PER_PLAYER];

			for (int i = 0; i < GameConstants.CARDS_PER_PLAYER; i++) {
				int amount = Cards.FindAll (delegate(Card card) {
					return Helper.GetGCV (game, card.Value) == Helper.GetGCV (game, (Value)i + 1);
				}).Count;
				cardsList [i] = new List<Card> (amount);
			}
		
			foreach (Card card in Cards)
				cardsList [Helper.GetGCV (game, card.Value) - 1].Add (card);
		
			for (int i = 0; i < GameConstants.CARDS_PER_PLAYER; i++) {
				if (cardsList [i].Count != 0)
					Helper.SortCards (game, cardsList [i]);
			}

			return cardsList;
		}

		public List<Card> GetRelativePairSequence (int num, List<Card>[] cardsList, IGame game, Card prevHighestCard, bool remove = false)
		{
			List<Card> retCards = new List<Card> ();
			Debug.LogError("Here");
			List<Card>[] arr;
			if(prevHighestCard != null)
				Debug.LogError ("prevHighestCard: " + prevHighestCard.Value.ToString ());
			int prevIndex = prevHighestCard != null ? Helper.GetGCV (game, prevHighestCard.Value) - 1 : -1;
			int prevSuitVal = -1;
			Debug.LogError ("prevIndex: " + prevIndex);
			if (prevHighestCard != null && cardsList [prevIndex].Count > 1)
				prevSuitVal = Helper.GetGSV (cardsList [prevIndex] [cardsList [prevIndex].Count - 1]);

			if (prevHighestCard != null && Helper.GetGSV (prevHighestCard) < prevSuitVal)
				prevIndex -= (num);
//			else if (prevHighestCard != null)// && prevSuitVal != -1)
//				prevIndex -= (num - 1);
			else if(prevHighestCard == null)
				prevIndex = -1;
//			else if (prevHighestCard != null && prevSuitVal != -1)
//				prevIndex -= (num - 1);
//			else
//				prevIndex = -1;

			Debug.LogError ("prevIndex: " + prevIndex);
			arr = cardsList.Where (cards => (cards.Count > 1 && Helper.GetGCV (game, cards [0].Value) > prevIndex)).ToArray ();
			Debug.LogError ("Arr Relative: " + JsonConvert.SerializeObject (arr));

			if (arr.Length < num)
				return retCards;

			int index = IfCardsListSequenceExist (num, arr, game);

			if (index != -1) {
				if (arr [index + num - 1] [0].Value == Value.Two)
					return retCards;
				int tmp;
				for (int i = 0; i < num; i++) {
					tmp = Helper.GetGCV (game, arr [index + i] [0].Value) - 1;

					retCards.Add (cardsList [tmp] [0]);
					if (remove)
						cardsList [tmp].RemoveAt (0);

					if (prevHighestCard != null && i == (num - 1) && cardsList [tmp] [0].Value == prevHighestCard.Value) {
						foreach (Card card in cardsList [tmp]) {
							if (Helper.GetGSV (card) > Helper.GetGSV (prevHighestCard)) {
								retCards.Add (card);
								if (remove)
									cardsList [tmp].Remove (card);
								break;
							}
						}
					} else {
						retCards.Add (cardsList [tmp] [0]);
						if (remove)
							cardsList [tmp].RemoveAt (0);
					}
				}
			}

			return retCards;
		}

		/// <summary>
		/// Determines whether this instance has pair sequence the specified num cardsList game remove.
		/// </summary>
		/// <returns><c>true</c> if this instance has pair sequence the specified num cardsList game remove; otherwise, <c>false</c>.</returns>
		/// <param name="num">Number.</param>
		/// <param name="cardsList">Cards list.</param>
		/// <param name="game">Game.</param>
		/// <param name="remove">If set to <c>true</c> remove.</param>
		public bool HasPairSequence (int num, List<Card>[] cardsList, IGame game, bool remove = false)
		{
			var arr = cardsList.Where (cards => cards.Count > 1).ToArray ();
			if (arr.Length < num)
				return false;

			int index = IfCardsListSequenceExist (num, arr, game);

			//Debug.LogError ("Arr After: " + JsonConvert.SerializeObject (arr));

			if (index != -1) {
				if (arr [index + num - 1] [0].Value == Value.Two)
					return false;
				if (remove) {
					int tmp;
					for (int i = 0; i < num; i++) {
						tmp = Helper.GetGCV (game, arr [index + i] [0].Value) - 1;

						Debug.LogError (cardsList [tmp] [0].Value.ToString ());
						cardsList [tmp].RemoveAt (0);
						Debug.LogError (cardsList [tmp] [0].Value.ToString ());
						cardsList [tmp].RemoveAt (0);
					}
				}
				return true;
			}

			return false;
		}
		
		public List<Card> GetRelativeSequence (int num, List<Card>[] cardsList, IGame game, Card prevHighestCard, bool remove = false)
		{
			List<Card> retCards = new List<Card> ();
			
			List<Card>[] arr;
			Debug.LogError ("prevHighestCard: " + prevHighestCard.Value.ToString ());
			int prevIndex = prevHighestCard != null ? Helper.GetGCV (game, prevHighestCard.Value) - 1 : -1;
			int prevSuitVal = -1;
			Debug.LogError ("prevIndex: " + prevIndex);
			if (prevHighestCard != null && cardsList [prevIndex].Count >= 1)
				prevSuitVal = Helper.GetGSV (cardsList [prevIndex] [cardsList [prevIndex].Count - 1]);

			if (prevHighestCard != null && Helper.GetGSV (prevHighestCard) < prevSuitVal)
				prevIndex -= (num);
//			else if (prevHighestCard != null)// && prevSuitVal != -1)
//				prevIndex -= (num - 1);
			else if(prevHighestCard == null)
				prevIndex = -1;
			Debug.LogError ("prevIndex: " + prevIndex);
			arr = cardsList.Where (cards => (cards.Count > 0 && Helper.GetGCV (game, cards [0].Value) > prevIndex)).ToArray ();
			Debug.LogError ("Arr Relative: " + JsonConvert.SerializeObject (arr));
			
			if (arr.Length < num)
				return retCards;
			
			int index = IfCardsListSequenceExist (num, arr, game);
			
			if (index != -1) {
				if (arr [index + num - 1] [0].Value == Value.Two)
					return retCards;
				int tmp;
				for (int i = 0; i < num; i++) {
					tmp = Helper.GetGCV (game, arr [index + i] [0].Value) - 1;

					//Debug.LogError ("Tmp: " + tmp + ": " + arr [index + i] [0].Value.ToString () + ", Index: " + index + ", i: " + i);
					if (prevHighestCard != null && i == (num - 1) && cardsList [tmp] [0].Value == prevHighestCard.Value) {
						foreach (Card card in cardsList [tmp]) {
							if (Helper.GetGSV (card) > Helper.GetGSV (prevHighestCard)) {
								retCards.Add (card);
								if (remove)
									cardsList [tmp].Remove (card);
								break;
							}
						}
					} else {
						retCards.Add (cardsList [tmp] [0]);
						if (remove)
							cardsList [tmp].RemoveAt (0);
					}
				}
			}
			
			return retCards;
		}
		
		/// <summary>
		/// Determines whether this instance has pair sequence the specified num cardsList game remove.
		/// </summary>
		/// <returns><c>true</c> if this instance has pair sequence the specified num cardsList game remove; otherwise, <c>false</c>.</returns>
		/// <param name="num">Number.</param>
		/// <param name="cardsList">Cards list.</param>
		/// <param name="game">Game.</param>
		/// <param name="remove">If set to <c>true</c> remove.</param>
		public bool HasSequence (int num, List<Card>[] cardsList, IGame game, bool remove = false)
		{
			var arr = cardsList.Where (cards => cards.Count == 1).ToArray ();
			if (arr.Length < num)
				return false;
			
			int index = IfCardsListSequenceExist (num, arr, game);
			//Debug.LogError ("Seq Index: " + index);
			//Debug.LogError ("Arr After: " + JsonConvert.SerializeObject (arr));
			
			if (index != -1) {
				if (arr [index + num - 1] [0].Value == Value.Two)
					return false;
				if (remove) {
					int tmp;
					for (int i = 0; i < num; i++) {
						tmp = Helper.GetGCV (game, arr [index + i] [0].Value) - 1;
						cardsList [tmp].RemoveAt (0);
					}
				}
				return true;
			}
            
			return false;
		}
        
		/// <summary>
		/// Ifs the pair sequence exist.
		/// </summary>
		/// <returns>The Index of the Sequence start or -1 if doesn't exist.</returns>
		/// <param name="num">Number.</param>
		/// <param name="arr">Pass the arr of pairs+</param>
		/// <param name="game">Game.</param>3 5 6 7 8 J Q K
		private int IfCardsListSequenceExist (int num, List<Card>[] arr, IGame game)
		{
			for (int i = 1, r = 1,startIndex = 0, count = 1; i < arr.Length; i++) {
				if (Helper.GetGCV (game, arr [i] [0].Value) - Helper.GetGCV (game, arr [i - 1] [0].Value) == 1)
					count ++;
				else {
					count = 1;
					i = r++;
					startIndex = i;
				}
				if (count == num)
					return startIndex;
			}
			//			Debug.LogError ("Count: " + count);

			return -1;
		}

		public bool IfFourOfKindExist (List<Card>[] cardsList, IGame game, bool remove = false)
		{
			foreach (var cardList in cardsList) {
				if (cardList.Count == 4) {
					if (remove) {
						cardsList [Helper.GetGCV (game, cardList [0].Value)].Clear ();
					}
					return true;
				}
			}
			return false;
		}

		public void Copy (Player copyFrom, Player copyTo)
		{
			copyTo.Cards = copyFrom.Cards; 
			copyTo.Name = copyFrom.Name; 
			copyTo.Number = copyFrom.Number;
			copyTo.Type = copyFrom.Type;
		}
	}
}


