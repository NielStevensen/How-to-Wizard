using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachCrystal : MonoBehaviour
{

    public string attachedModule;
    public int attachedType;

    // Start is called before the first frame update
    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.GetComponent<CrystalInfo>() != null) //if the object is a crystal
        {
            other.GetComponent<CrystalInfo>().unused = false;
            attachedModule = other.GetComponent<CrystalInfo>().name;
            attachedType = other.GetComponent<CrystalInfo>().moduleType;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<CrystalInfo>() != null)
        {
            attachedModule = null;
            attachedType = 0;
        }
    }
}
