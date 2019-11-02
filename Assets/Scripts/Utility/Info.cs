using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Platform { Unset = -1, VR = 0, PC = 1 }
public enum GameMode { Unset = -1, Story = 0, NewGamePlus = 1, Challenge = 2, Sandbox = 3}

public static class Info
{
	//Current target platform
	public static Platform targetPlatform = Platform.Unset;
	
	//Determine whether or not the target platform is VR
	public static bool IsCurrentlyVR()
	{
		if(targetPlatform == Platform.Unset)
		{
			return false;
		}

		return targetPlatform == Platform.VR;
	}

	//Pause status
	public static bool isPaused = false;

	//Toggle pause state
	public static void TogglePause()
	{
		isPaused = !isPaused;

		Time.timeScale = isPaused ? 0.0f : 1.0f;
	}

	//Current game mode
	public static GameMode currentGameMode = GameMode.Unset;

	//Options
	public static OptionsData optionsData = null;
}
