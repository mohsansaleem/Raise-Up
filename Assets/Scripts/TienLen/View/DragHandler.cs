using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

namespace TienLen.View
{
	public class DragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler {

		public static GameObject objectBeingDragged;
		public static Vector3 lastEmptyPosition;

		#region IBeginDragHandler implementation

		public void OnBeginDrag (PointerEventData eventData)
		{
			objectBeingDragged = gameObject;
			lastEmptyPosition = transform.position;
			GetComponent<CanvasGroup> ().blocksRaycasts = false;
		}

		#endregion

		#region IDragHandler implementation

		public void OnDrag (PointerEventData eventData)
		{
			transform.position = eventData.position;
		}

		#endregion

		#region IEndDragHandler implementation

		public void OnEndDrag (PointerEventData eventData)
		{
			objectBeingDragged = null;
			transform.position = lastEmptyPosition;
			
			GetComponent<CanvasGroup> ().blocksRaycasts = true;
		}

		#endregion

		#region IPointerEnterHandler implementation

		public void OnPointerEnter (PointerEventData eventData)
		{
			Vector3 tempPosition;
			if(objectBeingDragged != null && transform.position != lastEmptyPosition)
			{
				tempPosition = transform.position;
				transform.position = lastEmptyPosition;
				lastEmptyPosition = tempPosition;
			}
		}

		#endregion
	}
}