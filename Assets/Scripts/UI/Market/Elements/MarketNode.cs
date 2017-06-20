using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Global;
using Game.Managers;

namespace Game.UI
{
	public class MarketNode : MonoBehaviour
	{
		public Text ProductName;
		public Text ProductPrice;
		public Text ProductChips;

		private MarketProduct marketProduct;

		public MarketProduct MarketProduct {
			set {
				marketProduct = value;
				Debug.LogError("in-app item - name = "+marketProduct.ProductName+" id = "+marketProduct.ProductId);
//				ProductName.text = marketProduct.ProductName;
				ProductPrice.text = marketProduct.ProductPriceWithLabel;

				// TODO: If we add functionality to load inApps view on Server Data then change this one too. Also add the Sprite for respective InApps.
				//ProductChips.text = marketProduct
			}

			get {
				return marketProduct;
			}
		}

		public void OnBuyClick ()
		{
			Debug.LogError ("Going to Buy: " + ProductName.text);
			if (marketProduct != null)
				InAppsManager.buyItem (marketProduct.ProductId);
			else
				Debug.LogError ("marketProduct is null.");
		}
	}
}