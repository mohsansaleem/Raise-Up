using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ExpandMessagePopUp : MonoBehaviour
{
	RectTransform rt;
	Text txt;
	
	void Start ()
	{
		rt = gameObject.GetComponent<RectTransform> (); // Acessing the RectTransform 
		txt = gameObject.GetComponent<Text> (); // Accessing the text component
	}
	
	void Update ()
	{
		rt.sizeDelta = new Vector2 (txt.preferredWidth, txt.preferredHeight); // Setting the height to equal the height of text
	}
}
