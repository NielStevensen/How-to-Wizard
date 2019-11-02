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
            FixedJoint[] toCheck = GameObject.FindObjectsOfType<FixedJoint>();
            foreach (FixedJoint a in toCheck)
            {
                if (a.connectedBody.gameObject != null && a.connectedBody.gameObject == other.gameObject)
                {
                    a.GetComponent<PickupSpell>().ClearCrystal();
                    break;
                }
            }
            Destroy(other.gameObject);
        }
    }
}
