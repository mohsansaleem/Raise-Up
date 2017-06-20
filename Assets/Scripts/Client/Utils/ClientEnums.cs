using UnityEngine;
using System.Collections;

namespace Client.Utils.Enums
{
	public enum RoomState {

		READY,
		WAITING
	}

	public enum ConnectionStateDetailed {
		
		Initializing,
		Initialized,
		Connecting,
		Connected,
		Disconnecting,
		Disconnected
	}

	public enum ConnectionState {

		Connected,
		Disconnected
	}

	public enum ClientRequests {

		CONNECT,
		DISCONNECT,
		CHAT,
		REQUESTUSERS,
		JOINGAME,
		TABLEUPDATE
	}

	public enum ServerResponses {

		CONNECTED,
		JOINED,
		REFUSED,
		CHAT,
		LISTUSERS,
		BROADCAST,
		READYTOSTART,
		TABLEUPDATE
	}

	public enum TableAction : byte {
		
		PlayerTurn = 0,
		PlayerAction,
		PlayerState,
		PlayerNames,
		SubmitCards,
		PassTurn,
		ResetPlayerCards,
		ResetAllPlayerCards,
		ResetAllPlayerActions,
		RemoveAllCardsOnTable,
		RemovePlayerCardsOnTable,
		RemovePlayerSelectedCards,
		GameStarted,
		GameFinished
	}
}