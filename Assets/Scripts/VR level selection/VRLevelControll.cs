using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class VRLevelControll : MonoBehaviour
{
	#region Variables
	//UI panels
	public GameObject[] panels;

	[Space(10)]

	//Level selection variables
    public int selectedLevel = 1;

	[Space(10)]

    //Stats display
    public Sprite[] levelPics;
    public PlayerData stats;
    public Image display;
    public Text time;
    public Text spells;

	#endregion

	//Load data and initialise options
	void Start()
    {
		stats = SaveSystem.LoadGame();
        Info.currentGameMode = GameMode.Story;
        DisplayStats();
    }
	

	#region Level Select

    public void Cycle(int distsance)
    {
        if (distsance >= 1)
        {
            if(stats.storyClearData[(selectedLevel - 1) % stats.storyClearData.Length])
            {
                Debug.Log("Text");
                selectedLevel += 1;
            }
            //else
            //{
            //    selectedLevel = 1;
            //}
        }
        else if(distsance <= 1)
        {
            if(selectedLevel > 1)
            {
                selectedLevel -= 1;
            }
            //else
            //{
            //    for(int i = stats.storyClearData.Length; i > 0; --i)
            //    {
            //        selectedLevel = i;
            //        if (stats.storyClearData[i])
            //        {
            //            break;
            //        }
            //    }
            //}
        }
        display.sprite = levelPics[selectedLevel - 1];
        DisplayStats();
    }

	//Display the stats for the currently selected level
	public void SetLevel(int levelIndex)
    {
		if (selectedLevel == levelIndex)
		{
			LoadLevel();

			return;
		}

		selectedLevel = levelIndex;

		DisplayStats();
    }

	//Determine which stats to display
	void DisplayStats()
	{
		int selectedIndex = selectedLevel -1;
        

        if (selectedIndex < 0)
		{
			return;
		}

		switch (Info.currentGameMode)
		{
			case (GameMode.Story):
				DisplayStats(stats.storyClearData[selectedIndex], stats.storyClearTime[selectedIndex], stats.storyBestSpell[selectedIndex]);
                break;
			case (GameMode.NewGamePlus):
				DisplayStats(stats.nGameClearData[selectedIndex], stats.nGameClearTime[selectedIndex], stats.nGameBestSpell[selectedIndex]);

				break;
			case (GameMode.Challenge):
				DisplayStats(stats.extraClearData[selectedIndex], stats.extraClearTime[selectedIndex], stats.extraBestSpell[selectedIndex]);

				break;
			case (GameMode.Sandbox):
				DisplayStats(false, -1, -1);

				break;
		}
	}

	//Display level stats
	void DisplayStats(bool isCleared, float clearTime, int spellCount)
	{
		if (isCleared)
		{
			time.text = "Best time: " + FormatTime(clearTime);
			spells.text = "Least spells: " + spellCount;
		}
		else
		{
			time.text = "Best time: - : --";
			spells.text = "Least spells: --";
		}
	}

	//Format time as a string
    public string FormatTime(float time)
    {
        int minutes = (int)time / 60;
        int seconds = (int)time / 1 - 60 * minutes;

        return string.Format("{0:0}:{1:00}", minutes, seconds);
    }

	//Load the selected level
    public void LoadLevel()
    {
        if(selectedLevel != -1)
        {
			if (Info.currentGameMode == GameMode.Story || Info.currentGameMode == GameMode.NewGamePlus)
            {
                SceneManager.LoadScene("Level " + selectedLevel);
            }
			else if(Info.currentGameMode == GameMode.Challenge)
			{
				//SceneManager.LoadScene("Level " + selectedLevel);
			}
			else if(Info.currentGameMode == GameMode.Sandbox)
			{
				//SceneManager.LoadScene("Sandbox");
			}
        }
    }
	#endregion

	#region Quit
	//Quit the game
	public void Quit()
	{
		Application.Quit();
	}
	#endregion
}
