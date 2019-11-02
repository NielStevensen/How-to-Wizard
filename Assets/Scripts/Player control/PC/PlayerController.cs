﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour {
	#region Variables
	//Movement speed of the player
	[Tooltip("Movement speed of the player.")]
    public float movementSpeed = 5.0f;
	private float initialY;

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
	private ClearTable clearButton = null;

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

	[Space(10)]

	//Pause valuse
	[Tooltip("Pause UI.")]
	public RawImage pauseMenu;
	private bool shouldUnpause = false;
	private Camera blurCam;
	[Tooltip("Pause blur shader.")]
	public Shader blurShader;
	#endregion

	[Space(10)]
	
	//temp recolour values
	public Color primaryDefault;
	public Color secondaryDefault;
	public Color effectDefault;
	public Color hoverColour;
	public Color selectedColour;
	
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
		initialY = transform.position.y;

		cameraTransform = transform.GetChild(0);
		table = FindObjectOfType<SpellCreation>();
		clearButton = table.GetComponentInChildren<ClearTable>();

		crystalSlots = table.PCSpellSlots;
		
		for(int i = 0; i < 3; i++)
		{
			for(int j = 0; j< 5; j++)
			{
				storageIcons[i, j] = storageUI[i].transform.GetChild(j).gameObject.GetComponent<Image>();
			}
		}

		StartCoroutine(WaitUntilRelease());

		blurCam = new GameObject().AddComponent<Camera>();
		blurCam.enabled = false;
	}

    //Handle input
    void Update ()
    {
		HandlePause();

		if (!isResetting && !Info.isPaused)
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
	//Handle player movement and ensure y coord doesn't change
	void HandleMovement()
	{
		movement = Vector3.Normalize(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"))) * movementSpeed;
		movement = transform.TransformDirection(movement);
		CharacterController characterController = GetComponent<CharacterController>();

		if (characterController.enabled)
		{
			characterController.Move(movement * Time.deltaTime);

			Vector3 pos = transform.position;

			if (pos.y != initialY)
			{
				pos.y = initialY;
				transform.position = pos;
			}
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
						SelectCrystal(hit.collider.gameObject);
					}
					else
					{
						GameObject localReference = selectedCrystal;

						DeselectCrystals();

						if (localReference != hit.collider.gameObject)
						{
							SelectCrystal(hit.collider.gameObject);
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
								CrystalInfo crysInfo;

								if (slottedCrystals[i] != null)
                                {
									crysInfo = slottedCrystals[i].GetComponent<CrystalInfo>();

									crystalSlots[i].attachedModule = crysInfo.module;
                                    crystalSlots[i].attachedType = crysInfo.moduleType;
                                }
								else
								{
									crystalSlots[i].attachedModule = "";
									crystalSlots[i].attachedType = -1;
								}
                            }

                            table.ConfirmSpell();
                        }
					}
				}
				else if (hit.collider.gameObject.GetComponent<ClearTable>())
				{
					if(selectedCrystal != null)
					{
						Destroy(selectedCrystal);

						selectedCrystal = null;
					}
					else
					{
						Clear();
					}
				}
			}
		}
	}

	//Select a crystal
	void SelectCrystal(GameObject obj)
	{
		selectedCrystal = obj;

		selectedCrystal.GetComponent<CrystalInfo>().isSelected = true;

		selectedCrystal.GetComponent<Renderer>().material.color = hoverColour;
	}

	//Deselect crystals
	public void DeselectCrystals()
	{
		if(selectedCrystal != null)
		{
			selectedCrystal.GetComponent<CrystalInfo>().isSelected = false;

			switch (selectedCrystal.GetComponent<CrystalInfo>().moduleType)
			{
				case (0):
					selectedCrystal.GetComponent<Renderer>().material.color = primaryDefault;

					break;
				case (1):
					selectedCrystal.GetComponent<Renderer>().material.color = secondaryDefault;

					break;
				case (2):
					selectedCrystal.GetComponent<Renderer>().material.color = effectDefault;

					break;
			}
		}

		selectedCrystal = null;
	}

	//Destroy all slotted crystals
	public void Clear()
	{
		DeselectCrystals();

		for(int i = 0; i < 5; i++)
		{
			slottedCrystals[i] = null;
		}

		clearButton.ClearCrystals();
	}

	//Move crystals to their slot
	IEnumerator SlotInCrystal(GameObject crystal, GameObject slot)
	{
		isCraftCooldown = true;

		int slotNumber = int.Parse(slot.name[6].ToString()) - 1;

		for(int i = 0; i < 5; i++)
		{
			if(slottedCrystals[i] == crystal)
			{
				if(i == slotNumber)
				{
					yield break;
				}
				else
				{
					slottedCrystals[i] = null;
				}
			}
		}

		if (slottedCrystals[slotNumber] != null)
		{
			Destroy(slottedCrystals[slotNumber]);

			crystalSlots[slotNumber].attachedModule = "";
			crystalSlots[slotNumber].attachedType = -1;
		}

		slottedCrystals[slotNumber] = crystal;

		Vector3 origin = crystal.transform.position;
		Vector3 direction = (slot.transform.position + new Vector3(0, 0.1875f, 0)) - origin;
		
		for (int i = 0; i <= 90; i++)
		{
			float rate = i / 90.0f;
			float percentage = 1 / (1 + Mathf.Pow((float)Math.E, -12.5f * (rate - 0.5f)));
			
			crystal.transform.position = origin + direction * percentage;

			yield return new WaitForEndOfFrame();
		}

		crystal.GetComponent<CrystalInfo>().unused = false;

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
		//Storing spells
		if (Input.GetButtonDown("Fire1"))
		{
			RaycastHit hit;

			if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, interactionRange, 1 << LayerMask.NameToLayer("Spell Scroll")))
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
					
					foreach (Renderer renderer in storedSpells[emptySlot].transform.GetChild(0).GetComponentsInChildren<Renderer>())
					{
						renderer.enabled = false;
					}

					foreach (SpriteRenderer render in storedSpells[emptySlot].GetComponent<Spell>().symbolSlots)
					{
						render.enabled = false;
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

		//Cannot select a spel while on casting cooldown
		if (isCastingCooldown)
		{
			return;
		}

		//Numerical spell selection
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
		
		//Scroll spell selection
		float scroll = Input.GetAxis("Mouse ScrollWheel");

		if (scroll != 0)
		{
			int availableSpell = selectedSpell == -1 ? CycleForAvailable(0, 0, 3, 1) : CycleForAvailable(selectedSpell + 3, 1, 3, scroll > 0 ? 1 : -1);
			
			if (availableSpell > -1)
			{
				SelectSpell(availableSpell);
			}
		}
	}
	
	//Cycle through spells to find an available spell
	int CycleForAvailable(int initialIndex, int cycleStart, int cycleCount, int alt)
	{
		int availableSpell = -1;

		for (int i = cycleStart; i < cycleCount; i++)
		{
			if (storedSpells[(initialIndex + i * alt) % 3] != null)
			{
				availableSpell = (initialIndex + i * alt) % 3;

				break;
			}
		}

		return availableSpell;
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

	#region Pause
	//Handle pausing
	void HandlePause()
	{
		if (Input.GetButtonDown("Cancel") || shouldUnpause)// || Input.GetKeyDown(KeyCode.Tab))
		{
			shouldUnpause = false;

			Info.TogglePause();

			Cursor.lockState = Info.isPaused ? CursorLockMode.None : CursorLockMode.Locked;
			Cursor.visible = Info.isPaused;

			//Blurring. doesn't work right now. only a desirable
			/*if (Info.isPaused)
			{
				Texture2D bluredRender = new Texture2D(Screen.width, Screen.height);
				
				blurCam.CopyFrom(Camera.main);

				RenderTexture temporaryRT = RenderTexture.GetTemporary(Screen.width, Screen.height);
				blurCam.targetTexture = temporaryRT;

				blurCam.RenderWithShader(blurShader, "");

				RenderTexture.active = temporaryRT;
				bluredRender.ReadPixels(new Rect(0, 0, temporaryRT.width, temporaryRT.height), 0, 0);
				bluredRender.Apply();

				pauseMenu.texture = bluredRender;

				RenderTexture.ReleaseTemporary(temporaryRT);
			}*/
			
			pauseMenu.gameObject.SetActive(Info.isPaused);
		}
	}

	//Toggle pause from pause menu
	public void TogglePause()
	{
		shouldUnpause = true;
	}
	
	//Return to menu from pause menu
	public void QuitToMenu()
	{
		Info.TogglePause();

		SceneManager.LoadScene("PC Menu");
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