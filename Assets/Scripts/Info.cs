using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Platform { Unset = -1, VR = 0, PC = 1 }

public static class Info
{
	//Current target platform
	public static Platform platform = Platform.Unset;
	
	//Determine whether or not the target platform is VR
	public static bool IsCurrentlyVR()
	{
		if(platform == Platform.Unset)
		{
			return false;
		}

		return platform == Platform.VR;
	}
}
