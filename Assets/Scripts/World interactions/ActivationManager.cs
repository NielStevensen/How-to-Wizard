using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DoorState { Open = 0, Closed = 1}

public class ActivationManager : MonoBehaviour
{
	//Current state
	[Tooltip("The current state of the door.")]
	public DoorState currentState = DoorState.Open;

	//Movement values
	[Tooltip("How far the door moves when activated.")]
	public Vector3 activeDisplacement = Vector3.zero;
	private Vector3 origin;

	//Acting coroutine
	private Coroutine actingCoroutine;
	private int activationProgress = 0;

	//Set values
	private void Start()
	{
		origin = transform.position;
	}

	//Handle activation/deactivation of a door
	public void HandleDoor(DoorState state)
	{
		if(actingCoroutine != null)
		{
			StopCoroutine(actingCoroutine);
		}
		
		actingCoroutine = StartCoroutine(MoveDoor(-((int)state * 2 - 1)));
	}

	//Move the door
	IEnumerator MoveDoor(int alt)
	{
		int lowerThreshold = 0 - alt;
		int upperThreshold = 180 - alt;
		
		while (lowerThreshold < activationProgress && activationProgress < upperThreshold)
		{
			activationProgress += 1 * alt;
			
			transform.position = origin + activeDisplacement * Mathf.Sin(Mathf.Deg2Rad * activationProgress * 0.5f);

			yield return new WaitForEndOfFrame();
		}
	}
}
