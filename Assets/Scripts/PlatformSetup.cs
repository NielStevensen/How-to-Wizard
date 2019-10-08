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

	//Setup target platform if it is not already setup. Set up cursor and destroy objects based on platform
	private void Awake()
    {
		if (Info.targetPlatform == Platform.Unset)
		{
			if(targetPlatform == Platform.Unset)
			{
				targetPlatform = Platform.PC;
			}

			Info.targetPlatform = targetPlatform;
		}

		if (Info.IsCurrentlyVR())
		{
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            foreach (PCCraftingZone obj in FindObjectsOfType<PCCraftingZone>())
			{
				destroyOnVR.Add(obj.gameObject);
			}

			foreach(SpellCreation table in FindObjectsOfType<SpellCreation>())
			{
				foreach(AttachCrystal obj in table.PCSpellSlots)
				{
					destroyOnVR.Add(obj.gameObject);
				}
			}

			PlayerController player = FindObjectOfType<PlayerController>();

			if(player != null)
			{
				destroyOnVR.Add(player.gameObject);
			}
			
			foreach(GameObject obj in destroyOnVR)
			{
				Destroy(obj);
			}
		}
		else
		{
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            foreach (DestroyCrstalRange obj in FindObjectsOfType<DestroyCrstalRange>())
			{
				destroyOnPC.Add(obj.gameObject);
			}

			foreach (SpellCreation table in FindObjectsOfType<SpellCreation>())
			{
				foreach (AttachCrystal obj in table.VRSpellSlots)
				{
					destroyOnPC.Add(obj.gameObject);
				}
			}

			VRMovement player = FindObjectOfType<VRMovement>();

			if (player != null)
			{
				destroyOnPC.Add(player.gameObject);
			}

			foreach (GameObject obj in destroyOnPC)
			{
				Destroy(obj);
			}
		}

		if(Info.optionsData == null)
		{
			Info.optionsData = SaveSystem.LoadOptions();
		}
    }
}
