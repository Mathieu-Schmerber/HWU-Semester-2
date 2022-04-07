using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Gamemode : Singleton<Gamemode>
{
    public enum Mode
	{
		WOLF,
		HUNTER,
		AI_ONLY
	}

	private Mode _currentMode;

	public Mode CurrentMode
	{
		get => _currentMode;
		set {
			GamemodeSwitch[] switches = GameObject.FindObjectsOfType<GamemodeSwitch>();

			_currentMode = value;
			foreach (var item in switches)
				item.ApplyGamemode(_currentMode);
		}
	}

}
