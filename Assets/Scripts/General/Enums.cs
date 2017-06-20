using UnityEngine;
using System.Collections;

namespace Game.Enums
{
	public enum GameUI
	{
		Signup, 
		Login,
		Hud,
		GameMenu,
		Lobby,
		TienLenClient,
		UserProfile,
		Settings,
		Friends,
		Market
	}

	/// <summary>
	/// Previous screen visibility.
	/// </summary>
	public enum PreviousScreenVisibility
	{
		/// <summary>
		/// Do Not Dequeue or Hide Previous Screen.
		/// </summary>
		DoNothing,

		/// <summary>
		/// Dequeue Previous Screen, but if the Previous Screen is Hud then do not Dequeue it.
		/// </summary>
		DequeuePreviousExcludingHud,

		/// <summary>
		/// Dequeue Previous Screen.
		/// </summary>
		DequeuePreviousIncludingHud,
		
		/// <summary>
		/// Hide Previous Screen, but if the Previous Screen is Hud then do not Hide it.
		/// </summary>
		HidePreviousExcludingHud,
		
		/// <summary>
		/// Hide Previous Screen.
		/// </summary>
		HidePreviousIncludingHud
	}

	public enum PopupType
	{
		LoadingPopup,
		OneButton,
		TwoButton,
		ThreeButton
	}
}
