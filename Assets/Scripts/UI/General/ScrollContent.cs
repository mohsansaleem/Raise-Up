using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public enum ScrollContentDirection
{
	Horizontal,
	Vertical
}

public class ScrollContent : MonoBehaviour
{
	
	RectTransform contentRect;
	public float spaceOfOneItem = 100f;
	public int minimumChildCount = 4;
	public ScrollContentDirection direction = ScrollContentDirection.Horizontal;
	
	void Awake ()
	{
		contentRect = this.GetComponent<RectTransform> ();
		if (contentRect == null)
			Debug.Log ("Rect not found. Root: " + this.transform.root.name);
	}
	
	void OnEnable ()
	{ // this runs before RefreshContent and runs everytime
//		Debug.Log("ScrollContent-OnEnable: " + direction.ToString());
//		Debug.Log("ScrollContent-OnEnable: " + direction.ToString() + "   Childs: " + this.contentRect.childCount + "   GameObject: " + gameObject.name);
/*		switch (direction) {
		case ScrollContentDirection.Horizontal:
			if ( !ReferenceEquals(transform.parent.GetComponent<ScrollRect>(), null) )
				transform.parent.GetComponent<ScrollRect>().horizontalNormalizedPosition = 0f;
		break;
		case ScrollContentDirection.Vertical:
			if ( !ReferenceEquals(transform.parent.GetComponent<ScrollRect>(), null) )
				transform.parent.GetComponent<ScrollRect>().verticalNormalizedPosition = 1f;
		break;
		}*/
		RefreshContent ();
	}
	
	void ChangeHeightOfContent (int rows = 0)
	{
//		Debug.Log("Child Count: " + contentRect.childCount.ToString() + "   Rows: " + rows );
		if (rows > 0) {
			contentRect.SetHeight (rows * spaceOfOneItem);
		} else if (contentRect.childCount > minimumChildCount) {
			contentRect.SetHeight (contentRect.childCount * spaceOfOneItem);
		} else {
			contentRect.SetHeight (minimumChildCount * spaceOfOneItem);
		}
	}
	
	void ChangeWidthOfContent (int columns = 0)
	{
//		Debug.Log("Child Count: " + contentRect.childCount.ToString() + "   Columns: " + columns );
		if (columns > 0) {
			contentRect.SetWidth (columns * spaceOfOneItem);
		} else if (contentRect.childCount > minimumChildCount) {
			contentRect.SetWidth (contentRect.childCount * spaceOfOneItem);
		} else {
			contentRect.SetWidth (minimumChildCount * spaceOfOneItem);
		}
	}

//    void OnEnable()
//    {
//    	this.PerformActionWithDelay(0.2f, () => {
//			RefreshContent();
//    	});
//    }
    
	public void RefreshContent ()
	{ // this runs after OnEnable and runs once on loading data
//		Debug.Log("ScrollContent-RefreshContent: " + direction.ToString());
//		Debug.Log("ScrollContent-RefreshContent: " + direction.ToString() + "   Childs: " + this.contentRect.childCount + "   GameObject: " + gameObject.name);
//		Debug.Log("ScrollContent-RefreshContent: " + direction.ToString() + "   Childs: " + this.transform.childCount + "   GameObject: " + gameObject.name);
		
		if (contentRect == null)
			contentRect = this.GetComponent<RectTransform> ();
		
		switch (direction) {
		case ScrollContentDirection.Horizontal:
			ChangeWidthOfContent (this.transform.childCount);
			if (gameObject.activeInHierarchy) {
				this.PerformActionWithDelay (0.05f, () => {
					if (!ReferenceEquals (transform.parent.GetComponent<ScrollRect> (), null))
						transform.parent.GetComponent<ScrollRect> ().horizontalNormalizedPosition = 0f;
				});
			}
			break;
		case ScrollContentDirection.Vertical:
			ChangeHeightOfContent (this.transform.childCount);
			if (gameObject.activeInHierarchy) {
				this.PerformActionWithDelay (0.05f, () => {
					if (!ReferenceEquals (transform.parent.GetComponent<ScrollRect> (), null))
						transform.parent.GetComponent<ScrollRect> ().verticalNormalizedPosition = 1f;
				});
			}
			break;
		}
	}
}
