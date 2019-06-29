using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireController : MonoBehaviour
{
	//Lifetime
	[Tooltip("Lifetime of the flame object.")]
	public float lifetime = 5;

    //Setup
    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    //...
    void Update()
    {
        
    }

	//On entering the flame, set flammable objects on fire
    private void OnTriggerEnter(Collider other)
    {
        
    }
}
