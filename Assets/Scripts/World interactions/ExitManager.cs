using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitManager : MonoBehaviour
{
	//Level values
	[Tooltip("Level number.")]
	public int levelNumber = 1;
	[Tooltip("The next level to load.")]
	public string nextLevel = "";

	[Space(10)]

	//Number of spells casted
	[Tooltip("Number of spells casted.")]
	public int spellsCasted = 0;

	//Detect players finishing the level
	private void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.layer == LayerMask.NameToLayer("Player"))
		{
			StartCoroutine(CompleteLevel());
		}
		else if(other.gameObject.layer == LayerMask.NameToLayer("Hands"))
		{
			PickupSpell hand = other.GetComponent<PickupSpell>();

			if(hand != null)
			{
				if (hand.isHandInArea)
				{
					StartCoroutine(CompleteLevel());
				}
			}
		}
	}

	//Processes during level finish
	IEnumerator CompleteLevel()
	{
		int levelIndex = levelNumber - 1;

        PlayerData data = SaveSystem.LoadGame();

		switch (Info.currentGameMode)
		{
			case (GameMode.Story):
				data.storyClearData[levelIndex] = true;
				data.storyClearTime[levelIndex] = data.storyClearTime[levelIndex] == -1 ? Time.timeSinceLevelLoad : Mathf.Min(data.storyClearTime[levelIndex], Time.timeSinceLevelLoad);
				data.storyBestSpell[levelIndex] = data.storyBestSpell[levelIndex] == -1 ? spellsCasted : Mathf.Min(data.storyBestSpell[levelIndex], spellsCasted);

                break;
			case (GameMode.NewGamePlus):
				data.nGameClearData[levelIndex] = true;
				data.nGameClearTime[levelIndex] = data.nGameClearTime[levelIndex] == -1 ? Time.timeSinceLevelLoad : Mathf.Min(data.nGameClearTime[levelIndex], Time.timeSinceLevelLoad);
                data.nGameBestSpell[levelIndex] = data.nGameBestSpell[levelIndex] == -1 ? spellsCasted : Mathf.Min(data.nGameBestSpell[levelIndex], spellsCasted);

                break;
			case (GameMode.Challenge):
				data.extraClearData[levelIndex] = true;
				data.extraClearTime[levelIndex] = data.extraClearTime[levelIndex] == -1 ? Time.timeSinceLevelLoad : Mathf.Min(data.extraClearTime[levelIndex], Time.timeSinceLevelLoad);
                data.extraBestSpell[levelIndex] = data.extraBestSpell[levelIndex] == -1 ? spellsCasted : Mathf.Min(data.extraBestSpell[levelIndex], spellsCasted);

                break;
		}

		SaveSystem.SaveGame(data);

		yield return Info.IsCurrentlyVR() ? FindObjectOfType<VRMovement>().HandleLevelEndFadeVR() : FindObjectOfType<PlayerController>().HandleLevelEndFadePC();
		
		GetComponent<ManualTiling>().ClearMaterials();

		if(nextLevel != "")
		{
			if(nextLevel == "Menu")
			{
				SceneManager.LoadScene(Info.IsCurrentlyVR() ? "VR Menu" : "PC Menu");
			}
			else
			{
				SceneManager.LoadScene(nextLevel);
			}
		}
	}

	private void OnApplicationQuit()
	{
		GetComponent<ManualTiling>().ClearMaterials();
	}
}
