using UnityEngine;
using System.Collections;
using TienLen.Core.Models;

public class CardInfo : MonoBehaviour
{
	[SerializeField]
	private Card
		_card;

	public Card Card {
		get {
			return _card;
		}
		set {
			_card = value;
		}
	}
}
