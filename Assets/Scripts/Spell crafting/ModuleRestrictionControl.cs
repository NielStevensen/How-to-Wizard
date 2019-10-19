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
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
