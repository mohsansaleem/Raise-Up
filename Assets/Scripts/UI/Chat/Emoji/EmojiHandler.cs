using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EmojiHandler : MonoBehaviour {
	[SerializeField]
	private GameObject gridLayout;
	[SerializeField]
	private Button button;

	// Use this for initialization
	void Start () {
	
		string imageName = "Emoji";
		int count = 1;
		while(true) {
			string fileName = "Textures/Emojis/New/"+imageName+count;
//			Debug.LogError(fileName + " fetching");
			Sprite img = Resources.Load<Sprite>(fileName);
			if(img != null) {
				Button btn = Instantiate<Button>(button);
				btn.image.sprite = img;
				btn.gameObject.SetActive(true);
				btn.transform.SetParent(gridLayout.transform, false);
			} else {
				Debug.LogError("error in loading file "+imageName+count);
				break;
			}
			count++;
		}

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
