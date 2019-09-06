using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyCrstalRange : MonoBehaviour
{
    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.GetComponentInChildren<CrystalInfo>() != null)
        {
            other.gameObject.GetComponentInChildren<CrystalInfo>().unused = false;
            Destroy(other.gameObject);
        }
    }
}
