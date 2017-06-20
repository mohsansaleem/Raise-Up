using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TienLen.Core.Models;
using TienLen.Utils.Constants;
using TienLen.Utils.Helpers;
using TienLen.Utils.Enums;
using Newtonsoft.Json;

namespace TienLen.Core.GamePlay
{
	public class TienLenAI : IComputer
	{
		AIType _AIType;

		public List<Card>[] cardList = new List<Card>[GameConstants.CARDS_PER_PLAYER];

		public TienLenAI ()
		{
			_AIType = AIType.Hard;
		}

		public TienLenAI (AIType _ai)
		{
			_AIType = _ai;
		}

		override public void Initialised (List<Card> aiPlayerCards, IGame game)
		{
			for (int i = 0; i < GameConstants.CARDS_PER_PLAYER; i++) {
				int amount = aiPlayerCards.FindAll (delegate(Card card) {
					return Helper.GetGCV (game, card.Value) == Helper.GetGCV (game, (Value)i + 1);
				}).Count;
				cardList [i] = new List<Card> (amount);
			}
			
			foreach (Card card in aiPlayerCards)
				cardList [Helper.GetGCV (game, card.Value) - 1].Add (card);
			
			for (int i = 0; i < GameConstants.CARDS_PER_PLAYER; i++) {
				if (cardList [i].Count != 0)
					Helper.SortCards (game, cardList [i]);
			}
		}
		
//		public string Name()
//		{
//			return "Computer (Easy)";
//		}

		// TODO change multiple AI (easy, medium, hard) change here

		override public List<Card> Process (List<Card> currentCards, IGame game, Player player)
		{
			List<Card> cardSend = new List<Card> ();
		
			// For testing
//			if (((TienLenGame)game).IsLowestCardDone == true) {
//				return cardSend;
//			}
			// For Testing

			switch (((TienLenGame)game).FieldType) {
			case FieldType.Single:

				if (currentCards [0].Value == Value.Two) {
					//List<Card> crdLst = player. (3, cardList, game, null, true);

					bool exist = player.IfFourOfKindExist (cardList, game);

					if (exist) {
						foreach (var crdlst in cardList) {
							if (crdlst.Count == 4) {
								foreach (Card card in crdlst)
									cardSend.Add (card);
								crdlst.Clear ();
								break;
							}
						}
						((TienLenGame)game).FieldType = FieldType.FourKind;
					} else {
						List<Card> crdLst = player.GetRelativePairSequence (3, cardList, game, null, true);

						if (crdLst.Count != 0) {
							foreach (Card card in crdLst)
								cardSend.Add (card);
							((TienLenGame)game).FieldType = FieldType.Bomb;
						}
					}
				}


				if (cardSend.Count == 0) {
					for (int i = Helper.GetGCV(game, currentCards[0].Value) - 1; i < GameConstants.CARDS_PER_PLAYER; i++) {
						if (cardList [i].Count == 1 &&
							(Helper.GetGCV (game, cardList [i] [0].Value) > Helper.GetGCV (game, currentCards [0].Value) ||
							(i + 1 == Helper.GetGCV (game, currentCards [0].Value) && cardList [i] [0].Suite > currentCards [0].Suite))) {
							cardSend.Add (cardList [i] [0]);
							cardList [i].Remove (cardList [i] [0]);
							break;
						}
					}
				}
				break;
				
			case FieldType.Double:
			case FieldType.Triple:
			case FieldType.FourKind:
				if (currentCards [0].Value == Value.Two || currentCards.Count == 4) {
					List<Card> bombLst = player.GetRelativePairSequence ((currentCards [0].Value == Value.Two ? 2 : 0) + currentCards.Count, cardList, game, null, true);

					if (bombLst.Count != 0) {
						foreach (Card card in bombLst)
							cardSend.Add (card);
						((TienLenGame)game).FieldType = FieldType.Bomb;
					}
				}

				if (cardSend.Count == 0) {
					for (int i = Helper.GetGCV(game, currentCards[0].Value); i < GameConstants.CARDS_PER_PLAYER; i++) {
						if (cardList [i].Count == (int)((TienLenGame)game).FieldType &&
							Helper.GetSuiteSum (cardList [i]) >= Helper.GetSuiteSum (currentCards)) {
							foreach (Card card in cardList[i])
								cardSend.Add (card);
						
							cardList [i].Clear ();
							break;
						}
					}
				}
				
				break;
			case FieldType.Bomb:
				List<Card> bmbLst = player.GetRelativePairSequence (currentCards.Count / 2, cardList, game, currentCards [currentCards.Count - 1], true);
				
				foreach (Card card in bmbLst)
					cardSend.Add (card);
                    
				break;
			case FieldType.Sequence:
				List<Card> seqLst = player.GetRelativeSequence (currentCards.Count, cardList, game, currentCards [currentCards.Count - 1], true);
			
				foreach (Card card in seqLst)
					cardSend.Add (card);
			
				break;
			case FieldType.None:

				int tempIndex = -1;

				Debug.LogError ("FieldType.None");
				if (_AIType == AIType.Easy) {
					for (int i = 0; i < GameConstants.CARDS_PER_PLAYER; i++) {
						if (cardList [i].Count != 0) {
							((TienLenGame)game).FieldType = (FieldType)cardList [i].Count;
						
							foreach (Card card in cardList[i])
								cardSend.Add (card);
						
							cardList [i].Clear ();
							tempIndex = i;
							i = GameConstants.CARDS_PER_PLAYER; // to break the loop
						}
					}
				} else {

//					for (int i = 0; i < GameConstants.CARDS_PER_PLAYER; i++) {
//						if (cardList [i].Count != 0) {
//							((TienLenGame)game).FieldType = (FieldType)cardList [i].Count;
//						
//							foreach (Card card in cardList[i])
//								cardSend.Add (card);
//						
//							cardList [i].Clear ();
//							i = GameConstants.CARDS_PER_PLAYER; // to break the loop
//						}
//					}
					for (int i = Helper.GetGCV(game, game.LowestCard().Value); i < GameConstants.CARDS_PER_PLAYER; i++) {

						if (currentCards == null || currentCards.Count == 0) {
							currentCards = new List<Card> ();
							currentCards.Add (game.LowestCard ());
						}

						if ((cardList [i].Count == (int)FieldType.Double &&
							Helper.GetSuiteSum (cardList [i]) >= Helper.GetSuiteSum (currentCards)) ||
							(cardList [i].Count == (int)FieldType.Triple &&
							Helper.GetSuiteSum (cardList [i]) >= Helper.GetSuiteSum (currentCards)) ||
							(cardList [i].Count == (int)FieldType.Sequence &&
							Helper.GetSuiteSum (cardList [i]) >= Helper.GetSuiteSum (currentCards)) ||
							(cardList [i].Count == (int)FieldType.FourKind &&
							Helper.GetSuiteSum (cardList [i]) >= Helper.GetSuiteSum (currentCards))
						    ) {

							if (cardList [i].Count != 0) {
								((TienLenGame)game).FieldType = (FieldType)cardList [i].Count;

//								Debug.LogError ("FieldType: " + ((TienLenGame)game).FieldType.ToString ());

								foreach (Card card in cardList[i])
									cardSend.Add (card);
								
								cardList [i].Clear ();
								tempIndex = i;
								i = GameConstants.CARDS_PER_PLAYER; // to break the loop
							}

						} 
					}

					// AI has 3 of Spade and it didn't choose the combination containing 3 of Spade the clear the cards.
//					if (((TienLenGame)game).IsLowestCardDone == false &&
//						cardSend.Find (delegate(Card card) { 
//						return card.Value == ((TienLenGame)game).LowestCard ().Value && card.Suite == ((TienLenGame)game).LowestCard ().Suite; 
//					}) == null) {
//						if (tempIndex != -1)
//							cardList [tempIndex] = cardSend;
//						cardSend = new List<Card> ();
//					}

					if (cardSend.Count == 0) {
						for (int i = 0; i < GameConstants.CARDS_PER_PLAYER; i++) {
							if (cardList [i].Count != 0) {
								if(( ((TienLenGame)game).FieldType == FieldType.None ) || (( (TienLenGame)game).FieldType == (FieldType)cardList [i].Count) )
								{
								((TienLenGame)game).FieldType = (FieldType)cardList [i].Count;
								
								foreach (Card card in cardList[i])
									cardSend.Add (card);
								
								cardList [i].Clear ();
								i = GameConstants.CARDS_PER_PLAYER; // to break the loop
							}
						
							}
						}
					}
				}
				break;
			}

//			if (cardSend.Find (delegate(Card card) { 
//				return card.Value == ((TienLenGame)game).LowestCard ().Value && card.Suite == ((TienLenGame)game).LowestCard ().Suite; 
//			}) != null)
//			if (cardSend.Count != 0 && !((TienLenGame)game).IsLowestCardDone)
//				((TienLenGame)game).IsLowestCardDone = true;


			if (cardSend.Count == 0) {
				Debug.LogError ("Field Type: " + ((TienLenGame)game).FieldType);
				Debug.LogError ("Current Cards: " + JsonConvert.SerializeObject (currentCards));
				Debug.LogError ("AI Cards: " + JsonConvert.SerializeObject (cardList));
			}

			return cardSend;
		}
	}
}