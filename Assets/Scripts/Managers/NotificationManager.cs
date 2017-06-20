using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using StansAssets.Animation;

public class NotificationManager : MonoBehaviour {

	private Animator animator;
	[SerializeField]
	private Text
		text;
	private float height;

	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator>();
//		height = gameObject.GetComponent<RectTransform>().position.y;
		height = gameObject.GetComponent<RectTransform>().GetHeight();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void show(string message) {
		transform.SetAsLastSibling();
		text.text = message;
		iTween.MoveBy(gameObject, iTween.Hash("y", - height - height/4, "time", 1.5f, "oncomplete", "oncomplete"));
	}

	public void oncomplete() {
		Debug.LogError("OnComplete called");
		iTween.MoveBy(gameObject, iTween.Hash("y", height + height/4, "time", 1.5f, "delay", 1.0f));
	}
}
