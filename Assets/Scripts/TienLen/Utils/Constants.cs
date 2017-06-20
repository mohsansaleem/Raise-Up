using UnityEngine;
using System.Collections;

namespace TienLen.Utils.Constants
{
	public static class GameConstants
	{

		public const string CARDS_PATH = "Textures/Deck/";
		public const string MISC_PATH = "Textures/Misc/";
		public const string COVER_CARD = "Cover";
		public const string PASSED_CARD = "Passed";
		public const string TURN_CARD = "highlight3";

		public const byte CARDS_PER_PLAYER = 13;
		public const byte SUIT_PER_GAME = 4;
	}

	public static class GameConfig
	{

		public const byte MAX_PLAYERS = 4;
		public const byte MIN_PLAYERS = 1;

		public const float SORT_DELAY = 0.1f;

		public const int TURN_TIME = 20;

		public static readonly string[] PLAYER_NAMES = new string[] {
			"Lenny", "Carl", "Arnold", "Saffron", "Colin", "Davis", "Folly", "Sherald", "Dennis", "Lion", "Derrick", "Lan"
		};
	}
}
