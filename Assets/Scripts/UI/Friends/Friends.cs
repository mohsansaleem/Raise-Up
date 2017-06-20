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
using System.Linq;
using Game.Enums;

namespace Game.UI
{
	
	
	public class Friends : Widget<Friends> {

		public enum FriendsListType
		{
			FriendsList,
			FriendRequests,
			FriendInvites
		}

		public Text headingText;
		public Text descriptionText;
		public Transform friendsListParent;
		private List<User> friends;
		public FriendsListType friendListType = FriendsListType.FriendsList;
		
		override public void Show (params object[] param)
		{
			base.Show (param);

			friendListType = (FriendsListType)param [0];
			UpdateView ();
		}

		public void UpdateView(){

			foreach(FriendRow go in friendsListParent.GetComponentsInChildren<FriendRow>()){
				Destroy(go.gameObject);
			}

			switch(friendListType){
			case FriendsListType.FriendsList:

				headingText.text = "FRIENDS";

				Networking.Instance.ViewFriendsList ((response) =>
				                                     {
					FriendsResponse friendsResponse = JsonConvert.DeserializeObject<FriendsResponse> (response);
					friends = friendsResponse.friends;
					WebSocket_Client.Client.Instance.friendsID.Clear();
					WebSocket_Client.Client.Instance.friendsID = friendsResponse.friends.Select(x=> x.userID).ToList();
					Debug.Log("Friends Count: " + friends.Count);
					if(friends.Count != 0){
						PopulateList(friends);
					}
					else{
						descriptionText.enabled = true;
						descriptionText.text = "NO FRIENDS ADDED";
					}
					Debug.Log("ViewFriendsList Response: " + response);
				}
				, () => {});
					break;
			case FriendsListType.FriendRequests:

				headingText.text = "FRIEND REQUESTS";

				Networking.Instance.ViewFriendRequests ((response) =>
				                                     {
					FriendsResponse friendsResponse = JsonConvert.DeserializeObject<FriendsResponse> (response);
					friends = friendsResponse.requests;
					Debug.Log("Friends Count: " + friends.Count);
					if(friends.Count != 0){
						PopulateList(friends);
					}
					else{
						descriptionText.enabled = true;
						descriptionText.text = "NO FRIEND REQUESTS";
					}
					Debug.Log("ViewFriendRequests Response: " + response);
				}
				, () => {});
				break;
			case FriendsListType.FriendInvites:

				headingText.text = "FRIEND INVITES";

				Debug.Log("Not Implemented Yet!");
				break;
			}
		}

		private void PopulateList(List<User> friends){
			if (friends != null) {
				foreach (User friend in friends) {

					//GameObject go = PrefabsManager.Instance.Load (Constants.FRIEND_ROW_UI_PATH) as GameObject;
					GameObject go = GameObject.Instantiate( Resources.Load(Constants.FRIEND_ROW_UI_PATH)) as GameObject;
					FriendRow friendRow = go.GetComponent<FriendRow> ();
				
					friendRow.userData = friend;
				
					go.transform.SetParent (friendsListParent.transform, false);
					go.transform.localPosition = new Vector3 (friendRow.transform.localPosition.x, friendRow.transform.localPosition.y, 0);
					go.transform.localScale = Vector3.one;
					go.transform.SetAsFirstSibling ();
					switch (friendListType) {
					case FriendsListType.FriendsList:
						friendRow.UpdateView (FriendRow.FriendRowType.Friend);
						break;
					case FriendsListType.FriendRequests:
						friendRow.UpdateView (FriendRow.FriendRowType.Request);
						break;
					case FriendsListType.FriendInvites:
						Debug.Log ("Not Implemented Yet!");
						break;
					}
				}
			}
		}

		public void OnCloseButton(){
			UIManager.Instance.DequeueUI (PreviousScreenVisibility.DoNothing);
			/*
			foreach(FriendRow go in friendsListParent.GetComponentsInChildren<FriendRow>()){
				Destroy(go.gameObject);
			}
			UIManager.Instance.HideUI (Game.Enums.GameUI.Friends);
			*/
		}
		
	}
}
