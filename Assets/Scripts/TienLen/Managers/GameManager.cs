using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using TienLen.Core.Models;
using TienLen.Core.GamePlay;
using TienLen.Utils.Constants;
using TienLen.Utils.Enums;
using TienLen.Utils.Helpers;
using TienLen.View;
using Client.Misc;
using Client.Utils.Enums;
using Game;
using Game.UI;
using Game.Managers;
using Zems.Common.Domain.Messages;
using Global;

namespace TienLen.Managers
{

	public class GameManager : Singleton<GameManager>
	{
		[SerializeField]
		private bool
			CreateNewUser;

		[SerializeField]
		private TienLen.Utils.Enums.Game
			game = TienLen.Utils.Enums.Game.TienLen; // default Game.TienLen
		[SerializeField]
		private GameType
			gameType = GameType.Offline; // default GameType.Offline

		private GamePhase gamePhase = GamePhase.None;
		private IGame gameInstance;

		GameEngine gameEngine;

		private TienLenClient client;

		public GamePhase Phase {
			get {
				return gamePhase;
			}
			set {
				gamePhase = value;
			}
		}

		void Testing ()
		{
			Debug.LogError ("Timer Start Time: " + DateTime.Now.ToString ());
			System.Threading.Timer timer = null; 

			timer = new System.Threading.Timer ((obj) =>
			{
				Debug.LogError ("Timer Called");
				Debug.LogError ("Timer Ended Time: " + DateTime.Now.ToString ());
				timer.Dispose ();
			}, 
			null, 3000, System.Threading.Timeout.Infinite);
			
			timer.Dispose ();
			Debug.LogError ("Timer Disposed: " + DateTime.Now.ToString ());
		}

		protected override void Start ()
		{
			//Testing ();

			if (CreateNewUser)
				PlayerPrefs.DeleteAll ();
			if (PlayerPrefs.HasKey ("userName")) 
				UIManager.Instance.ShowUI (Game.Enums.GameUI.GameMenu, Game.Enums.PreviousScreenVisibility.DequeuePreviousExcludingHud);
			else
				UIManager.Instance.ShowUI (Game.Enums.GameUI.Signup, Game.Enums.PreviousScreenVisibility.DequeuePreviousExcludingHud);

		}

		private bool InitGame ()
		{
			UIManager.Instance.ShowUI (Game.Enums.GameUI.TienLenClient);
			client = (TienLenClient)UIManager.Instance.GetScreen (Game.Enums.GameUI.TienLenClient);
			client.ResetTable ();
			gameInstance = CreateGameInstance ();
			return true;
		}

		public void StartOffline (int buyIns)
		{
			((Lobby)Lobby.Instance).SelectedRom = null;
			gameType = GameType.Offline;
			if (!InitGame ())
				return;
			gamePhase = GamePhase.Initialzing;

			BeginOffline (buyIns);
		}

		public void StartOnline (int buyIns)
		{
			gameType = GameType.MultiplayerAI;
			if (!InitGame ())
				return;
			JoinMultiplayer (buyIns);
		}

		private Player[] CreateOfflinePlayers (IGame gameInstance)
		{

			Player[] players = new Player[gameInstance.MaxPlayers ()];
			List<string> allocatedNames = new List<string> (gameInstance.MaxPlayers ());

			for (int i = 0; i < gameInstance.MaxPlayers(); i++) {
				if (i == 2)
					players [i] = new Player ("You", UserType.Host, i + 1, null);
				else {
					string name = Helper.GenerateName ();

					// to ensure unique names
					while (allocatedNames.Contains(name))
						name = Helper.GenerateName ();

					allocatedNames.Add (name);
					players [i] = new Player (name, UserType.Computer, i + 1, null);
					players [i].AIFunc = CreateAIInstance ();
				}
			}

			allocatedNames.Clear ();
			allocatedNames = null;

			return players;
		}

		private IGame CreateGameInstance ()
		{
			IGame gameInstance = null;

			switch (game) {
			case TienLen.Utils.Enums.Game.TienLen:
				gameInstance = (IGame)Activator.CreateInstance (typeof(TienLenGame));
				break;
			}

			return gameInstance;
		}

		private IComputer CreateAIInstance ()
		{
			IComputer computer = null;

			switch (game) {
			case TienLen.Utils.Enums.Game.TienLen:
				computer = (IComputer)Activator.CreateInstance (typeof(TienLenAI));
				break;
			}

			return computer;
		}

		private void BeginOffline (int buyIns)
		{
			if (client != null) {
				Debug.Log ("BeginOffline");
				if (gameEngine != null)
					((TienLenGameEngine)gameEngine).TableActionChanged = null;
				DataBank.Instance.AddChipsLocally(-1*buyIns);
				gameEngine = new TienLenGameEngine (gameInstance, CreateOfflinePlayers (gameInstance), gameType, buyIns, client.OnTableStateChanged);
				client.InitParams (gameEngine.game, gameEngine.players, gameEngine.gameType, buyIns, (TienLenGameEngine)gameEngine);

			} else
				Logger.Log ("Unable to find TienLenClient component. Game won't start!");
		}

//		private void TryLogin ()
//		{
//			gameType = GameType.Offline;
//			BeginOffline ();
//			return;
//			ClientEvents.ConnectResponseChanged += ConnectResponseChanged;
//			if (ClientManager.Instance.Connected) {
//				PopupHandler.Instance.Show ("Join Match?", new ButtonProps[] {
//					new ButtonProps { text = "OK" , callback = () => JoinMultiplayer() } ,
//					new ButtonProps { text = "Play Offline" , callback = () => { gameType = GameType.Offline; BeginOffline(); } } 
//				});
//			} else {
//				ClientManager.Instance.TryConnect ();
//				// TODO after login, server will let you join Lobby, then join any of listed room from lobby
//			}
//		}

//		private void ConnectResponseChanged (object sender, ConnectResponseEventArgs e)
//		{
//			if (e.Success) {
//				PopupHandler.Instance.Show ("Join Match?", new ButtonProps[] {
//					new ButtonProps { text = "OK" , callback = () => JoinMultiplayer() } ,
//					new ButtonProps { text = "Play Offline" , callback = () => { gameType = GameType.Offline; BeginOffline(); } } 
//				});
//			} else {
//				PopupHandler.Instance.Show ("Sorry we are experiencing problem at server side, try offline mode", new ButtonProps[] {
//					new ButtonProps { text = "OK" , callback = () => { gameType = GameType.Offline; BeginOffline(); } } ,
//					new ButtonProps { text = "Retry" , callback = () => TryLogin() } 
//				});
//			}
//
//			Logger.Log (e.Message);
//		}

		/*
		private void PlayerStateChanged (object sender, PlayerStateEventArgs e)
		{

		}*/

		public void TableStateChanged (IActionMessage innerMessage)
		{
			if (client == null) {
				Debug.LogError ("Client is Null.");
			} else
				client.TableStateChanged (innerMessage);
		}


		private void JoinMultiplayer (int buyIns)
		{
			//ClientEvents.ConnectResponseChanged -= ConnectResponseChanged;
			//ClientEvents.PlayerStateChanged += PlayerStateChanged;
//			DataBank.Instance.AddChipsLocally(-1*buyIns);
			client.InitParams (gameInstance, new Player[gameInstance.MaxPlayers ()], gameType, buyIns);
			//ClientManager.Instance.JoinMultiplayerGame (game.ToString (), gameType.ToString ());
		}

		public void QuitGame ()
		{
			Application.Quit ();
		}
	}
}