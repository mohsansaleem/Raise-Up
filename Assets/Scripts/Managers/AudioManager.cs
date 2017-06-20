using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DarkTonic.MasterAudio;

namespace Game
{
	/// <summary>
	/// Background music type.
	/// </summary>
	public enum BackgroundMusicType
	{
		MainTheme,
		VictoryTheme,
		LogoTheme
	}

	public enum SoundType
	{
		LogIn,
		PlayerJoining,
		ScreenChange,
		TurnNotification,
		CardPlayed

	}

	/// <summary>
	/// Audio Manager.
	/// </summary>
	public class AudioManager : Singleton<AudioManager>
	{
		const string tapSound = "tapSound";

		/// <summary>
		/// The Name of the background playlist.
		/// </summary>
		const string BackgroundPlaylistName = "BackGroundMusic";

		/// <summary>
		/// The Game Sounds List.
		/// </summary>
		Dictionary<string,string> SoundsList = new Dictionary<string, string> ();

		/// <summary>
		/// The Background Music list.
		/// </summary>
		Dictionary<string,string> MusicList = new Dictionary<string, string> ();

		protected void Awake ()
		{
			//base.Awake ();

			// TODO: Add the respective Background SoundNames here after adding in the Audio Manager.
			// Before Adding any Background Music in this list, add it to the BackgroundMusic PlayList.
			MusicList.Add (BackgroundMusicType.MainTheme.ToString (), "Main Theme");
			MusicList.Add (BackgroundMusicType.VictoryTheme.ToString (), "game end sound");
			MusicList.Add (BackgroundMusicType.LogoTheme.ToString (), "logo screen");


			// Tap Sound
			SoundsList.Add (tapSound, "Menu Tap");
			SoundsList.Add (SoundType.LogIn.ToString (), "login");
			SoundsList.Add (SoundType.PlayerJoining.ToString (), "player joining game");
			SoundsList.Add (SoundType.ScreenChange.ToString (), "switching screen");
			SoundsList.Add (SoundType.CardPlayed.ToString (), "CardPlayed");
			SoundsList.Add (SoundType.TurnNotification.ToString (), "Chinese Bell1");
		}

		/// <summary>
		/// Plays the menu tap.
		/// </summary>
		public void PlayMenuTap ()
		{
			MasterAudio.PlaySound (SoundsList [tapSound]);
		}

		/// <summary>
		/// Play the Background Music.
		/// </summary>
		/// <param name="backgroundMusicType">Background Music Type.</param>
		public void PlayBackgroundMusic (BackgroundMusicType backgroundMusicType)
		{
			Debug.LogError ("PlayBackgroundMusic: " + backgroundMusicType.ToString ());
			MasterAudio.TriggerPlaylistClip (BackgroundPlaylistName, MusicList [backgroundMusicType.ToString ()]);
		}

		public void StopBackgroundMusic ()
		{
			Debug.LogError ("StopBackgroundMusic");
			MasterAudio.StopPlaylist (BackgroundPlaylistName);
		}

		/// <summary>
		/// Play the Sound on the Specific Events like Attack, Damage, Death etc.
		/// </summary>
		/// <param name="objectName">Name of the Building or the Unit</param>
		/// <param name="soundType">Sound Type like Death, Damage, Attack etc.</param>
		public void PlaySound (SoundType soundType)
		{
//			Debug.Log ("PlaySound: " + objectName + "   sType: " + snd);
			//if (MasterAudio.SoundsReady)
			MasterAudio.PlaySound (SoundsList [soundType.ToString ()]);
		}

		public void PlaySoundWithDelay(SoundType soundType, float delay) {
			this.PerformActionWithDelay(delay, ()=> {
				MasterAudio.PlaySound (SoundsList [soundType.ToString ()]);
			});
		}

		public void StopGeneralSound ()
		{
			MasterAudio.StopBus ("GeneralSounds");
		}
		public void UnmuteBackgroundMusic ()
		{
			//MasterAudio.PlaylistMasterVolume=1;
			MasterAudio.UnmutePlaylist ();


		}

		public void MuteBackgroundMusic ()
		{
			//MasterAudio.ToggleMutePlaylist();
			MasterAudio.MutePlaylist ();

		}

		public void MuteSoundEffects ()
		{
			MasterAudio.MuteBus ("GeneralSounds");
		}

		public void UnmuteSoundEffects ()
		{
			MasterAudio.UnmuteBus ("GeneralSounds");
		}

		void Start ()
		{
			//PlayBackgroundMusic (BackgroundMusicType.MainTheme);
		}
	}
}
