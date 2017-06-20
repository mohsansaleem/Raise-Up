using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TienLen.Core.Models;

namespace Zems.Common.Domain.Messages
{
	public class PlayersStatus : IActionMessage
	{
		public List<PlayerStatus> GamePlayersStatus { get; set; }

		public PlayersStatus ()
		{
			GamePlayersStatus = new List<PlayerStatus> ();
		}

		public PlayersStatus (Player[] players)
		{
			GamePlayersStatus = new  List<PlayerStatus> ();
			foreach (Player player in players)
				GamePlayersStatus.Add (new PlayerStatus (player.ID, player.Number, player.Name, player.Results.Place, player.Chips));
		}
	}

	public class PlayerStatus
	{
		public string Id;
		public int Number;
		public string Name;
		public int Place;
		public int Chips;

		public PlayerStatus ()
		{
			Id = "";
			Number = -1;
			Name = "";
			Place = 0;
			Chips = 0;
		}

		public PlayerStatus (string id, int number, string name, int place, int chips)
		{
			Id = id;
			Number = number;
			Name = name;
			Place = place;
			Chips = chips;
		}
	}
}
