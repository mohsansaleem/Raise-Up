using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Global;

public class PlayerRow : MonoBehaviour
{
	[SerializeField]
	private Text
		playerName;
	[SerializeField]
	private Text
		playerRank;

	private User user;

	public User User {
		get {
			return user;
		}
		set {
			user = value;
			playerName.text = user.username;
			playerRank.text = user.userType.ToString ();
		}
	}
}
