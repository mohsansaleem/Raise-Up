using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Global;
using Game.Managers;
using TienLen.Core.Models;
using Facebook.Unity;
using Newtonsoft.Json;
using TienLen.Managers;
using System.Collections.Generic;
using Game.Enums;

namespace Game.UI
{
	
	
	public class FriendRow : Widget<FriendRow> {

		public enum FriendRowType
		{
			Friend,
			Request
		}

		public User userData;

		public Text userNameText;
		public Image userImage;
		public Button viewProfileBtn;
		public Button acceptRequestBtn;

		public FriendRowType friendRowType = FriendRowType.Friend;
		
		override public void Show (params object[] param)
		{
			base.Show (param);
			friendRowType = (FriendRowType)param [0];
			UpdateView (friendRowType);
		}

		public void UpdateView(FriendRowType rowType){
			switch(rowType){
			case FriendRowType.Friend:
				viewProfileBtn.gameObject.SetActive(true);
				acceptRequestBtn.gameObject.SetActive(false);
				break;
			case FriendRowType.Request:
				viewProfileBtn.gameObject.SetActive(false);
				acceptRequestBtn.gameObject.SetActive(true);
				break;
			}
			friendRowType = rowType;
			userNameText.text = userData.username;
			if (userData.userProfile.customFields != null) {
				if (userData.userProfile.customFields.fbId != null) {
					StartCoroutine (SetFBPic (userData.userProfile.customFields.fbId));
				}
				else{
					StartCoroutine (SetFBPic ("100003908174930"));
				}
			} else {
				StartCoroutine (SetFBPic ("100003908174930"));
			}

		}

		public void ViewProfile(){
			if (userData != null) {
				UIManager.Instance.ShowUI (GameUI.UserProfile, PreviousScreenVisibility.DoNothing, userData,Profile.ProfileType.Friend);
			}
		}

		public void AcceptRequest(){
			Networking.Instance.AcceptFriendRequest (userData.userID, (response) =>
			                                     {
				friendRowType = FriendRowType.Friend;
				UpdateView(friendRowType);
				Debug.Log("AcceptFriendRequest Response: " + response);
			}
			, () => {});
		}

		IEnumerator SetFBPic(string fbId)
		{
			WWW url = new WWW ("http" + "://graph.facebook.com/" + fbId + "/picture"); //100003908174930
			yield return url;
			Texture2D textFb2 = new Texture2D (50, 50, TextureFormat.RGB24, false); //TextureFormat must be DXT5
			
			url.LoadImageIntoTexture (textFb2);
			userImage.sprite = Sprite.Create(textFb2,new Rect(0,0,textFb2.width,textFb2.height),new Vector2(0.5f,0.5f),100);
			userImage.color = Color.white;
			Debug.LogError ("Loading Image complete");
		}
		
	}
}
