using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Game;
using Game.UI;
using Game.Enums;
using Game.Managers;

namespace Game.Managers
{
	public class PopupManager : Singleton<PopupManager>
	{
		
		[SerializeField]
		private Transform
			_parentCanvas;
		[SerializeField]
		public List<Popup>
			_visiblePopupQueue;

		public GameObject _popupInstance;

		protected override void Start ()
		{
			base.Start ();
			_visiblePopupQueue = new List<Popup> ();
		} 
		
		
		public void ShowPopup (PopupType popupToShow, PopupData data, Vector3 pos, bool hidePrev = true)
		{
			if (_visiblePopupQueue == null)
				_visiblePopupQueue = new List<Popup> ();

			HidePreviousPopup (data, hidePrev);
			Popup popupRef = null;


			switch (popupToShow) {
			case PopupType.LoadingPopup:
					
				if (_popupInstance == null) {
					GameObject obj = Instantiate (PrefabsManager.Instance.Load (Constants.POPUP_PATH), Vector3.zero, Quaternion.identity)as GameObject;
						
					obj.transform.SetParent (_parentCanvas, false);
					_popupInstance = obj;
						
				}
				_popupInstance.transform.localPosition = pos;
				popupRef = _popupInstance.GetComponent (typeof(Popup)) as Popup;

				break;


			case PopupType.OneButton:

				if (_popupInstance == null) {
					GameObject obj = Instantiate (PrefabsManager.Instance.Load (Constants.POPUP_PATH), Vector3.zero, Quaternion.identity)as GameObject;
						
					obj.transform.SetParent (_parentCanvas, false);
					_popupInstance = obj;
						
				}
				_popupInstance.transform.localPosition = pos;
				popupRef = _popupInstance.GetComponent (typeof(Popup)) as Popup;
				break;

			case PopupType.TwoButton:

				if (_popupInstance == null) {
					GameObject obj = Instantiate (PrefabsManager.Instance.Load (Constants.POPUP_PATH), Vector3.zero, Quaternion.identity)as GameObject;
						
					obj.transform.SetParent (_parentCanvas, false);
					_popupInstance = obj;
						
				}
				_popupInstance.transform.localPosition = pos;
				popupRef = _popupInstance.GetComponent (typeof(Popup)) as Popup;
					
				break;

			case PopupType.ThreeButton:

				if (_popupInstance == null) {
					GameObject obj = Instantiate (PrefabsManager.Instance.Load (Constants.POPUP_PATH), Vector3.zero, Quaternion.identity)as GameObject;
						
					obj.transform.SetParent (_parentCanvas, false);
					_popupInstance = obj;
						
				}
				_popupInstance.transform.localPosition = pos;
				popupRef = _popupInstance.GetComponent (typeof(Popup)) as Popup;
					
				break;

			}
			// _popupInstance.GetComponent<Popup>().p=data;
			// _popupInstance.GetComponent<Popup>().pType=popupToShow;

			popupRef.pType = popupToShow;
			ShowPopup (popupToShow, popupRef, data, hidePrev);

		}
		
	
		
		public void ShowPreviousPopup (Popup data, bool hidePrev)
		{
			// if (_visiblePopupQueue.Count > 0)
			// this.ShowPopup(_visiblePopupQueue [_visiblePopupQueue.Count - 1], data, hidePrev);
		}
		
		void HidePreviousPopup (PopupData data, bool hidePrev)
		{

			if (hidePrev) {
				_visiblePopupQueue.Clear ();
			} else {
				if (_popupInstance == null)
					return;
				Popup p = new Popup ();
				p.pType = _popupInstance.GetComponent<Popup> ().pType;
				p.p = _popupInstance.GetComponent<Popup> ().p;
				_visiblePopupQueue.Add (p);
			}


		}
		

		
		private void ShowPopup (PopupType pType, Popup popupToShow, PopupData data, bool hidePrev)
		{
			if (popupToShow != null) {
				popupToShow.Show (pType, data);
			}
		}
		
		public void HidePopup ()
		{
			if (_popupInstance == null)
				return;
			Popup popupToHide = _popupInstance.GetComponent<Popup> ();
			popupToHide.Hide ();

			if (_visiblePopupQueue != null && _visiblePopupQueue.Count > 0) {

				Popup p = _visiblePopupQueue [_visiblePopupQueue.Count - 1];
				_visiblePopupQueue.Remove (p);
				ShowPopup (p.pType, p.p, Vector3.zero, true);
			}
           
		}
	}
}