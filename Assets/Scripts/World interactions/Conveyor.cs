using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Conveyor : MonoBehaviour
{
    [Tooltip("can the conveyor use transfrom to move non rigid body objects")]
    public bool canUseTransfrom;
    public float Movespeed;


    // Update is called once per frame
    private void OnTriggerStay(Collider other)
    {
        if(other.GetComponent<Rigidbody>() != null)
        {
            other.GetComponent<Rigidbody>().velocity = other.GetComponent<Rigidbody>().velocity + (transform.forward * Movespeed);
        }
        else if (canUseTransfrom)
        {
            transform.Translate(transform.forward * Movespeed);
        }
    }
}
