﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachCrystal : MonoBehaviour
{

    public string attachedModule;
    public int attachedType = -1;

    // Start is called before the first frame update
    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.GetComponent<CrystalInfo>() != null) //if the object is a crystal
        {
            other.GetComponent<CrystalInfo>().unused = false;
            attachedModule = other.GetComponent<CrystalInfo>().module;
            attachedType = other.GetComponent<CrystalInfo>().moduleType;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<CrystalInfo>() != null)
        {
            attachedModule = "";
            other.GetComponent<CrystalInfo>().unused = true;
            attachedType = -1;
        }
    }
}
