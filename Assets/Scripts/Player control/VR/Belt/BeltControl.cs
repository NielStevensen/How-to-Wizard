using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeltControl : MonoBehaviour
{

    public bool occupied;
    public GameObject occupant;
    public int myNumber;
    bool canOccupy;
    float children = 0;

    // Start is called before the first frame update


    private void OnTriggerStay(Collider other)
    {
        canOccupy = true;
        if (other.gameObject.GetComponent<Spell>() != null && !occupied)
        {
            foreach (GameObject b in GetComponentInParent<BeltHolder>().slots)
            {
                if (b.GetComponent<BeltControl>().occupant == other.gameObject)
                {
                    canOccupy = false;
                }
            }
            FixedJoint[] toCheck = GameObject.FindObjectsOfType<FixedJoint>();
            foreach(FixedJoint a in toCheck)
            {
                if (canOccupy)
                {
                    if (a.connectedBody.gameObject == other.gameObject) a.GetComponent<PickupSpell>().beltSlots[myNumber] = true;
                    occupant = a.connectedBody.gameObject;
                    occupied = true;
                }
            }
            // get hand refrence and set vallue on hand to specify location is valid
        }
    }
    private void OnTransformChildrenChanged()
    {
        if (transform.childCount < children)
        {
            occupied = false;
            occupant = null;
        }
        children = transform.childCount;
    }
    private void OnTriggerExit(Collider other)
    {
        if (other == occupant && occupied)
        {
            FixedJoint[] toCheck = GameObject.FindObjectsOfType<FixedJoint>();
            foreach (FixedJoint a in toCheck)
            {
                if (a.connectedBody.gameObject == other.gameObject) a.GetComponent<PickupSpell>().beltSlots[myNumber] = false;
                occupied = false;
                occupant = null;
            }
            // get hand refrence and set vallue on hand to specify location is no longer valid
        }
    }
}
