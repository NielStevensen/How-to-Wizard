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
			CompleteLevel();
		}
		else if(other.gameObject.layer == LayerMask.NameToLayer("VRPlayer"))
		{
			PickupSpell hand = other.GetComponent<PickupSpell>();
			VRMovement player = other.transform.GetComponentInParent<VRMovement>();

			//check if player and hand are in bounds. then complete level
			//if(player.)
			CompleteLevel();
		}
	}

	//Processes during level finish
	void CompleteLevel()
	{
		PlayerData data = SaveSystem.LoadGame();

		switch (Info.currentGameMode)
		{
			case (GameMode.Story):
				data.storyClearData[levelNumber] = true;
				data.storyClearTime[levelNumber] = Time.timeSinceLevelLoad;
				data.nGameBestSpell[levelNumber] = spellsCasted;

				break;
			case (GameMode.NewGamePlus):
				data.nGameClearData[levelNumber] = true;
				data.nGameClearTime[levelNumber] = Time.timeSinceLevelLoad;
				data.nGameBestSpell[levelNumber] = spellsCasted;

				break;
			case (GameMode.Challenge):
				data.extraClearData[levelNumber] = true;
				data.extraClearTime[levelNumber] = Time.timeSinceLevelLoad;
				data.extraBestSpell[levelNumber] = spellsCasted;

				break;
		}

		SaveSystem.SaveGame(data);

		if(nextLevel != "")
		{
			SceneManager.LoadScene(nextLevel);
		}
	}
}
