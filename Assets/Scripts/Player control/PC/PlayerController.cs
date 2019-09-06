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

	[Space(10)]

	//Crafting values
	[Tooltip("Crafting interaction range.")]
	public float craftingRange = 2.5f;
	private Transform cameraTransform;
	private bool isCraftCooldown = false;
	[Tooltip("The cooldown on crafting after clicking the craft confirm.")]
	public float craftCooldown = 2.5f;
	private GameObject selectedCrystal = null;
	private GameObject[] slottedCrystals = new GameObject[5] { null, null, null, null, null };
	private AttachCrystal[] crystalSlots = new AttachCrystal[5];
	[SerializeField]
	private SpellCreation table = null;

	//Is the player resetting?
	private bool isResetting = false;

	//Set cursor state and set references
	private void Start()
    {
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;

		cameraTransform = transform.GetChild(0);
		table = FindObjectOfType<SpellCreation>();

		for(int i = 0; i < 5; i++)
		{
			crystalSlots[i] = table.transform.GetChild(i).gameObject.GetComponent<AttachCrystal>();
		}
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
		if (isCraftCooldown)
		{
			return;
		}

		if (Input.GetButtonDown("Fire1"))
		{
			Debug.DrawRay(cameraTransform.position, Vector3.Normalize(cameraTransform.forward) * craftingRange, Color.green, 5);

			RaycastHit hit;

			if(Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, craftingRange, 1 << LayerMask.NameToLayer("Crafting")))
			{
				if (hit.collider.gameObject.GetComponent<CrystalInfo>())
				{
					if(selectedCrystal == null)
					{
						selectedCrystal = hit.collider.gameObject;
					}
					else
					{
						GameObject localReference = selectedCrystal;

						DeselectCrystals();

						if (localReference != hit.collider.gameObject)
						{
							selectedCrystal = hit.collider.gameObject;
						}
					}
				}
				else if (hit.collider.gameObject.GetComponent<AttachCrystal>())
				{
					if (selectedCrystal != null)
					{
						StartCoroutine(SlotInCrystal(selectedCrystal, hit.collider.gameObject));

						DeselectCrystals();
					}
				}
				else if (hit.collider.gameObject.GetComponent<FinalizeSpell>())
				{
					DeselectCrystals();

					StartCoroutine(HandleCraftingCooldown());

					for(int i = 0; i < 5; i++)
					{
						if(slottedCrystals[i] != null)
						{
							crystalSlots[i].attachedModule = slottedCrystals[i].GetComponent<CrystalInfo>().module;
							crystalSlots[i].attachedType = slottedCrystals[i].GetComponent<CrystalInfo>().moduleType;
						}
					}

					table.ConfirmSpell();
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
		isCraftCooldown = true;

		int slotNumber = int.Parse(slot.name[6].ToString()) - 1;

		if (slottedCrystals[slotNumber] != null)
		{
			Destroy(slottedCrystals[slotNumber]);

			crystalSlots[slotNumber].attachedModule = "";
			crystalSlots[slotNumber].attachedType = -1;
		}

		slottedCrystals[slotNumber] = crystal;

		Vector3 origin = crystal.transform.position;
		Vector3 direction = slot.transform.position - origin;
		
		for (int i = 0; i <= 90; i++)
		{
			float rate = i / 90.0f;
			float percentage = 1 / (1 + Mathf.Pow((float)Math.E, -12.5f * (rate - 0.5f)));
			
			crystal.transform.position = origin + direction * percentage;

			yield return new WaitForEndOfFrame();
		}

		isCraftCooldown = false;
	}

	//Handle crafting cooldown whlie a spell is crafting
	IEnumerator HandleCraftingCooldown()
	{
		isCraftCooldown = true;

		yield return new WaitForSeconds(craftCooldown);

		isCraftCooldown = false;
	}
}