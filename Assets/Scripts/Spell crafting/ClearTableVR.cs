using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearTableVR : MonoBehaviour
{
    AttachCrystal[] moduleZones;
    bool clear = false;

    // Start is called before the first frame update
    void Start()
    {
        moduleZones = FindObjectsOfType<AttachCrystal>();      
    }

    //Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PickupSpell>())
        {
             foreach(CrystalInfo a in FindObjectsOfType<CrystalInfo>())
             {
                Destroy(a.gameObject);
             }

            clear = true;
        }
    }

    private void LateUpdate()
    {
        if(clear)
        {
            foreach (AttachCrystal a in moduleZones)
            {
                a.attachedModule = "";
                a.attachedType = -1;
            }

            clear = false;
        }
    }
}