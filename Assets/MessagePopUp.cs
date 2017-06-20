using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MessagePopUp : MonoBehaviour
{
	[SerializeField]
	private Text
		Message;

	[SerializeField]
	private Text
		MessageLayer;

	void Start ()
	{
		Invoke ("DestroySelf", 5f);
	}

	void DestroySelf ()
	{
		DestroyImmediate (gameObject);
	}

	public string SetMessage {
		set {
			Message.text = value;
			MessageLayer.text = value;
		}
	}
}
