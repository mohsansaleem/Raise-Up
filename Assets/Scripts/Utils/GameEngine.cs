using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using TienLen.Core.Models;
using TienLen.Core.Misc;
using TienLen.Utils;
using TienLen.Utils.Enums;
using TienLen.Utils.Constants;
using TienLen.Utils.Helpers;
using System.Threading;
using Client.Utils.Enums;
using Newtonsoft.Json;
using Client.Utils;

abstract public class GameEngine 
{
	public GameType gameType;
	public IGame game;
	public Player[] players;
}
