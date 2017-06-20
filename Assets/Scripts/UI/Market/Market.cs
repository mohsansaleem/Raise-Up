using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Game.Managers;
using Global;

namespace Game.UI
{
	public class Market : Widget<Market>
	{
		public Text Currency;
		public GameObject InAppsLoadPanel;
		public List<MarketNode> MarketNodes;

		override public void Show (params object[] param)
		{
			base.Show (param);

			if (DataBank.Instance.user != null)
				PopulateUserData ();

			DataBank.Instance.OnGameStateUpdate += PopulateUserData;



			PopulateProducts ();

		}

		private void PopulateProducts ()
		{
			var productsList = InAppsManager.Instance.GetProductsList ();

			Debug.LogError ("PopulateProducts: " + productsList.Count);

			InAppsLoadPanel.SetActive (productsList.Count == 0);

			productsList.Sort ((a, b) => {
				return a.ProductPrice.CompareTo (b.ProductPrice); });

			int i = 0;

			foreach (var prod in productsList) {
				if (i >= MarketNodes.Count)
					break;
				Debug.LogError (prod.ProductName + " Updated");
				MarketNodes [i++].MarketProduct = prod;
			}
		}

		public override void Hide ()
		{
			base.Hide ();
			DataBank.Instance.OnGameStateUpdate -= PopulateUserData;
		}
		
		override public void Destroy ()
		{
			DataBank.Instance.OnGameStateUpdate -= PopulateUserData;
			base.Destroy ();
		}

		void PopulateUserData ()
		{
			Currency.text = DataBank.Instance.user.gameState.wallet.chips.ToString ();
		}

		public void OnExitClick ()
		{
			UIManager.Instance.PopScreen ();
		}
	}
}