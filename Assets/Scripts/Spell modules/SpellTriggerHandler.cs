using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellTriggerHandler : MonoBehaviour
{
	//Values to retrieve
	//[HideInInspector]
	public List<GameObject> containedObjects = new List<GameObject>();
	
	//Find all objects contained in trigger
	private void OnTriggerEnter(Collider other)
	{
		int otherLayer = other.gameObject.layer;

		if (otherLayer != LayerMask.NameToLayer("Ignore Raycast") && otherLayer != LayerMask.NameToLayer("Player"))
		{
			containedObjects.Add(other.gameObject);
		}
	}
}
