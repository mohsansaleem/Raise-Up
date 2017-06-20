using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Global;
using Game;
using Game.UI;
using Game.Enums;
using System.Linq;
using TienLen.Core.GamePlay;

namespace Game.Managers
{
	public class InAppsManager : Singleton<InAppsManager>
	{
		public List<string> IOSProductIds;
		public List<string> AndroidProductIds;

		private static bool IsInitialized = false;

		// Use this for initialization
		void Start ()
		{
			if(Application.platform == RuntimePlatform.Android) {
				//listening for Purchase and consume events
				AndroidInAppPurchaseManager.ActionProductPurchased += OnProductPurchased;  
				AndroidInAppPurchaseManager.ActionProductConsumed  += OnProductConsumed;
				
				//listening for store initialising finish
				AndroidInAppPurchaseManager.ActionBillingSetupFinished += OnBillingConnected;
				
				//you may use loadStore function without parameter if you have filled base64EncodedPublicKey in plugin settings
				AndroidInAppPurchaseManager.Client.Connect();
				
			} else {
				
				if (!IsInitialized) {
				
					//You do not have to add products by code if you already did it in seetings guid
					//Windows -> IOS Native -> Edit Settings
					//Billing tab.
					// TODO: Add SKU Keys
					// Add SKU Keys in the Inspector.
//					Debug.LogError ("Adding IOSProductIds: " + IOSProductIds.Count);
//					foreach (var productId in IOSProductIds)
//					IOSInAppPurchaseManager.instance.AddProductId (productId);
				

					//Event Use Examples
					IOSInAppPurchaseManager.OnVerificationComplete += HandleOnVerificationComplete;
					IOSInAppPurchaseManager.OnStoreKitInitComplete += OnStoreKitInitComplete;
			
					IOSInAppPurchaseManager.OnTransactionComplete += OnTransactionComplete;
					IOSInAppPurchaseManager.OnRestoreComplete += OnRestoreComplete;

					IsInitialized = true;
				
				} 

				IOSInAppPurchaseManager.Instance.LoadStore ();
			}
		}
	

		//--------------------------------------
		//  PUBLIC METHODS
		//--------------------------------------

		public List<MarketProduct> GetProductsList ()
		{
			List<MarketProduct> marketProducts = new List<MarketProduct> ();
			if(Application.platform == RuntimePlatform.Android) {
				var products = AndroidInAppPurchaseManager.Client.Inventory.Products;
				foreach (var prod in products) {
					Debug.LogError("display name - "+prod.Title);
					
					marketProducts.Add (new MarketProduct (prod.SKU, prod.Title, prod.Price, prod.LocalizedPrice));
				}
			} else {
				var products = IOSInAppPurchaseManager.Instance.Products;
				foreach (var prod in products) {
					Debug.LogError("display name - "+prod.DisplayName);
					
					marketProducts.Add (new MarketProduct (prod.Id, prod.DisplayName, prod.Price, prod.LocalizedPrice));
				}
			}

			return marketProducts;
		}

		public static void buyItem (string productId)
		{
			//IOSInAppPurchaseManager.Instance.IsStoreLoaded;
			Debug.LogError("Trying to buy - "+productId);
			if(Application.platform == RuntimePlatform.Android) {
				AndroidInAppPurchaseManager.Client.Purchase (productId);
			} else {
				IOSInAppPurchaseManager.Instance.BuyProduct (productId);
			}
		}
		
		//--------------------------------------
		//  GET/SET
		//--------------------------------------
		public static bool IsStoreLoaded {
			get {
				if(Application.platform == RuntimePlatform.Android) {
					return AndroidInAppPurchaseManager.Client.IsInventoryLoaded;
				} else{
					return IOSInAppPurchaseManager.Instance.IsStoreLoaded;
				}
			}
		}

		//--------------------------------------
		//  EVENTS
		//--------------------------------------
		
		
		private static void UnlockProducts (string productIdentifier)
		{
			var widget = UIManager.Instance.GetScreen (GameUI.Market);
			if (widget != null) {
				var marketNode = (widget as Market).MarketNodes.Where ((item) => item.MarketProduct.ProductId.Equals (productIdentifier)).FirstOrDefault ();
				if (marketNode != null)
					DataBank.Instance.AddChipsOnServerAndClient (long.Parse (marketNode.ProductChips.text.Replace (",", "")));
			}

			Debug.LogError (productIdentifier + " Unlocked!!!");
		}
		
		private static void OnTransactionComplete (IOSStoreKitResult result)
		{
			
			Debug.Log ("OnTransactionComplete: " + result.ProductIdentifier);
			Debug.Log ("OnTransactionComplete: state: " + result.State);
			
			switch (result.State) {
	
			case InAppPurchaseState.Purchased:
#if  UNITY_EDITOR_OSX || UNITY_EDITOR
				Debug.LogError ("UNITY_EDITOR_OSX || UNITY_EDITOR");
				UnlockProducts (result.ProductIdentifier);
#elif UNITY_IPHONE || UNITY_IOS
				Debug.LogError ("UNITY_IOS || UNITY_IPHONE");
				IOSInAppPurchaseManager.Instance.VerifyLastPurchase (IOSInAppPurchaseManager.SANDBOX_VERIFICATION_SERVER);
#endif			
				break;
			case InAppPurchaseState.Restored:
				//Our product been succsesly purchased or restored
				//So we need to provide content to our user depends on productIdentifier
				UnlockProducts (result.ProductIdentifier);
				break;
			case InAppPurchaseState.Deferred:
				//iOS 8 introduces Ask to Buy, which lets parents approve any purchases initiated by children
				//You should update your UI to reflect this deferred state, and expect another Transaction Complete  to be called again with a new transaction state 
				//reflecting the parent’s decision or after the transaction times out. Avoid blocking your UI or gameplay while waiting for the transaction to be updated.
				break;
			case InAppPurchaseState.Failed:
				//Our purchase flow is failed.
				//We can unlock intrefase and repor user that the purchase is failed. 
				Debug.Log ("Transaction failed with error, code: " + result.Error.Code);
				Debug.Log ("Transaction failed with error, description: " + result.Error.Description);
				
				
				break;
			}
			
			if (result.State == InAppPurchaseState.Failed) {
//				if(Application.platform == RuntimePlatform.Android) {
//					AndroidMessage.Create("Transaction Failed", "Error code: " + result.Error.Code + "\n" + "Error description:" + result.Error.Description);
//				}else {
//					AndroidMessage.Create ("Transaction Failed", "Error code: " + result.Error.Code + "\n" + "Error description:" + result.Error.Description);
//				}
			} else {
				if(Application.platform == RuntimePlatform.Android) {
//					AndroidMessage.Create("Store Kit Response", "product " + result.ProductIdentifier + " state: " + result.State.ToString ());
				}else {
//					IOSNativePopUpManager.showMessage ("Store Kit Response", "product " + result.ProductIdentifier + " state: " + result.State.ToString ());
				}
			}
			
		}
		
		
		private static void OnRestoreComplete (IOSStoreKitRestoreResult res)
		{
			if (res.IsSucceeded) {
				Debug.LogError ("OnRestoreComplete");
				//IOSNativePopUpManager.showMessage ("Success", "Restore Compleated");
			} else {
				IOSNativePopUpManager.showMessage ("Error: " + res.Error.Code, res.Error.Description);
			}
		}	
		
		
		static void HandleOnVerificationComplete (IOSStoreKitVerificationResponse response)
		{
			IOSNativePopUpManager.showMessage ("Verification", "Transaction verification status: " + response.status.ToString ());

			if (response.status == 0) {
				Debug.Log ("Transaction is valid");
				UnlockProducts (response.productIdentifier);
			}

			Debug.Log ("ORIGINAL JSON: " + response.originalJSON);
		}
		
		
		private static void OnStoreKitInitComplete (ISN_Result result)
		{
			
			if (result.IsSucceeded) {
				
				int avaliableProductsCount = 0;
				foreach (IOSProductTemplate tpl in IOSInAppPurchaseManager.instance.Products) {
					if (tpl.IsAvaliable) {
						avaliableProductsCount++;
					}
				}
				
				//IOSNativePopUpManager.showMessage ("StoreKit Init Succeeded", "Available products count: " + avaliableProductsCount);
				Debug.LogError ("StoreKit Init Succeeded Available products count: " + avaliableProductsCount);
			} else {
				IOSNativePopUpManager.showMessage ("StoreKit Init Failed", "Error code: " + result.Error.Code + "\n" + "Error description:" + result.Error.Description);
			}
		}


		#region Android

		private static bool _isInited = false;

		private static void OnBillingConnected(BillingResult result) {
			AndroidInAppPurchaseManager.ActionBillingSetupFinished -= OnBillingConnected;
			
			if(result.IsSuccess) {
				//Store connection is Successful. Next we loading product and customer purchasing details
				AndroidInAppPurchaseManager.Client.RetrieveProducDetails();
				AndroidInAppPurchaseManager.ActionRetrieveProducsFinished += OnRetrieveProductsFinised;
			} 
			
//			AndroidMessage.Create("Connection Responce", result.Response.ToString() + " " + result.Message);
			Debug.Log ("Connection Responce: " + result.Response.ToString() + " " + result.Message);
		}

		private static void OnRetrieveProductsFinised(BillingResult result) {
			AndroidInAppPurchaseManager.ActionRetrieveProducsFinished -= OnRetrieveProductsFinised;
			
			
			if(result.IsSuccess) {
				_isInited = true;
				//TODO
//				AndroidMessage.Create("Success", "Billing init complete inventory contains: " + AndroidInAppPurchaseManager.Client.Inventory.Purchases.Count + " products");
				
				foreach(GoogleProductTemplate tpl in AndroidInAppPurchaseManager.Client.Inventory.Products) {
					Debug.Log(tpl.Title);
					Debug.Log(tpl.OriginalJson);
				}
			} else {
				//TODO
//				AndroidMessage.Create("Connection Response", result.Response.ToString() + " " + result.Message);
			}
			
			Debug.Log ("Connection Response: " + result.Response.ToString() + " " + result.Message);
			
		}

		private static void OnProductPurchased(BillingResult result) {
			if(result.IsSuccess) {
//				AndroidMessage.Create ("Product Purchased", result.Purchase.SKU+ "\n Full Response: " + result.Purchase.OriginalJson);
				OnProcessingPurchasedProduct (result.Purchase);
			} else {
//				AndroidMessage.Create("Product Purchase Failed", result.Response.ToString() + " " + result.Message);
			}
			
			Debug.Log ("Purchased Responce: " + result.Response.ToString() + " " + result.Message);
		}

		private static void OnProcessingPurchasedProduct(GooglePurchaseTemplate purchase) {
			UnlockProducts(purchase.SKU);
			AndroidInAppPurchaseManager.Client.Consume(purchase.SKU);

//			switch(purchase.SKU) {
//			case "":
//				AndroidInAppPurchaseManager.Client.Consume("");
//				break;
//
//			}
		}

		private static void OnProductConsumed (BillingResult result)
		{
			if (result.isSuccess) {
				OnProcessingConsumeProduct (result.purchase);
			} else {
//				AndroidMessage.Create ("Product Cousume Failed", result.response.ToString () + " " + result.message);
			}
			
			Debug.Log ("Cousume Responce: " + result.response.ToString () + " " + result.message);
		}

		private static void OnProcessingConsumeProduct (GooglePurchaseTemplate purchase)
		{
			

		}
		#endregion



	}
}