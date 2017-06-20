
namespace TienLen.Utils.Enums
{
	/// <summary>
	/// The cards suite.
	/// </summary>
	public enum Suite
	{
		Spades = 1,
		Clubs,
		Diamonds,
		Hearts
	}

	/// <summary>
	/// Card face or number value.
	/// </summary>
	public enum Value
	{
		/// <summary>
		/// Ace = 1
		/// </summary>
		Ace = 1,
		/// <summary>
		/// Two = 2
		/// </summary>
		Two,
		/// <summary>
		/// Three = 3
		/// </summary>
		Three,
		/// <summary>
		/// Four = 4
		/// </summary>
		Four,
		/// <summary>
		/// Five = 5
		/// </summary>
		Five,
		/// <summary>
		/// Six = 6
		/// </summary>
		Six,
		/// <summary>
		/// Seven = 7
		/// </summary>
		Seven,
		/// <summary>
		/// Eight = 8
		/// </summary>
		Eight,
		/// <summary>
		/// Nine = 9
		/// </summary>
		Nine,
		/// <summary>
		/// Ten = 10
		/// </summary>
		Ten,
		/// <summary>
		/// Jack = 11
		/// </summary>
		Jack,
		/// <summary>
		/// Queen = 12
		/// </summary>
		Queen,
		/// <summary>
		/// King = 13
		/// </summary>
		King
	}

	public enum FieldType
	{
		None,
		/// <summary>
		/// The single.
		/// </summary>
		Single,
		Double,
		Triple,
		FourKind,
		Sequence,
		Bomb
	}

	/// <summary>
	/// Enumerations of the possible client property game changes.
	/// </summary>
	public enum ClientPropChange
	{
		/// <summary>
		/// The current players turn. The second parameter needs to be a integer value.
		/// </summary>
		PlayerTurn,
		/// <summary>
		/// If the player can skip his/her turn. The second parameter needs to be a boolean value.
		/// </summary>
		Skippable,
		/// <summary>
		/// If the cards should be cleared if it is someone's free turn. The second parameter needs to be a boolean value.
		/// </summary>
		AllowFreeTurn,
		/// <summary>
		/// If changing turns should be clockwise. The second paramterer needs to be a boolean value.
		/// </summary>
		/// <remarks>The default value is true.</remarks>
		ClockwiseTurn
	}

	/// <summary>
	/// Current game phase.
	/// </summary>
	public enum GamePhase
	{
		/// <summary>
		/// The game is at a state of animating cards.
		/// </summary>
		Animating,
		/// <summary>
		/// A player has completely finished his/her turn. This comes after finishing the animation.
		/// </summary>
		EndTurn,
		/// <summary>
		/// A quick message is currently in place (eg. invalid move).
		/// </summary>
		Message,
		/// <summary>
		/// Game is loading.
		/// </summary>
		Initialzing,
		/// <summary>
		/// Game is ready to Start.
		/// </summary>
		Initialzed,
		/// <summary>
		/// Game is commerced
		/// </summary>
		Started,
		/// <summary>
		/// No action is taking place, thus idle.
		/// </summary>
		None
	}

	public enum UserType
	{
		/// <summary>
		/// Hosting a network game. In offline play, the local player is also considered as a host.
		/// </summary>
		Host,
		/// <summary>
		/// A standard player.
		/// </summary>
		Player,
		/// <summary>
		/// A user who's just watching a game.
		/// </summary>
		Spectator,
		/// <summary>
		/// [Online] - The user has left the game. 
		/// </summary>
		Leaver,
		/// <summary>
		/// Computer player
		/// </summary>
		Computer
	}

	/// <summary>
	/// Defines the type of server the player is currently on.
	/// </summary>
	public enum GameType
	{
		/// <summary>
		/// Tien Len multiplayer.
		/// </summary>
		Realtime,
		/// <summary>
		/// Tien Len multiplayer with bots.
		/// </summary>
		MultiplayerAI,
		/// <summary>
		/// Windows Live Messenger.
		/// </summary>
		TurnBased,
		/// <summary>
		/// Offline play.
		/// </summary>
		Offline
	}

	/// <summary>
	/// Defines what game would be among multiple games to play
	/// </summary>
	public enum Game
	{

		TienLen
	}

	public enum ActionType
	{
		Turn,
		Skipped,
		TurnCompleted
	}

	public enum AIType
	{
		Easy,
		Hard
	}

	public enum RoomStatus
	{
		InActive,
		Active
	}

	/*public enum MsgType
	{
		/// <summary>
		/// A invitation packet.
		/// </summary>
		Invite,
		/// <summary>
		/// Packet which tells the host the player has finished loading the client game.
		/// </summary>
		Ready,
		/// <summary>
		/// Packet which is an announcement message from the host.
		/// </summary>
		Announce,
		/// <summary>
		/// Packet received from host which tells information of the cards for a player.
		/// </summary>
		CardInfo,
		/// <summary>
		/// Ingame packet received from host indicating who will start first.
		/// </summary>
		PlayerStart,
		/// <summary>
		/// Ingame packet recieved from player about cards being sent to the field or if he has decided to skip the turn.
		/// </summary>
		FieldCards,
		/// <summary>
		/// Custom packet made by the game.
		/// </summary>
		Custom
	}*/
}