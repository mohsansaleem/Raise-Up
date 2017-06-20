using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class EmojiEnabler : MonoBehaviour {

	private Image image;
	private Image parentImage;

	void Awake() {
		image = GetComponent<Image>();
		parentImage = image.transform.parent.GetComponent<Image>();
	}

	// Use this for initialization
	void Start () {
		image = GetComponent<Image>();
		parentImage = image.transform.parent.GetComponent<Image>(); 
		parentImage.transform.SetAsFirstSibling();
		image.transform.SetAsFirstSibling();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SetActive(bool enable) {
		image.gameObject.SetActive(enable);
		parentImage.gameObject.SetActive(enable);
	}

	public void SetAbove() {
		image.enabled = true;
		parentImage.enabled = true;
		parentImage.transform.SetAsLastSibling();
		image.transform.SetAsLastSibling();
	}

	void OnEnable() {
//		image.enabled = true;
//		parentImage.enabled = true;
//		parentImage.transform.SetAsLastSibling();
//		image.transform.SetAsLastSibling();
	}

	void OnDisable() {
		image.enabled = false;
		parentImage.enabled = false;
		parentImage.transform.SetAsFirstSibling();
		image.transform.SetAsFirstSibling();
	}
}
