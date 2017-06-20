using System.Collections.Generic;
using System;
using TienLen.Utils.Enums;

namespace TienLen.Core.Models
{
	/// <summary>
	/// AI Player interface.
	/// </summary>
	public class IComputer
	{
		/// <summary>
		/// Name of the computer.
		/// </summary>
		//string Name();
		
		/// <summary>
		/// Called when it is the AI's turn, thus must choose the cards to play on the field.
		/// </summary>
		/// <param name="currentCards">The cards the AI has currently.</param>
		/// <param name="game">The current game.</param>
		public virtual List<Card> Process (List<Card> currentCards, IGame game, Player player)
		{
			return null;
		}
		
		/// <summary>
		/// When the AI's cards are ready.
		/// </summary>
		/// <param name="aiPlayer">The AI player.</param>
		/// <param name="game">The current game.</param>
		public virtual void Initialised (List<Card> aiPlayerCards, IGame game)
		{

		}
	}
	
	/// <summary>
	/// Card game interface.
	/// </summary>
	public interface IGame
	{
		/// <summary>
		/// Called when the player has confirmed his/her cards. This function will determine if it is valid or not.
		/// </summary>
		/// <param name="chosenCards">The cards the player has selected.</param>
		/// <param name="currentCards">The cards currently on the field.</param>
		bool ValidateCards (List<Card> chosenCards, List<Card> currentCards);
		/// <summary>
		/// Called when the game has been loaded. You can add your events here.
		/// </summary>
		void Initialised (GameInfo info);
		
		// TODO function to override
		/// <summary>
		/// Called when the player presses the Options button in the game selection screen.
		/// </summary>
		/// <param name="parentFrm"></param>
		//void LoadOptions(System.Windows.Forms.Form parentFrm);
		//void LoadOptions();
		
		// TODO see if used
		/// <summary>
		/// The image overview of the game. It is displayed in the game selection screen.
		/// </summary>
		//Image ImageOverview();
		
		/// <summary>
		/// If the options button should be visible.
		/// </summary>
		//bool AllowOptions();
		/// <summary>
		/// Name of the game.
		/// </summary>
		//string Name();
		/// <summary>
		/// Description of the game. It is displayed in the game selection screen.
		/// </summary>
		//string Description();
		/// <summary>
		/// The maxmimum amount of players the game can take.
		/// </summary>
		int MaxPlayers ();
		/// <summary>
		/// The minimum amount of players the game can take.
		/// </summary>
		int MinPlayers ();
		/// <summary>
		/// The lowest card in the game.
		/// </summary>
		Card LowestCard ();
		/// <summary>
		/// The highest card in the game.
		/// </summary>
		Card HighestCard ();

		bool IsLowestCard (Card card);

		int GetFirstPlayerTurn ();

		GameInfo GetGameInfo ();
	}
}