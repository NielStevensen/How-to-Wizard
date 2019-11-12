using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PCUIController : MonoBehaviour
{
	#region Variables
	//UI panels
	public GameObject[] panels;

	[Space(10)]

	//Level selection variables
    public int selectedLevel = -1;
	public Button[] levelButtons;
	public Button[] modeButtons;
	public Button startButton;

	[Space(10)]

	//Stats display
	public PlayerData stats;
    public Text time;
    public Text spells;

	[Space(10)]

	//Options
	public OptionsData options;
    public Slider xSens;
    public Slider ySens;
    public Toggle yInversion;
    public Slider BGMVolume;
    public Slider SFXVolume;
	#endregion

	//Load data and initialise options
	void Start()
    {
		stats = SaveSystem.LoadGame();
        options = SaveSystem.LoadOptions();
		
		ChangeGameMode(Info.currentGameMode == GameMode.Unset ? (int)GameMode.Story : (int)Info.currentGameMode);

		if (stats.storyClearData[14])
		{
			foreach(Button button in modeButtons)
			{
				button.interactable = true;
			}
		}

		if (Info.optionsData == null)
		{
			Info.optionsData = options;
		}

        xSens.value = options.xSens;
        ySens.value = options.ySens;
        yInversion.isOn = options.yInversion;
        BGMVolume.value = options.bgmLevel;
        SFXVolume.value = options.sfxLevel;
    }
	
	//Change the panel displayed
    public void ChangePanel(int panelIndex)
    {
        for (int i = 0; i < panels.Length; i ++)
        {
            if (i == panelIndex) panels[i].SetActive(true);
            else panels[i].SetActive(false);
        }
    }

	#region Level Select
	//Change the current game mode
	public void ChangeGameMode(int mode)
	{
		Info.currentGameMode = (GameMode)mode;
		
		switch (Info.currentGameMode)
		{
			case (GameMode.Story):
				ChangeAvailability(stats.storyClearData);

				break;
			case (GameMode.NewGamePlus):
				for (int i = 0; i < levelButtons.Length; i++)
				{
					levelButtons[i].interactable = true;
				}

				break;
			case (GameMode.Challenge):
				ChangeAvailability(stats.extraClearData);

				break;
			case (GameMode.Sandbox):
				for (int i = 0; i < levelButtons.Length; i++)
				{
					levelButtons[i].interactable = false;
				}

				break;
		}

		DisplayStats();
	}

	//Change which levels are available
	void ChangeAvailability(bool[] clearData)
	{
		bool shouldNextBeAvailable = true;
		
		for (int i = 0; i < clearData.Length; i++)
		{
			levelButtons[i].interactable = true;
			//levelButtons[i].interactable = clearData[i] ? true : DetermineAvailability(ref shouldNextBeAvailable);
		}
	}

	//Determine if an uncleared level should be available (because the previous was cleared)
	bool DetermineAvailability(ref bool availability)
	{
		if (availability)
		{
			availability = false;

			return true;
		}
		else
		{
			return false;
		}
	}

	//Display the stats for the currently selected level
	public void SetLevel(int levelIndex)
    {
		startButton.interactable = true;

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
		int selectedIndex = selectedLevel - 1;

		if(selectedIndex < 0)
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

	#region Options
	//Apply settings
	public void ApplySettings()
    {
        options.xSens = xSens.value;
        options.ySens = ySens.value;
        options.yInversion = yInversion.isOn;
        options.bgmLevel = BGMVolume.value;
        options.sfxLevel = SFXVolume.value;
        SaveSystem.SaveOptions(options);

		Info.optionsData = options;
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
