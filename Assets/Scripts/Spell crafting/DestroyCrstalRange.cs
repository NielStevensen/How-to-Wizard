using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyCrstalRange : MonoBehaviour
{
    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.GetComponentInChildren<CrystalInfo>() != null)
        {
            Destroy(other.gameObject);
        }
    }
}
