using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Global;
using Game.Managers;
using TienLen.Core.Models;
using Facebook.Unity;
using Newtonsoft.Json;
using Game.Enums;

namespace Game.UI
{


	public class Profile : Widget<Profile>
	{

		public enum ProfileType
		{
			Self,
			Friend,
			Other
		}

		public GameObject SendChipsPanel;

		public Image profileImage;
		public Text userName;
		public Text userChips;
		public Text userWins;
		public Button addFriendBtn;
		public Button unFriendBtn;
		public Button sendChipsBtn;
		public Button voteKickBtn;
		public Button viewFriendsBtn;
		public Button viewRequestsBtn;
		public Button inviteBtn;

		public InputField sendChipsText;

		private User userData; 
		private Player playerData;

		private ProfileType profileType = ProfileType.Self;

		override public void Show (params object[] param)
		{
			base.Show (param);
			//ResetUI ();

			//StartCoroutine( MyPictureCallback ());
			ShowSendChipsWindow (false);

			userData = null;
			playerData = null;

			PopulateData (param);

			DataBank.Instance.OnGameStateUpdate += PopulateData;

			MyPictureCallback ();
		}

		public void PopulateCurrentUserData ()
		{
			Debug.LogError ("PopulateCurrentUserData Called");

			PopulateData (DataBank.Instance.user);
		}

		public void PopulateData ()
		{
			Debug.LogError ("Profile.PopulateData Called");
			if ((userData != null && userData.userID.Equals (DataBank.Instance.user.userID)) || (playerData != null && playerData.ID.Equals (DataBank.Instance.user.userID.ToString ()))) {
				PopulateCurrentUserData ();
			} else if (userData != null)
				PopulateData (userData, profileType);
			else if (playerData != null)
				PopulateData (playerData, profileType);
		}

		void PopulateData (params object[] param)
		{
			Debug.LogError ("PopulateData(param) Called: " + param [0].ToString ());
			if (param [0] is User) {
				userData = (User)param [0];
				Debug.Log ("Profile User Params-> Name: " + userData.username + " Chips:" + userData.gameState.wallet.chips + " Wins:" + userData.gameState.games.Count);
				if (userData.username != null) {
					userName.text = userData.username;
				} else {
					userName.text = "Name Nil";
				}
				if (userData.gameState != null) {
					userChips.text = userData.gameState.wallet.chips.ToString ();
					userWins.text = userData.gameState.games.Count.ToString ();
				} else {
					userChips.text = "nil";
					userWins.text = "nil";
				}
				
				if (userData.userID != null) {
					if (userData.userID == DataBank.Instance.user.userID) {
						profileType = ProfileType.Self;
					} else {
						if (param.Length > 1) {
							if (param [1] != null) {
								profileType = (ProfileType)param [1];
							} else {
								profileType = ProfileType.Other;
							}
						} else {
							profileType = ProfileType.Other;
						}
					}
				}
				
			} else if (param [0] is Player) {
				playerData = (Player)param [0];
				//Debug.Log ("Profile Player Params-> Name: " + playerData.Name + " Chips:" + playerData.Chips + " Wins:" + "nil");
				userName.text = playerData.Name;
				userChips.text = playerData.Chips.ToString ();
				userWins.text = "nil";
			}
			
			UpdateProfileDialouge ();
		}

		public void UpdateProfileDialouge ()
		{
			switch (profileType) {
			case ProfileType.Self:
				addFriendBtn.gameObject.SetActive (false);
				unFriendBtn.gameObject.SetActive (false);
				sendChipsBtn.gameObject.SetActive (false);
				voteKickBtn.gameObject.SetActive (false);
				viewFriendsBtn.gameObject.SetActive (true);
				viewRequestsBtn.gameObject.SetActive (true);
				inviteBtn.gameObject.SetActive(true);
				break;
			case ProfileType.Friend:
				addFriendBtn.gameObject.SetActive (false);
				unFriendBtn.gameObject.SetActive (true);
				sendChipsBtn.gameObject.SetActive (true);
				voteKickBtn.gameObject.SetActive (true);
				viewFriendsBtn.gameObject.SetActive (false);
				viewRequestsBtn.gameObject.SetActive (false);
				inviteBtn.gameObject.SetActive(false);
				break;
			case ProfileType.Other:
				addFriendBtn.gameObject.SetActive (true);
				unFriendBtn.gameObject.SetActive (false);
				sendChipsBtn.gameObject.SetActive (false);
				voteKickBtn.gameObject.SetActive (true);
				viewFriendsBtn.gameObject.SetActive (false);
				viewRequestsBtn.gameObject.SetActive (false);
				inviteBtn.gameObject.SetActive(false);
				break;
			}
		}

		public void OnViewFriends ()
		{
			UIManager.Instance.ShowUI (GameUI.Friends, PreviousScreenVisibility.DoNothing, Friends.FriendsListType.FriendsList);
		}

		public void OnViewRequests ()
		{
			UIManager.Instance.ShowUI (GameUI.Friends, PreviousScreenVisibility.DoNothing, Friends.FriendsListType.FriendRequests);
		}

		public void OnAddFriend ()
		{
			Networking.Instance.SendFriendRequest (userData.userID, (reply) => {
				Debug.Log ("SendFriendRequest Response: " + reply);

			}, () => {
				Debug.Log ("Friend Request Failed!");
			});
		}

		public void OnUnFriend ()
		{
			Networking.Instance.RemoveFriend (userData.userID, (reply) => {
				Debug.Log ("RemoveFriend Response: " + reply);
				WebSocket_Client.Client.Instance.friendsID.Remove (userData.userID);
				profileType = ProfileType.Other;
				UpdateProfileDialouge ();
				
			}, () => {
				Debug.Log ("RemoveFriend Request Failed!");
			});
		}

		public void OnInviteFriends() {
//			FB.Mobile.AppInvite(new Uri("https://fb.me/761695860600029"), callback: this.HandleResult);

			FB.Mobile.AppInvite(new System.Uri("https://fb.me/761695860600029"), new System.Uri("https://s3.amazonaws.com/frag-fbomb/raise-up/u.png"), (result) => {
				Debug.LogError(result);
				WebSocket_Client.Client.Instance.NotificationReceivedFromServer("App invite sent");
			});

		}

		public void OnCloseButton ()
		{
			Friends friendScreen = UIManager.Instance.GetScreen (GameUI.Friends, false, true, false)  as Friends;
			if (friendScreen != null) {
				friendScreen.UpdateView ();
			}
			UIManager.Instance.DequeueUI (PreviousScreenVisibility.DoNothing);
		}

		public void ShowSendChipsWindow (bool doShow)
		{
			sendChipsText.text = "";
			SendChipsPanel.SetActive (doShow);
		}

		public void OnSendChips ()
		{
			long chips = long.Parse (sendChipsText.text);

			if (chips < 1) {
				sendChipsText.text = "";
				//IOSNativePopUpManager.showMessage ("Please Enter a valid price");
				return;
			}

			if (userData != null && DataBank.Instance.user.gameState.wallet.chips > chips) {
				Networking.Instance.SendChipsToFriend (userData.userID, chips, (reply) => {

					if (userData != null)
						userData.gameState.wallet.chips += chips;
					else if (playerData != null)
						playerData.Chips += (int)chips;

					DataBank.Instance.AddChipsLocally (-chips);
					Debug.LogError("Send Chips - "+chips);
					ShowSendChipsWindow (false);
				}, () => {});
			} else {

			}
		}

		void MyPictureCallback () // store user profile pic
		{
			//if (FB.IsLoggedIn) {
			Debug.LogError ("Loading Image");
			string fbID;
			//if (FB.IsLoggedIn) {
			//	fbID = Facebook.Unity.AccessToken.CurrentAccessToken.UserId;
			//} else {
			if (userData.userProfile == null) {
				Debug.LogError("userProfile is null");
				Networking.Instance.GetUserProfile (userData.userID, (reply) => {
					Debug.Log ("User Profile: " + reply);
					User usr = JsonConvert.DeserializeObject<User> (reply);
					fbID = usr.userProfile.customFields.fbId;
					Debug.LogError("GetUserProfile response - "+fbID);
//					StartCoroutine (SetFBPic (Game.Constants.DEFAULT_FBID));
					profileImage.sprite =  Resources.Load<Sprite>("Textures/Shared/chips");

				}, () => {
//					fbID = "100003908174930";
					profileImage.sprite =  Resources.Load<Sprite>("Textures/Shared/chips");
					Debug.LogError ("getUserprofile false");

//					StartCoroutine (SetFBPic (Game.Constants.DEFAULT_FBID));
				});
			} else {
				Debug.LogError("userProfile is not null");
				if (userData.userProfile.customFields != null) {
					if (userData.userProfile.customFields.fbId != null) {
						StartCoroutine (SetFBPic (userData.userProfile.customFields.fbId));
						Debug.LogError("Trying to download FB profile pic - "+userData.userProfile.customFields.fbId);


					} else {
//						fbID = "100003908174930";
						profileImage.sprite =  Resources.Load<Sprite>("Textures/Shared/chips");
						Debug.LogError("fb id =  null");
//						StartCoroutine (SetFBPic (Game.Constants.DEFAULT_FBID));
					}
				} else {
//					fbID = "100003908174930";
					profileImage.sprite =  Resources.Load<Sprite>("Textures/Shared/chips");
					Debug.LogError("custom field =  null");

//					StartCoroutine (SetFBPic (Game.Constants.DEFAULT_FBID));
				}
			}

			//}

		}

		public override void Hide ()
		{
			DataBank.Instance.OnGameStateUpdate -= PopulateData;
			base.Hide ();
		}

		override public void Destroy ()
		{
			DataBank.Instance.OnGameStateUpdate -= PopulateData;
			base.Destroy ();
		}

		int retry = 0;
		int MAXRETRY = 5;

		IEnumerator SetFBPic (string fbId)
		{
			WWW url = new WWW ("http" + "://graph.facebook.com/" + fbId + "/picture"); //100003908174930
			int count = 1;
			Debug.LogError("bytes downloadd "+url.bytesDownloaded + " url - "+url.url);
			yield return url;
			Texture2D textFb2 = new Texture2D (200, 200, TextureFormat.RGB24, false); //TextureFormat must be DXT5
			Debug.LogError(url.url+"  "+url.texture.name+" "+url.bytesDownloaded+" "+url.ToString()+" "+url.texture.format.ToString());
			url.LoadImageIntoTexture (textFb2);
			if(textFb2.height == 8 && textFb2.width == 8) {
				retry++;
				if(retry >= MAXRETRY) {
					retry = 0;
					profileImage.sprite = Resources.Load<Sprite>("Textures/Shared/chips");
				} else {
					yield return new WaitForSeconds(15.0f);
					
					yield return SetFBPic(fbId);
					Debug.LogError("Retrying count = "+retry);
				}
				yield break;
			}
			profileImage.sprite = Sprite.Create(textFb2,new Rect(0,0,textFb2.width,textFb2.height),new Vector2(0.5f,0.5f), 100);
			Debug.LogError(profileImage.sprite  == null ? " is null" : "not null");
			profileImage.color = Color.white;
			Debug.LogError ("Loading Image complete");
		}
	}
}
