using UnityEngine;
using System.Collections;

namespace Game.UI
{
	public abstract class Widget <T> : MonoBehaviour, IWidget where T : MonoBehaviour
	{
		public static Widget<T> Instance { get; private set; }

		public CanvasGroup _canvasGroup;

		// Use this for initialization
		void Start ()
		{
			Instance = this;
		}

		virtual public void Show (params object[] param)
		{
			gameObject.SetActive (true);
		}
		
		virtual public void Hide ()
		{
			gameObject.SetActive (false);
		}

		virtual public void Disable ()
		{
			_canvasGroup.alpha = 0.7f;
			_canvasGroup.interactable = false;
			_canvasGroup.blocksRaycasts = false;
		}

		virtual public void Enable ()
		{
			_canvasGroup.alpha = 1;
			_canvasGroup.interactable = true;
			_canvasGroup.blocksRaycasts = true;
		}
		
		virtual public void Destroy ()
		{
			DestroyImmediate (gameObject);
		}
	}
}