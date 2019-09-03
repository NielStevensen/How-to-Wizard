using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class VRMovement : MonoBehaviour
{
	//Movement control scheme
	[Tooltip("Whether the player is moving via trackpad or teleportation.")]
	public bool isTeleportation = true;

    public LayerMask ignoreRays;
    public SteamVR_Input_Sources hand;
	public GameObject handObject;
    public Transform cameraTransform;
    public SteamVR_Action_Vector2 moveAction;
	public SteamVR_Action_Boolean teleportAction;
	
	//Raycast info
	public float teleportMaxDistance = 7.5f;
	RaycastHit teleportHit;
	Vector3 teleportLocation;
	public LineRenderer lineRenderer;
	
	//Movement speed of the player
	[Tooltip("Movement speed of the player.")]
    public float movementSpeed = 5.0f;

    //Movement expressed as a vector3
    private Vector3 movement = Vector3.zero;

	//Trackpad deadzone
	[Tooltip("Trackpad deadzone.")]
	public float deadzone = 0.1f;
	
	//Handle movement and phone
	void Update()
    {
		if (isTeleportation)
		{
			if (teleportAction.GetLastState(hand))
			{
				if (Physics.Raycast(handObject.transform.position, handObject.transform.forward, out teleportHit, teleportMaxDistance, ~ignoreRays))
				{
					//test location of hit
					//test based on an ideal size
					//if within a ertain range, can try to match those
					//else, use maximum values based on ideal size
					//when testing, should ignore certain objects: props, table (to an extent so the player can comfortably walk up to it), pressure plates

					//outside of the walls, there should be a foggy particle effect so that players can't see out past the walls
				}
				else
				{
					
				}
			}
			else if (teleportAction.GetLastStateUp(hand))
			{
				
			}
			else
			{
				
			}
		}
		else
		{
			if(Vector2.Distance(Vector2.zero, new Vector2(moveAction.GetAxis(hand).x, moveAction.GetAxis(hand).y)) > deadzone)
			{
				movement = Vector3.Normalize(new Vector3(moveAction.GetAxis(hand).x, 0, moveAction.GetAxis(hand).y)) * movementSpeed;
				movement = cameraTransform.TransformDirection(movement);
			}
			
			CharacterController characterController = GetComponent<CharacterController>();

			if (characterController.enabled)
			{
				characterController.Move(movement * Time.deltaTime);
			}
		}
    }
}
