using UnityEngine;
using System.Collections;

public class ChatDrag : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	public void OnDrag(){ 
		transform.position = Input.mousePosition; 
	}
}
