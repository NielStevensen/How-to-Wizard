﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserController : MonoBehaviour
{
	//Activation values
	[Tooltip("The initial activation state of this laser.")]
	public bool startsActivated = true;
	[Tooltip("Is this laser active?")]
	public bool isActivated = true;
	[Tooltip("Rotation speed of the laser.")]
	public float rotationSpeed = 0.0f;
	private Vector3 currentRotation = Vector3.zero;
	[Tooltip("Origin of the laser.")]
	public Transform laserOrigin;
	
	//Impact object
	private GameObject impactObject;
	private NullManager nullManager;
	
	//Log an error if there is no origin
	void Start()
	{
		Debug.Assert(laserOrigin != null, gameObject.name + " does not have a laser origin!");
		
		isActivated = startsActivated;
	}
	
	//Handle laser and impact
	void Update()
	{
		if (isActivated)
		{
			RaycastHit hit;
			
			Debug.DrawLine(laserOrigin.position, laserOrigin.position + Vector3.Normalize(laserOrigin.forward) * 1000.0f, Color.red);
			
			if (Physics.Raycast(laserOrigin.position, laserOrigin.forward, out hit, 1000.0f))
			{
				if (hit.collider.gameObject != impactObject)
				{
					impactObject = hit.collider.gameObject;
					nullManager = impactObject.GetComponent<NullManager>();
				}
				
				if (impactObject.tag == "Interactable")
				{
					bool shouldDestroy = true;
					
					if (nullManager != null)
					{
						if (nullManager.IsNulled)
						{
							shouldDestroy = false;
						}
					}
					
					if (shouldDestroy)
					{
						Destroy(impactObject);
						
						impactObject = null;
					}
				}
			}
			
			if (rotationSpeed != 0)
			{
				currentRotation.y += rotationSpeed;
				
				transform.Rotate(new Vector3(0, rotationSpeed, 0));
			}
		}
	}
	
	//Set activation state
	public void HandleState(bool state)
	{
		isActivated = startsActivated != state;
	}
}