using UnityEngine;
using System.Collections;

namespace Game.UI
{
	[SerializeField]
	public interface IWidget
	{
		void Show (params object[] param);

		void Hide ();

		void Destroy ();
	}
}