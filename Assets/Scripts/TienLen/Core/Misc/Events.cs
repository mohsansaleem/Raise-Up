using System;
using TienLen.Core.Models;
using TienLen.Utils.Enums;

namespace TienLen.Core.Misc
{
	#region Classes inheriting EventArgs
	
	public class PlayerEventArgs : EventArgs
	{
		private Player player;
		
		public PlayerEventArgs(Player turn)
		{
			player = turn;
		}
		
		/// <summary>
		/// The player who triggered this event.
		/// </summary>
		public Player Player
		{
			get { return player; }
		}
	}
	
	public class TurnEndedEventArgs : EventArgs
	{
		private Player player;
		private bool finished;
		
		public TurnEndedEventArgs(Player turn, bool finished)
		{
			player = turn;
			this.finished = finished;
		}
		
		public Player Player
		{
			get { return player; }
		}
		
		/// <summary>
		/// If the player has finished on this turn.
		/// </summary>
		/// <remarks>This does not mean the player has finished. It is only true if the player has finished ON that turn.</remarks>
		public bool Finished
		{
			get { return finished; }
		}
	}
	
	public class PhaseChangedEventArgs : EventArgs
	{
		private Player player;
		private GamePhase phase;
		
		public PhaseChangedEventArgs(Player turn, GamePhase phase)
		{
			player = turn;
			this.phase = phase;
		}
		
		/// <summary>
		/// The new game phase.
		/// </summary>
		public GamePhase NewPhase
		{
			get { return phase; }
		}
		
		public Player Player
		{
			get { return player; }
		}
	}
	
	public class ClientPropChangeArgs : EventArgs
	{
		private ClientPropChange changeType;
		private object info;
		
		public ClientPropChangeArgs(ClientPropChange changeType, object info)
		{
			this.info = info;
			this.changeType = changeType;
		}
		
		public ClientPropChange PropChangeType
		{
			get { return changeType; }
		}
		
		public object Info
		{
			get { return info; }
		}
	}

	#endregion
	
	public static class GameEvents
	{
		/// <summary>
		/// Called when the game phase changes.
		/// </summary>
		public static event EventHandler<PhaseChangedEventArgs> GamePhaseChanged;
		/// <summary>
		/// Called when one's cards have successfully been prepared, after shuffling.
		/// </summary>
		public static event EventHandler<PlayerEventArgs> PlayerCardsReady;
		
		/// <summary>
		/// Called when the game is ready to play.
		/// </summary>
		/// <remarks>If the game is on MSN, this also means if the MSN initialisation has been complete.</remarks>
		public static event EventHandler GameReady;
		
		/// <summary>
		/// Called when the player's current turn has changed.
		/// </summary>
		public static event EventHandler<PlayerEventArgs> TurnChanged;
		/// <summary>
		/// Called when the player's turn has ended.
		/// </summary>
		public static event EventHandler<TurnEndedEventArgs> TurnEnded;
		
		//public static event EventHandler<ClientFuncEventArgs> ClientFuncCalled;
		public static event EventHandler<ClientPropChangeArgs> ClientPropChangeCalled;
		
		#region OnEvents
		
		internal static void OnTurnChanged(Player player)
		{
			if (TurnChanged != null)
				TurnChanged(null, new PlayerEventArgs(player));
		}
		
		internal static void OnClientPropChangeCalled(ClientPropChange changeType, object info)
		{
			ClientPropChangeCalled(null, new ClientPropChangeArgs(changeType, info));
		}
		
		internal static void OnTurnEnded(Player player, bool finished)
		{
			if (TurnEnded != null)
				TurnEnded(null, new TurnEndedEventArgs(player, finished));
		}
		
		internal static void OnGamePhaseChanged(Player turn, GamePhase phase)
		{
			if (GamePhaseChanged != null)
				GamePhaseChanged(null, new PhaseChangedEventArgs(turn, phase));
		}
		
		internal static void OnPlayerCardsReady(Player turn)
		{
			if (PlayerCardsReady != null)
				PlayerCardsReady(null, new PlayerEventArgs(turn));
		}
		
		internal static void OnGameReady()
		{
			if (GameReady != null)
				GameReady(null, EventArgs.Empty);
		}
		
		#endregion
	}
}