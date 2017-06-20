using System.Collections.Generic;
using TienLen.Core.Models;
using TienLen.Utils.Enums;
using TienLen.Utils.Constants;
using System;

namespace TienLen.Utils.Helpers
{
	public static class Helper
	{
		/// <summary>
		/// Get the integer value of the card, relying on the highest and lowest card of the game.
		/// </summary>
		/// <param name="game">The game to use, for comparing the card's value.</param>
		/// <param name="value">The value to parse.</param>
		public static int GetGCV (IGame game, Value value)
		{
			if (value == game.LowestCard ().Value)
				return 1;
			
			if (value > game.LowestCard ().Value)
				return (int)value - (int)game.LowestCard ().Value + 1;
			else
				return GameConstants.CARDS_PER_PLAYER - ((int)game.HighestCard ().Value - (int)value);
		}

		public static int GetGSV (Card card)
		{
			if (card.Suite == Suite.Spades)
				return 1;
			else if (card.Suite == Suite.Clubs)
				return 3;
			else if (card.Suite == Suite.Diamonds)
				return 6;
			else if (card.Suite == Suite.Hearts)
				return 10;
			return 0;
		}

		public static void SortCards (IGame game, List<Card> cards)
		{
			cards.Sort (delegate(Card c1, Card c2) {
				if (Helper.GetGCV (game, c1.Value).CompareTo (Helper.GetGCV (game, c2.Value)) == 0)
					return c1.Suite.CompareTo (c2.Suite);
				else
					return Helper.GetGCV (game, c1.Value).CompareTo (Helper.GetGCV (game, c2.Value));
			});
		}

		/// <summary>
		/// Get an integer value of the sum of the cards suites, with a card list.
		/// </summary>
		public static int GetSuiteSum (List<Card> cardList)
		{
			int sum = 0;
			
			for (int i = 0; i < cardList.Count; i++) {
				if (cardList [i].Suite == Suite.Spades)
					sum += 1;
				else if (cardList [i].Suite == Suite.Clubs)
					sum += 3;
				else if (cardList [i].Suite == Suite.Diamonds)
					sum += 6;
				else if (cardList [i].Suite == Suite.Hearts)
					sum += 10;
			}
			
			return sum;
		}
		
		/// <summary>
		/// Get an integer value of the sum of the cards suites, with an array of cards.
		/// </summary>
		public static int GetSuiteSum (Card[] cardList)
		{
			return GetSuiteSum (new List<Card> (cardList));
		}

		/// <summary>
		/// Returns the card path with respect to card model.
		/// </summary>
		/// <returns>The card path.</returns>
		/// <param name="card">Card.</param>
		public static string GetCardPath (Card card)
		{
			string cardName = string.Empty;

			if ((int)card.Value > 1 && (int)card.Value < 11)
				cardName = ((int)card.Value).ToString ();
			else
				cardName = card.Value.ToString ();

			cardName = cardName + "_" + card.Suite.ToString ();
			;

			return cardName;
		}

		public static string GenerateName ()
		{
			Random ran = new Random ();

			return GameConfig.PLAYER_NAMES [ran.Next (0, GameConfig.PLAYER_NAMES.Length)];
		}
	}
}