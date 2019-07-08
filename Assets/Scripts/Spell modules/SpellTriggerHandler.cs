using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellTriggerHandler : MonoBehaviour
{
	//Values to retrieve
	[HideInInspector]
	public List<GameObject> containedObjects = new List<GameObject>();
	
	//Find all objects contained in trigger
	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer != LayerMask.NameToLayer("Ignore Raycast"))
		{
			containedObjects.Add(other.gameObject);
		}
	}
}
