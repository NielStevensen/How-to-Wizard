using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CycleButton : MonoBehaviour
{

    public VRLevelControll controller;
    public int cycleValue;
    public GameMode mode;

    public GameObject challengeButton;
    public GameObject newGamePlusButton;
    private PlayerData stats;
    // Start is called before the first frame update
    void Start()
    {
        stats = SaveSystem.LoadGame();
        if(stats.storyClearData[14])
        {
            challengeButton.SetActive(true);
            newGamePlusButton.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (mode != GameMode.Unset)
        {
            Info.currentGameMode = mode;
        }
        if (other.GetComponent<PickupSpell>() != null)
        {
            controller.Cycle(cycleValue);
        }
    }
}
