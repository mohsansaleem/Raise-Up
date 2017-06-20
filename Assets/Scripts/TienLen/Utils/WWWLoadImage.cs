using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WWWLoadImage : MonoBehaviour {

	private Image userImage;
	int retry = 0;
	int MAXRETRY = 5;

	public void Start(){
		if(userImage == null){
			userImage = gameObject.GetComponent<Image>();
		}
	}

	public void LoadWithFBID(string fbID){
		if (userImage != null) {
			StartCoroutine (SetFBPic (fbID));
		}
	}

	public void SetDefaultProfilePicture(){Debug.LogError("*************** Loading defaultProfilePicture ***************");
		userImage.sprite =  Resources.Load<Sprite>("Textures/Shared/profile_default");
	}

	public void SetDefault() {
		userImage.sprite =  Resources.Load<Sprite>("Textures/Shared/chips");
	}

	IEnumerator SetFBPic(string fbId) {
		WWW url = new WWW ("https" + "://graph.facebook.com/" + fbId + "/picture"); //100003908174930
//		WWW url = new WWW ("http://plumtri.org/sites/all/themes/plumtritheme/images/default_profile.jpg"); //100003908174930
		Debug.LogError("*********************wurl text = "+url.url+" *****************");
		int count = 1;
		Debug.LogError("bytes downloadd "+url.bytesDownloaded);
		yield return url;
		Texture2D textFb2 = new Texture2D (50, 50, TextureFormat.RGB24, false); //TextureFormat must be DXT5
		Debug.LogError(url.url+"  "+url.texture.name+" "+url.bytesDownloaded+" "+url.ToString()+" "+url.texture.format.ToString());
		url.LoadImageIntoTexture (textFb2);
		if(textFb2.height == 8 && textFb2.width == 8) {
			retry++;
			if(retry == MAXRETRY) {
				userImage.sprite =  Resources.Load<Sprite>("Textures/Shared/chips");
			} else {
				yield return new WaitForSeconds(15.0f);

				yield return SetFBPic(fbId);
//				StartCoroutine (SetFBPic (fbId));
				Debug.LogError("Retrying count = "+retry);
			}
			yield break;
		}
		userImage.sprite = Sprite.Create(textFb2,new Rect(0,0,textFb2.width,textFb2.height),new Vector2(0.5f,0.5f), 100);
		Debug.LogError(userImage.sprite  == null ? " is null" : "not null");
		userImage.color = Color.white;
		Debug.LogError ("Loading Image complete");
	}
}
