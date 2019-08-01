using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlateController : MonoBehaviour
{
	//Activation details
	[Tooltip("The object this pressure plate activates.")]
	public ActivationManager target;
	[Tooltip("The state passed to a door on activation.")]
	public DoorState activeState;
	[Tooltip("The state passed to a door on deactivation.")]
	public DoorState inactiveState;

	[Space(10)]

	//Pressure plate values
	[Tooltip("The amount of weight required to activate this pressure plate.")]
	public float weightThreshold = 5.0f;
	[Tooltip("The current amount of weight detected.")]
	public float currentWeight = 0.0f;
	[Tooltip("The current activation state of the pressure plate.")]
	public bool isActivated = false;
	private List<GameObject> objectsAbove = new List<GameObject>();



	//On moving above the pressure plate, note its presence and weight
	private void OnTriggerEnter(Collider other)
	{
		if(other.tag == "Interactable")
		{
			objectsAbove.Add(other.gameObject);

			currentWeight += other.attachedRigidbody.mass;

			CheckActivationState();
		}
	}

	//On moving off the pressure plate, note its absence and reduction in weight
	private void OnTriggerExit(Collider other)
	{
		if (objectsAbove.Contains(other.gameObject))
		{
			objectsAbove.Remove(other.gameObject);

			currentWeight -= other.attachedRigidbody.mass;
			
			CheckActivationState();
		}
	}

	//Check if the detected weight falls in the weight threshold
	void CheckActivationState()
	{
		if(currentWeight >= weightThreshold && !isActivated)
		{
			isActivated = true;

			target.HandleDoor(activeState);
		}
		else if(currentWeight < weightThreshold && isActivated)
		{
			isActivated = false;

			target.HandleDoor(inactiveState);
		}
	}
}
