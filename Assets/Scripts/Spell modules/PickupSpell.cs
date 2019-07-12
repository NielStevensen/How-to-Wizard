using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupSpell : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<Spell>())
        {
            other.gameObject.GetComponent<Spell>().VRSpell();
        }
    }
}
