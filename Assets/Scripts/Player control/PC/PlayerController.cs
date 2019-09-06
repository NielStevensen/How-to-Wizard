using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    //Movement speed of the player
    [Tooltip("Movement speed of the player.")]
    public float movementSpeed = 5.0f;

    //Movement expressed as a vector3
    private Vector3 movement = Vector3.zero;

	//Crafting values
	[Tooltip("Crafting interaction range.")]
	public float craftingRange = 2.5f;
	private Transform cameraTransform;
	private GameObject selectedCrystal = null;

	//Is the player resetting?
	private bool isResetting = false;

	//Set cursor state and set references
	private void Start()
    {
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;

		cameraTransform = transform.GetChild(0);
    }

    //Handle input
    void Update ()
    {
		if (!isResetting)
		{
			HandleMovement();

			HandleCrafting();
		}
	}

	//Handle player movement
	void HandleMovement()
	{
		movement = Vector3.Normalize(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"))) * movementSpeed;
		movement = transform.TransformDirection(movement);
		CharacterController characterController = GetComponent<CharacterController>();

		if (characterController.enabled)
		{
			characterController.Move(movement * Time.deltaTime);
		}
	}

	//Handle crafting
	void HandleCrafting()
	{
		if (Input.GetButton("Fire1"))
		{
			RaycastHit hit;

			if(Physics.Raycast(cameraTransform.position, cameraTransform.rotation.eulerAngles, out hit, craftingRange, 1 << LayerMask.NameToLayer("Crafting")))
			{
				if (hit.collider.gameObject.GetComponent<CrystalInfo>())
				{
					if(selectedCrystal != null)
					{
						DeselectCrystals();
					}
					
					selectedCrystal = hit.collider.gameObject;
				}
				else if (hit.collider.gameObject.GetComponent<AttachCrystal>())
				{
					if(selectedCrystal != null)
					{
						SlotInCrystal(selectedCrystal, hit.collider.gameObject);
					}
				}
				else if (hit.collider.gameObject.GetComponent<FinalizeSpell>())
				{
					DeselectCrystals();


				}
			}


		}
	}

	//Deselect crystals
	public void DeselectCrystals()
	{
		selectedCrystal = null;
	}

	//Move crystals to their slot
	IEnumerator SlotInCrystal(GameObject crystal, GameObject slot)
	{
		Vector3 origin = crystal.transform.position;
		Vector3 direction = slot.transform.position - origin;

		for (int i = 0; i <= 180; i++)
		{
			float rate = i / 180;
			float percentage = 1 / (1 + Mathf.Pow((float)Math.E, -12.5f * (rate - 0.5f)));

			crystal.transform.position = origin + direction * percentage;

			yield return new WaitForEndOfFrame();
		}
	}
}