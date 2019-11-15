using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellTriggerHandler : MonoBehaviour
{
	//Values to retrieve
	public List<GameObject> containedObjects = new List<GameObject>();

	//Life of an AOE object
	private int lifeTime = 10;

	//Disable if no collision
	private void Update()
	{
		lifeTime--;

		if(lifeTime == 0)
		{
			containedObjects.Add(null);
		}
	}

	//Find all objects contained in trigger
	private void OnTriggerEnter(Collider other)
	{
		containedObjects.Add(other.gameObject);
	}
}
