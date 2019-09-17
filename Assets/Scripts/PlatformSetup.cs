using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformSetup : MonoBehaviour
{
	//The target platform
	[Tooltip("The target platform for this build.\nSet this in the menu to set the target platform for the build." +
		"\nSet this in individual levels to set the platform for that level. This will not affect builds." +
		"\nUnset will default to PC.")]
	public Platform targetPlatform = Platform.Unset;

	[Space(10)]

	//Objects to destroy on particular platforms
	[Tooltip("Objects to destroy if the target platform is PC.")]
	public List<GameObject> destroyOnPC = new List<GameObject>();
	[Tooltip("Objects to destroy if the target platform is VR.")]
	public List<GameObject> destroyOnVR = new List<GameObject>();

	//Setup target platform if it is not already setup. Destroy objects based on platform
	void Start()
    {
        if(Info.platform == Platform.Unset)
		{
			if(targetPlatform == Platform.Unset)
			{
				targetPlatform = Platform.PC;
			}

			Info.platform = targetPlatform;
		}

		if (Info.IsCurrentlyVR())
		{
			foreach (PCCraftingZone obj in GameObject.FindObjectsOfType<PCCraftingZone>())
			{
				destroyOnVR.Add(obj.gameObject);
			}

			foreach(GameObject obj in destroyOnVR)
			{
				Destroy(obj);
			}
		}
		else
		{
			foreach (DestroyCrstalRange obj in GameObject.FindObjectsOfType<DestroyCrstalRange>())
			{
				destroyOnPC.Add(obj.gameObject);
			}

			foreach (GameObject obj in destroyOnPC)
			{
				Destroy(obj);
			}
		}
    }
}
