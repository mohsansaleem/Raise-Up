using System;
using System.Collections;
using Client.Utils.Enums;
using TienLen.Core.Models;
using Zems.Common.Domain.Messages;

namespace Client.Misc
{
	public class ConnectResponseEventArgs : EventArgs
	{
		private bool _success;
		private string _message;
		
		public ConnectResponseEventArgs(bool success, string message)
		{
			_success = success;
			_message = message;
		}
		
		public bool Success
		{
			get { return _success; }
		}

		public string Message
		{
			get { return _message; }
		}
	}

	public class ConnectionStateEventArgs : EventArgs
	{
		private ConnectionState _state;
		
		public ConnectionStateEventArgs(ConnectionState state)
		{
			_state = state;
		}

		public ConnectionState State
		{
			get { return _state; }
		}
	}

	public class ConnectionStateDetailedEventArgs : EventArgs
	{
		private ConnectionStateDetailed _state;
		
		public ConnectionStateDetailedEventArgs(ConnectionStateDetailed state)
		{
			_state = state;
		}
		
		public ConnectionStateDetailed State
		{
			get { return _state; }
		}
	}

	public class RoomStateEventArgs : EventArgs
	{
		private RoomState _state;
		private IGame _gameState;

		public RoomStateEventArgs(RoomState state, IGame gameState)
		{
			_state = state;
			_gameState = gameState;
		}

		public RoomState State
		{
			get { return _state; }
		}

		public IGame GameState
		{
			get { return _gameState; }
		}
	}

	public class PlayerStateEventArgs : EventArgs
	{
		private Player _playerState;
		
		public PlayerStateEventArgs(Player state)
		{
			_playerState = state;
		}
		
		public Player PlayerState
		{
			get { return _playerState; }
		}
	}

	public class CustomEventArgs : EventArgs
	{
		private object[] _params;

		public CustomEventArgs(params object[] param)
		{
			_params = param;
		}

		public object[] Params {
			get {
				return _params;
			}
		}
	}

	public class TableStateEventArgs : EventArgs
	{
		private TableAction _tableAction;
		private object _param;

		public TableStateEventArgs(TableAction tableAction, object param)
		{
			_tableAction = tableAction;
			_param = param;
		}

		public TableAction TableAction {
			get {
				return _tableAction;
			}
		}

		public object Param {
			get {
				return _param;
			}
		}
	}

	public static class ClientEvents {

		public static event EventHandler<ConnectResponseEventArgs> ConnectResponseChanged;

		public static event EventHandler<ConnectionStateDetailedEventArgs> ConnectionStateDetailedChanged;
		public static event EventHandler<ConnectionStateEventArgs> ConnectionStateChanged;

		public static event EventHandler<RoomStateEventArgs> RoomStateChanged;
		public static event EventHandler<PlayerStateEventArgs> PlayerStateChanged;

		public static event EventHandler<CustomEventArgs> CustomEventChanged;
		public static event Action<IActionMessage>  TableStateChanged;

		internal static void OnConnectResponseChanged(bool success, string message)
		{
			if (ConnectResponseChanged != null)
				ConnectResponseChanged(null, new ConnectResponseEventArgs(success, message));
		}

		internal static void OnConnectionStateDetailedChanged(ConnectionStateDetailed state)
		{
			if (ConnectionStateDetailedChanged != null)
				ConnectionStateDetailedChanged(null, new ConnectionStateDetailedEventArgs(state));
		}

		internal static void OnConnectionStateChanged(ConnectionState state)
		{
			if (ConnectionStateChanged != null)
				ConnectionStateChanged(null, new ConnectionStateEventArgs(state));
		}

		internal static void OnRoomStateChanged(RoomState state, IGame gameState)
		{
			if(RoomStateChanged != null)
				RoomStateChanged(null, new RoomStateEventArgs(state, gameState));
		}

		internal static void OnPlayerStateChanged(Player state)
		{
			if(PlayerStateChanged != null)
				PlayerStateChanged(null, new PlayerStateEventArgs(state));
		}

		internal static void OnCustomEventChanged(params object[] param)
		{
			if(CustomEventChanged != null)
				CustomEventChanged(null, new CustomEventArgs(param));
		}

		internal static void OnTableStateChanged(IActionMessage innerMessage)
		{
			if(TableStateChanged != null)
				TableStateChanged(innerMessage);
		}
	}
}