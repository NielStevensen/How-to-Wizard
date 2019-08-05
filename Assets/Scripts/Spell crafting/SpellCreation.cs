using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellCreation : MonoBehaviour
{

    public GameObject[] moduleZones;
    public List<string> spellInstructions = new List<string>();
    public List<int> moduletypes = new List<int>();
    bool isValid;
    public GameObject spellPrefab;
    public GameObject IncorrectSpell;
    public Transform spawnpoint;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void ConfirmSpell()
    {
        // clear lists
        spellInstructions.Clear();
        moduletypes.Clear();
        isValid = true; //aceptable spell until proven optherwiise
        // check all zones and add to list of modules
        for (int i = 0; i < 5; i++)
        {
            if (moduleZones[i].GetComponentInChildren<CrystalInfo>() != null)
            {
                spellInstructions.Add(moduleZones[i].GetComponentInChildren<CrystalInfo>().module);
                print(spellInstructions[spellInstructions.Count -1]);
                moduletypes.Add(moduleZones[i].GetComponentInChildren<CrystalInfo>().moduleType);
            }
        }
            // check valididty
        if (moduletypes.Count > 1)
        {
            if (moduletypes[0] != 0) isValid = false; // if the first is not a primary
            if (moduletypes[1] == 0) isValid = false; // if the second is a primary
            for (int j = 2; j < moduletypes.Count; j++)
            {
                if (moduletypes[j] != 2) isValid = false; // if anthing from 3 - 5 is not an effect
            }
        }
        else
        {
            isValid = false;
        }

        if(isValid == true) // if the spell passes al checks
        {
        GameObject currentSpell = Instantiate(spellPrefab, spawnpoint.position, Quaternion.identity);// create a spell object
            for (int k = 0; k < moduletypes.Count; k++)
            {
                    currentSpell.GetComponent<Spell>().Modules.Add(spellInstructions[k]); // add relevant modules to list
            }
        }
        else
        {
            Instantiate(IncorrectSpell, spawnpoint.position, Quaternion.identity);
        }

       }
}
