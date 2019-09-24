using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour {

    //Movement speed of the player
    [Tooltip("Movement speed of the player.")]
    public float movementSpeed = 5.0f;

    //Movement expressed as a vector3
    private Vector3 movement = Vector3.zero;

	[Space(10)]

	//Crafting values
	[Tooltip("Crafting and scroll interaction range.")]
	public float interactionRange = 3.75f;
	private Transform cameraTransform;
	private bool isCraftCooldown = false;
	[Tooltip("The cooldown on crafting after clicking the craft confirm.")]
	public float craftCooldown = 2.5f;
	private GameObject selectedCrystal = null;
	private GameObject[] slottedCrystals = new GameObject[5] { null, null, null, null, null };
	private AttachCrystal[] crystalSlots = new AttachCrystal[5];
	private SpellCreation table = null;

	[Space(10)]

	//Spell storage values
	[Tooltip("Spell storage UI objects.")]
	public Image[] storageUI;
	private Image[,] storageIcons = new Image[3, 5];
	[Tooltip("The symbols for each module.")]
	public Sprite[] moduleSymbols;
	private GameObject[] storedSpells = new GameObject[3] { null, null, null };
	private int selectedSpell = -1;
	[Tooltip("Selection border image.")]
	public RectTransform selectionBorder;
	[HideInInspector]
	public bool isSpellCollected = true;

	[Space(10)]

	//Spell casting values
	[Tooltip("Delay between using spells in seconds.")]
	public float castingDelay = 1.0f;
	private bool isCastingCooldown = false;
	[HideInInspector]
	public bool isSpellCasted = false;
	[Tooltip("Spell casting source.")]
	public GameObject spellOrigin;

	[Space(10)]

	//Reset values
	[Tooltip("The amount of time in seconds the reset button must he held to reset.")]
	public float resetTime = 2.5f;
	private bool canReset = false;
	private bool isResetting = false;


	//temp recolour values
	public Color crystalDefault;
	public Color crystalSelected;


	//currently doesn't work
	//Shaders
	/*[Tooltip("Default shader.")]
	public Material defaultMaterial;
	[Tooltip("Outline shader.")]
	public Material outlineMaterial;

	[Space(10)]

	//Outline colours
	[Tooltip("Hover outline colour.")]
	public Color hoverColour;
	[Tooltip("Selected outline colour.")]
	public Color selectedColour;

	//Object reference
	private RaycastHit hit;
	private GameObject target = null;
	private Renderer targetRenderer = null;
	private CrystalInfo targetInfo = null;
	private bool targetSelectState = false;
	*/
	
	//Set cursor state and set references
	private void Start()
    {
		cameraTransform = transform.GetChild(0);
		table = FindObjectOfType<SpellCreation>();

		for(int i = 0; i < 5; i++)
		{
			crystalSlots[i] = table.transform.GetChild(i).gameObject.GetComponent<AttachCrystal>();
		}

		for(int i = 0; i < 3; i++)
		{
			for(int j = 0; j< 5; j++)
			{
				storageIcons[i, j] = storageUI[i].transform.GetChild(j).gameObject.GetComponent<Image>();
			}
		}

		StartCoroutine(WaitUntilRelease());
    }

    //Handle input
    void Update ()
    {
		if (!isResetting)
		{
			HandleMovement();

			HandleCrafting();

			HandleSpellStorage();

			HandleSpellCastng();

			if (canReset)
			{
				HandleResetInput();
			}

			//HandleCraftingOutline();
		}
	}

	#region Movement
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
	#endregion

	#region Spell Crafting
	//Handle crafting
	void HandleCrafting()
	{
		if (isCraftCooldown)
		{
			return;
		}

		if (Input.GetButtonDown("Fire1"))
		{
			RaycastHit hit;

			if(Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, interactionRange, 1 << LayerMask.NameToLayer("Crafting")))
			{
				if (hit.collider.gameObject.GetComponent<CrystalInfo>())
				{
					if(selectedCrystal == null)
					{
						selectedCrystal = hit.collider.gameObject;

						selectedCrystal.GetComponent<CrystalInfo>().isSelected = true;

						selectedCrystal.GetComponent<Renderer>().material.color = crystalSelected;
					}
					else
					{
						GameObject localReference = selectedCrystal;

						DeselectCrystals();

						if (localReference != hit.collider.gameObject)
						{
							selectedCrystal = hit.collider.gameObject;

							selectedCrystal.GetComponent<CrystalInfo>().isSelected = true;

							selectedCrystal.GetComponent<Renderer>().material.color = crystalSelected;
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
					if (isSpellCollected)
					{
                        DeselectCrystals();

                        bool areSlottedCrystals = false;

                        foreach(GameObject obj in slottedCrystals)
                        {
                            if(obj != null)
                            {
                                areSlottedCrystals = true;

                                break;
                            }
                        }

                        if (areSlottedCrystals)
                        {
                            isSpellCollected = false;

                            StartCoroutine(HandleCraftingCooldown());

                            for (int i = 0; i < 5; i++)
                            {
                                if (slottedCrystals[i] != null)
                                {
                                    crystalSlots[i].attachedModule = slottedCrystals[i].GetComponent<CrystalInfo>().module;
                                    crystalSlots[i].attachedType = slottedCrystals[i].GetComponent<CrystalInfo>().moduleType;
                                }
                            }

                            table.ConfirmSpell();
                        }
					}
				}
				else if (hit.collider.gameObject.GetComponent<Button>())
				{	
					Clear();
				}
			}
		}
	}

	//Deselect crystals
	public void DeselectCrystals()
	{
		if(selectedCrystal != null)
		{
			selectedCrystal.GetComponent<CrystalInfo>().isSelected = false;

			selectedCrystal.GetComponent<Renderer>().material.color = crystalDefault;
		}

		selectedCrystal = null;
	}

	//Destroy all slotted crystals
	public void Clear()
	{
		for (int i = 0; i < 5; i++)
			if (slottedCrystals[i] != null)
			{
				Destroy(slottedCrystals[i]);

				crystalSlots[i].attachedModule = "";
				crystalSlots[i].attachedType = -1;
			}
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
	#endregion

	#region Spell storage
	//Handle spell storage input
	void HandleSpellStorage()
	{
		if (Input.GetButtonDown("Fire1"))
		{
			RaycastHit hit;

			if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, interactionRange, 1 << LayerMask.NameToLayer("SpellScroll")))
			{
				int emptySlot = -1;

				for(int i = 0; i < 3; i++)
				{
					if(storedSpells[i] == null)
					{
						emptySlot = i;

						break;
					}
				}

				if(emptySlot > -1)
				{
					isSpellCollected = true;

					storedSpells[emptySlot] = hit.collider.gameObject;
					
					storedSpells[emptySlot].GetComponent<Collider>().enabled = false;

					for(int i = 0; i < 3; i++)
					{
						storedSpells[emptySlot].transform.GetChild(0).GetChild(i).gameObject.GetComponent<Renderer>().enabled = false;
					}

					storedSpells[emptySlot].gameObject.name = "Stored Spell " + emptySlot;

					storageUI[emptySlot].color = Color.yellow;

					List<string> moduleList = storedSpells[emptySlot].GetComponent<Spell>().Modules;

					for(int i = 0; i < moduleList.Count; i++)
					{
						int moduleIndex = 0;

						switch (moduleList[i])
						{
							case "Projectile":
								moduleIndex = 0;

								break;
							case "Split":
								moduleIndex = 1;

								break;
							case "Charge":
								moduleIndex = 2;

								break;
							case "Touch":
								moduleIndex = 3;

								break;
							case "AOE":
								moduleIndex = 4;

								break;
							case "Proximity":
								moduleIndex = 5;

								break;
							case "Timer":
								moduleIndex = 6;

								break;
							case "Fire":
								moduleIndex = 7;

								break;
							case "Push":
								moduleIndex = 8;

								break;
							case "Pull":
								moduleIndex = 9;

								break;
							case "Weight":
								moduleIndex = 10;

								break;
							case "Barrier":
								moduleIndex = 11;

								break;
							case "Null":
								moduleIndex = 12;

								break;
						}

						storageIcons[emptySlot, i].sprite = moduleSymbols[moduleIndex];
					}
				}
			}
		}

		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			SelectSpell(0);
		}
		else if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			SelectSpell(1);
		}
		else if (Input.GetKeyDown(KeyCode.Alpha3))
		{
			SelectSpell(2);
		}
	}
	
	//Handle spell selection
	void SelectSpell(int slot)
	{
		if (selectedSpell == slot || storedSpells[slot] == null)
		{
			selectedSpell = -1;

			selectionBorder.localPosition = new Vector3(-1000.0f, -500, 0);
		}
		else
		{
			selectedSpell = slot;

			selectionBorder.localPosition = new Vector3(-1000.0f + (slot + 1) * 100.0f, -500, 0);
		}
	}
	#endregion

	#region Spell casting
	//Handle spell casting
	void HandleSpellCastng()
	{
		if(selectedSpell > -1 && !isCastingCooldown)
		{
			if (Input.GetButtonDown("Fire2"))
			{
				StartCoroutine(HandleCastingCooldown(storedSpells[selectedSpell], selectedSpell));

				Destroy(storedSpells[selectedSpell].GetComponent<Rigidbody>());
				
				storedSpells[selectedSpell].transform.SetParent(spellOrigin.transform);
				storedSpells[selectedSpell].transform.localPosition = Vector3.zero;
				storedSpells[selectedSpell].transform.localRotation = Quaternion.identity;

				storedSpells[selectedSpell].GetComponent<Spell>().CallSpell();
				
				storedSpells[selectedSpell] = null;
				selectedSpell = -1;

				selectionBorder.localPosition = new Vector3(-1000.0f, -500, 0);
			}
		}
	}

	//Handle casting cooldown
	IEnumerator HandleCastingCooldown(GameObject scroll, int index)
	{
		isCastingCooldown = true;

		while (!isSpellCasted)
		{
			yield return new WaitForEndOfFrame();
		}

		isSpellCasted = false;

		storageUI[index].color = Color.clear;
		
		for (int i = 0; i < 5; i++)
		{
			storageIcons[index, i].sprite = moduleSymbols[13];
		}

		yield return new WaitForSeconds(castingDelay);

		isCastingCooldown = false;
	}
	#endregion

	#region Reset
	//Handle reset input
	void HandleResetInput()
	{
		if (Input.GetButtonDown("Reset"))
		{
			StartCoroutine(HandleResetHold());
		}
	}

	//Handle reset hold input
	IEnumerator HandleResetHold()
	{
		isResetting = true;

		bool wasHeld = true;

		float elapsedTime = 0.0f;

		while(elapsedTime < resetTime)
		{
			if (!Input.GetButton("Reset"))
			{
				wasHeld = false;

				break;
			}

			elapsedTime += Time.deltaTime;
			
			yield return new WaitForEndOfFrame();
		}

		isResetting = false;

		if (wasHeld)
		{
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		}
	}

	//Prevent holding q from a reset after a successful reset starting a reset
	IEnumerator WaitUntilRelease()
	{
		yield return new WaitForSeconds(1);

		while (Input.GetButtonDown("Reset") || Input.GetButton("Reset"))
		{
			yield return new WaitForEndOfFrame();
		}
		
		canReset = true;
	}
	#endregion

	/*
	#region Crafting outline
	//Handle crafting outline
	void HandleCraftingOutline()
	{
		GameObject newTarget = null;

		if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, interactionRange, 1 << LayerMask.NameToLayer("Crafting")))
		{
			newTarget = hit.collider.gameObject;
		}

		if (target != null && newTarget == target)
		{
			if(targetInfo != null)
			{
				if (targetSelectState != targetInfo.isSelected)
				{
					targetSelectState = targetInfo.isSelected;

					ApplyOutline();
				}
			}
		}
		else
		{
			if (target != null)
			{
				RemoveOutline();
			}

			target = newTarget;

			if (target != null)
			{
				targetRenderer = target.GetComponent<Renderer>();
				targetInfo = target.GetComponent<CrystalInfo>();
				targetSelectState = targetInfo == null ? false : targetInfo.isSelected;

				ApplyOutline();
			}
		}
	}

	//Apply outline
	void ApplyOutline()
	{
		foreach (Material mat in targetRenderer.materials)
		{
			//mat.shader = outlineShader;
			mat.SetInt("_DrawOutline", 1);
			mat.SetColor("_OutlineColour", targetSelectState ? selectedColour : hoverColour);
		}
	}

	//Remove outline
	void RemoveOutline()
	{
		if (targetRenderer != null)
		{
			foreach (Material mat in targetRenderer.materials)
			{
				//mat.shader = defaultShader;
				mat.SetInt("_DrawOutline", 0);
			}
		}
	}
	#endregion
	*/
}