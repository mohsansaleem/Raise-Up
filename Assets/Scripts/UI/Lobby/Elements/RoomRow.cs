using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Global;
using Game.Managers;

namespace Game.UI
{
	public class RoomRow : MonoBehaviour
	{
		[SerializeField]
		private Text
			roomName;
		[SerializeField]
		private Text
			roomPlayers;
		[SerializeField]
		private Text
			buyIn;
		[SerializeField]
		private Image
			backGround;

		private Room room;

		public Room Room {
			get {
				return room;
			}
			set {
				room = value;
	
				roomName.text = room.roomName;
				roomPlayers.text = room.roomUsers.Count.ToString () + "/" + room.roomSize;
				buyIn.text = "$" + room.buyIn;
			}
		}

		public void onClick (RoomRow roomRow)
		{
			Debug.LogError ("Lobby Room Row onClick.");
			((Lobby)UIManager.Instance.GetScreen (Game.Enums.GameUI.Lobby)).SelectedRom = roomRow;
		}

		public void OnJoinClick ()
		{
			Debug.LogError("OnJoinClick");
			if(((Lobby)Lobby.Instance).SelectedRom != null) {
				Debug.LogError("selected room = "+((Lobby)Lobby.Instance).SelectedRom.roomName);
			}
			if (Lobby.Instance != null) {
				((Lobby)Lobby.Instance).SelectRoom (Room);
				((Lobby)Lobby.Instance).JoinRoom ();
			}
		}

		public void  Select (bool select)
		{
//			backGround.color = new Color (backGround.color.r, backGround.color.g, backGround.color.b, select ? 0.4f : 0.2f);
			backGround.gameObject.SetActive(select);
		}
	}
}