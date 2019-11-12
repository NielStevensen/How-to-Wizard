using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModuleRestrictionControl : MonoBehaviour
{
    public GameObject[] crystals;
    // Start is called before the first frame update
    void Start()
    {
        if(Info.currentGameMode == GameMode.NewGamePlus) // activae all for new game plus
        {
            foreach(GameObject a in crystals)
            {
                a.SetActive(true);
            }
        }
        if (Info.currentGameMode == GameMode.Sandbox)
        {
            PlayerData data = SaveSystem.LoadGame();
            crystals[0].SetActive(true);
            crystals[7].SetActive(true);

            if (data.storyClearData[0])
            {
                crystals[1].SetActive(true);
            }
            if (data.storyClearData[1])
            {
                crystals[8].SetActive(true);
                crystals[9].SetActive(true);
            }
            if (data.storyClearData[2])
            {
                crystals[4].SetActive(true);
            }
            if (data.storyClearData[4])
            {
                crystals[2].SetActive(true);
                crystals[5].SetActive(true);
                crystals[10].SetActive(true);
            }
            if (data.storyClearData[5])
            {
                crystals[11].SetActive(true);
            }
            if (data.storyClearData[6])
            {
                crystals[3].SetActive(true);
                crystals[12].SetActive(true);
            }
            if (data.storyClearData[4])
            {
                crystals[6].SetActive(true);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
