using UnityEngine;
using System.Collections;
using Game.Managers;

public class UIManagerSample : MonoBehaviour
{
	// Use this for initialization
	void Start ()
	{
		UIManager.Instance.ShowUI (Game.Enums.GameUI.Hud);
	}
}
