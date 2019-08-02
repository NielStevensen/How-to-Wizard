using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachCrystal : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<CrystalInfo>() != null) //if the object is a crystal
        {
            other.gameObject.transform.SetParent(transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.transform.parent = gameObject.transform)
        {
            other.transform.SetParent(null);
        }
    }
}
