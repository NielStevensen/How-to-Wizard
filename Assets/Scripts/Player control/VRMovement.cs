using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class VRMovement : MonoBehaviour
{

    public SteamVR_Input_Sources hand;
    public Transform cameraTransform;
    public SteamVR_Action_Vector2 moveAction;

    //Movement speed of the player
    [Tooltip("Movement speed of the player.")]
    public float movementSpeed = 5.0f;

    //Movement expressed as a vector3
    private Vector3 movement = Vector3.zero;


    //Handle movement and phone
    void Update()
    {
        movement = Vector3.Normalize(new Vector3(moveAction.GetAxis(hand).x, 0, moveAction.GetAxis(hand).y)) * movementSpeed;
        Debug.Log(moveAction.GetAxis(hand));
        movement = cameraTransform.TransformDirection(movement);
        CharacterController characterController = GetComponent<CharacterController>();

        if (characterController.enabled)
        {
            characterController.Move(movement * Time.deltaTime);
        }
    }
}
