using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDestructionZone : MonoBehaviour
{
	//When an interactable leaves the zone, destroy it
	private void OnTriggerExit(Collider other)
	{
		if(other.tag == "Interactable")
		{
			if(other.gameObject.layer != LayerMask.NameToLayer("Ignore Raycast"))
			{
				Destroy(other.gameObject);
			}
		}
	}
}
