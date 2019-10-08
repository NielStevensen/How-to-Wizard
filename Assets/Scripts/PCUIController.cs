using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PCUIController : MonoBehaviour
{

    public GameObject[] panels;
    public int selectedLevel = -1;

    public PlayerData stats;
    public Text time;
    public Text spells;
    public bool isChallengeMode;

    public OptionsData options;
    public Slider xSens;
    public Slider ySens;
    public Toggle yInversion;
    public Slider BGMVolume;
    public Slider SFXVolume;


    // Start is called before the first frame update
    void Start()
    {
        stats = SaveSystem.LoadGame();
        options = SaveSystem.LoadOptions();
        xSens.value = options.xSens;
        ySens.value = options.ySens;
        yInversion.isOn = options.yInversion;
        BGMVolume.value = options.bgmLevel;
        SFXVolume.value = options.sfxLevel;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void ChangePanel(int panelIndex)
    {
        for (int i = 0; i < panels.Length; i ++)
        {
            if (i == panelIndex) panels[i].SetActive(true);
            else panels[i].SetActive(false);
        }
    }

    public void setStats(int levelIndex)
    {
        selectedLevel = levelIndex -1;
        if (stats.storyClearData[selectedLevel] == true)           
        {
            time.text = "Best time: " + FormatTime(stats.storyClearTime[selectedLevel]);
            spells.text = "Least spells: " + stats.storyBestSpell[selectedLevel].ToString();
        }
        else
        {
            time.text = "Best time: - : --";
            spells.text = "Least spells: --";
        }
    }
    public string FormatTime(float time)
    {
        int minutes = (int)time / 60;
        int seconds = (int)time / 1 - 60 * minutes;

        return string.Format("{0:0}:{1:00}", minutes, seconds);
    }


    public void loadLevel()
    {
        if(selectedLevel != -1)
        {
            if ( !isChallengeMode)
            {
                Info.currentGameMode = GameMode.Story;
                string toload = "Level " + (selectedLevel + 1);
                SceneManager.LoadScene(toload);
            }

        }
    }

    public void applySettings()
    {
        Debug.Log("upsadint settings");
        options.xSens = xSens.value;
        options.ySens = ySens.value;
        options.yInversion = yInversion.isOn;
        options.bgmLevel = BGMVolume.value;
        options.sfxLevel = SFXVolume.value;
        SaveSystem.SaveOptions(options);
    }

}
