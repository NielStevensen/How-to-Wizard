using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PCCraftingZone : MonoBehaviour
{
	//Player reference
	[SerializeField]
	private GameObject controller;

	//Set reference
	private void Start()
	{
		controller = FindObjectOfType<PlayerController>().transform.GetChild(0).gameObject;
	}

	//While inside, raycast to highlight stuff
	private void OnTriggerStay(Collider other)
	{
		
	}

	private void OnTriggerExit(Collider other)
	{
		
	}
}
