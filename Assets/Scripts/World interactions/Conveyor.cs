using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Conveyor : MonoBehaviour
{
    [Tooltip("can the conveyor use transfrom to move non rigid body objects")]
    public bool canUseTransfrom;
    public float Movespeed;
    public Vector2 panningSpeed;
    public Vector3 rotatingSpeed;

    public GameObject[] gears;

    public GameObject belt;

    // Update is called once per frame
    private void OnTriggerStay(Collider other)
    {
        if(other.tag == "Interactable")
        {
            if (other.GetComponent<Rigidbody>() != null)
            {
                other.GetComponent<Rigidbody>().velocity = other.GetComponent<Rigidbody>().velocity + (transform.forward * Movespeed);
            }
            else if (canUseTransfrom)
            {
                transform.Translate(transform.forward * Movespeed);
            }
        }
    }

    private void Update()
    {
        belt.GetComponent<MeshRenderer>().material.mainTextureOffset = (belt.GetComponent<MeshRenderer>().material.mainTextureOffset + panningSpeed / 1000f);
        foreach(GameObject a in gears)
        {
            a.transform.Rotate(rotatingSpeed);
        }
    }
}
