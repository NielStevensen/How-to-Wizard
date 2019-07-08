using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileReturn : MonoBehaviour
{
    public GameObject whatHit;
    public Vector3 whereHit;
    public SpellModuleList caller;

    private void OnCollisionEnter(Collision collision)
    {
        caller.activeprojectile = false;
        Debug.Log("hit");
        whatHit = collision.gameObject;
        whereHit = collision.contacts[0].point;
    }
}
