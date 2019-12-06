using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlatformSetup : MonoBehaviour
{
	//The target platform
	[Tooltip("The target platform for this build.\nSet this in the menu to set the target platform for the build." +
		"\nSet this in individual levels to set the platform for that level. This will not affect builds." +
		"\nUnset will default to PC.")]
	public Platform targetPlatform = Platform.Unset;

    public GameObject bgmControll;
    public AudioClip[] clips;

	[Space(10)]

	//Objects to destroy on particular platforms
	[Tooltip("Objects to destroy if the target platform is PC.")]
	public List<GameObject> destroyOnPC = new List<GameObject>();
	[Tooltip("Objects to destroy if the target platform is VR.")]
	public List<GameObject> destroyOnVR = new List<GameObject>();

	//Setup target platform if it is not already setup. Set up cursor and destroy objects based on platform
	private void Awake()
    {
        AudioSource[] sources = GameObject.FindObjectsOfType<AudioSource>();
        PersistentBGM bgm = FindObjectOfType<PersistentBGM>();

        if(bgm == null)
        {
            bgm = Instantiate(bgmControll).GetComponent<PersistentBGM>();
        }
        
        if(SceneManager.GetActiveScene().name.Contains("Menu"))
        {
            if (bgm.GetComponent<AudioSource>().clip != clips[0])
            {
                bgm.GetComponent<AudioSource>().clip = clips[0];
                bgm.GetComponent<AudioSource>().Play();
            }
        }
        else if(Info.currentGameMode == GameMode.Challenge)
        {
            if (bgm.GetComponent<AudioSource>().clip != clips[1])
            {
                bgm.GetComponent<AudioSource>().clip = clips[1];
                bgm.GetComponent<AudioSource>().Play();
            }
        }
        else
        {
            if (bgm.GetComponent<AudioSource>().clip != clips[2])
            {
                bgm.GetComponent<AudioSource>().clip = clips[2];
                bgm.GetComponent<AudioSource>().Play();
            }

        }

        if (Info.optionsData != null)
        {
            bgm.GetComponent<AudioSource>().volume = Info.optionsData.bgmLevel;
        }
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
			if(SceneManager.GetActiveScene().name != "PC Menu")
			{
				Cursor.lockState = CursorLockMode.Locked;
				Cursor.visible = false;
			}
			else
			{
				Cursor.lockState = CursorLockMode.None;
				Cursor.visible = true;
			}

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
