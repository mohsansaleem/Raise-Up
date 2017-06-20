using System.Collections;
using TienLen.Utils.Enums;

namespace TienLen.Core.Models
{
	public class GameInfo
	{
		private Player[] players;
		private GameType gameType;

		private int buyIns;
		public int BuyIns {
			get {
				return buyIns;
			}
			
			set {
				buyIns = value;
			}
		}
		
		/// <summary>
		/// Array of players in this game.
		/// </summary>
		public Player[] Players {
			get { return players; }
		}
		
		/// <summary>
		/// The main player.
		/// </summary>
		public Player MainPlayer {
			get {
				foreach (Player player in players) {
					if (player.Type == UserType.Host)
						return player;
				}
				return null;
			}
		}
		
		/// <summary>
		/// The type of game the player is on.
		/// </summary>
		public GameType GameType {
			get { return gameType; }
		}
		
		public GameInfo (Player[] players, GameType svrType, int buyIns)
		{
			this.players = players;
			this.gameType = svrType;

			this.BuyIns = buyIns;
		}
	}
}