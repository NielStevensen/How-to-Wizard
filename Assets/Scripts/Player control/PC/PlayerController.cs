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
	[Tooltip("Speed modifier while running.")]
	public float runModifier = 1.5f;
	private float initialY;

    //Movement expressed as a vector3
    private Vector3 movement = Vector3.zero;

	[Space(10)]

	//Crafting values
	[Tooltip("Crafting and scroll interaction range.")]
	public float interactionRange = 3.75f;
	private Transform cameraTransform;
	[HideInInspector]
	public bool isCraftCooldown = false;
	private float craftCooldown = 2.5f;
	[HideInInspector]
	public GameObject selectedCrystal = null;
	[HideInInspector]
	public GameObject[] slottedCrystals = new GameObject[5] { null, null, null, null, null };
	private AttachCrystal[] crystalSlots = new AttachCrystal[5];
	private Animator[] slotAnimators = new Animator[5];
	private SpellCreation table = null;
	private ClearTable clearButton = null;
	[Tooltip("The layers crafting objects can be on.")]
	public LayerMask craftingMask;
	private Color previousColour;
	[Tooltip("The colour selected crystals adopt.")]
	public Color selectedColour;

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
	[HideInInspector]
	public Animator animator;
	public static int startCastHash;
	public static int throwHash;
	public static int chargeHash;
	public static int endChargeHash;
	public static int touchHash;
	public static int cancelCastHash;

	[Space(10)]

	//Reset values
	[Tooltip("The amount of time in seconds the reset button must he held to reset.")]
	public float resetTime = 2.5f;
	private bool canReset = false;
	private bool isResetting = false;

	[Space(10)]

	//Pause values
	[Tooltip("Pause UI.")]
	public RawImage pauseMenu;
	private bool shouldUnpause = false;
	private Camera blurCam;
	[Tooltip("Pause blur shader.")]
	public Shader blurShader;
    //Pause tips
    public Image Instuction;
    public Sprite[] menuPictures;
    public bool[] unlockedPictures = new bool[20];
    int currentText;

    [Space(10)]

	//Fade values
	[Tooltip("Fade image.")]
	public Image fadeImage;
	private bool shouldFadeRestrictControl = false;
	[Tooltip("Level start fade in time in seconds.")]
	public float startFadeDuration = 1.25f;
	[Tooltip("Level reset fade in time in seconds.")]
	public float resetFadeDuration = 0.375f;
	[Tooltip("Level finish fade in time in seconds.")]
	public float finishFadeDuration = 2.5f;
	[Tooltip("Level quit fade in time in seconds.")]
	public float quitFadeDuration = 0.375f;
	#endregion

	//Set cursor state and set references
	private void Start()
    {
        PlayerData data = SaveSystem.LoadGame();
        for (int i = 0; i < 8; i++)
        {
            unlockedPictures[i] = true;
        }
        if (data.storyClearData[0])
        {
            unlockedPictures[8] = true;
        }
        if (data.storyClearData[1])
        {
            unlockedPictures[9] = true;
            unlockedPictures[10] = true;
        }
        if (data.storyClearData[3])
        {
            unlockedPictures[11] = true;
            unlockedPictures[12] = true;
        }
        if (data.storyClearData[5])
        {
            unlockedPictures[13] = true;
            unlockedPictures[14] = true;
        }
        if (data.storyClearData[6])
        {
            unlockedPictures[15] = true;
            unlockedPictures[16] = true;
        }
        if (data.storyClearData[7])
        {
            unlockedPictures[17] = true;
            unlockedPictures[18] = true;
        }
        if (data.storyClearData[8])
        {
            unlockedPictures[19] = true;
        }
        unlockedPictures[20] = true;

        initialY = transform.position.y;

		cameraTransform = transform.GetChild(0);
		table = FindObjectOfType<SpellCreation>();
		clearButton = table.GetComponentInChildren<ClearTable>();
		craftCooldown = table.craftCooldown;

		crystalSlots = table.PCSpellSlots;
		
		for(int i = 11; i < 16; i++)
		{
			slotAnimators[i - 11] = table.transform.GetChild(i).GetComponent<Animator>();
		}

		for(int i = 0; i < 3; i++)
		{
			for (int j = 0; j< 5; j++)
			{
				storageIcons[i, j] = storageUI[i].transform.GetChild(j).gameObject.GetComponent<Image>();
			}
		}

		animator = cameraTransform.GetComponentInChildren<Animator>();
		startCastHash = Animator.StringToHash("startCast");
		throwHash = Animator.StringToHash("throw");
		chargeHash = Animator.StringToHash("charge");
		endChargeHash = Animator.StringToHash("endCharge");
		touchHash = Animator.StringToHash("touch");
		cancelCastHash = Animator.StringToHash("cancelCast");

		StartCoroutine(WaitUntilRelease());

		StartCoroutine(HandleFade(startFadeDuration, Color.black, new Color(0, 0, 0, 0)));

		//blurCam = new GameObject().AddComponent<Camera>();
		//blurCam.enabled = false;
	}

    //Handle input
    void Update ()
    {
		if (shouldFadeRestrictControl)
		{
			return;
		}

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
		}
	}

	#region Movement
	//Handle player movement and ensure y coord doesn't change
	void HandleMovement()
	{
		movement = Vector3.Normalize(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"))) * movementSpeed;
		movement *= Input.GetButton("Run") ? runModifier : 1.0f;
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

			if(Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, interactionRange, craftingMask))
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
							isCraftCooldown = true;

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

		Renderer crystalRenderer = selectedCrystal.GetComponent<Renderer>();
		previousColour = crystalRenderer.material.color;
		crystalRenderer.material.color = selectedColour;
		selectedCrystal.GetComponentInChildren<ParticleSystem>().startColor = selectedColour;
		
		foreach(Animator anim in slotAnimators)
		{
			anim.enabled = true;
		}
	}

	//Deselect crystals
	public void DeselectCrystals()
	{
		if(selectedCrystal != null)
		{
			selectedCrystal.GetComponent<CrystalInfo>().isSelected = false;

			selectedCrystal.GetComponent<Renderer>().material.color = previousColour;
			selectedCrystal.GetComponentInChildren<ParticleSystem>().startColor = previousColour;
		}

		selectedCrystal = null;

		for(int i = 0; i < 5; i++)
		{
			if(slottedCrystals[i] == null)
			{
				slotAnimators[i].enabled = false;
			}
		}
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
		int slotNumber = int.Parse(slot.name[6].ToString()) - 1;

		for(int i = 0; i < 5; i++)
		{
			if(slottedCrystals[i] == crystal)
			{
				if(i == slotNumber)
				{
					DeselectCrystals();

					yield break;
				}
				else
				{
					slottedCrystals[i] = null;
				}
			}
		}

		isCraftCooldown = true;

		if (slottedCrystals[slotNumber] != null)
		{
			Destroy(slottedCrystals[slotNumber]);

			crystalSlots[slotNumber].attachedModule = "";
			crystalSlots[slotNumber].attachedType = -1;
		}

		slottedCrystals[slotNumber] = crystal;

		for (int i = 0; i < 5; i++)
		{
			if (slottedCrystals[i] == null)
			{
				slotAnimators[i].enabled = false;
			}
		}

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

		DeselectCrystals();

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

			if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, interactionRange, 1 << LayerMask.NameToLayer("Outline")))
			{
				if(hit.collider.gameObject.GetComponent<Spell>() != null)
				{
					int emptySlot = -1;

					for (int i = 0; i < 3; i++)
					{
						if (storedSpells[i] == null)
						{
							emptySlot = i;

							break;
						}
					}

					if (emptySlot > -1)
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

						for (int i = 0; i < moduleList.Count; i++)
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
		}

		//Cannot select a spell while on casting cooldown
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
			if(selectedSpell > -1)
			{
				animator.SetTrigger(cancelCastHash);
				StartCoroutine(HandleCastingCooldown(-2));
			}

			selectedSpell = -1;

			selectionBorder.localPosition = new Vector3(-1000.0f, -475, 0);
		}
		else
		{
			if (selectedSpell == -1)
			{
				animator.SetTrigger(startCastHash);
				StartCoroutine(HandleCastingCooldown(-1));
			}

			selectedSpell = slot;

			selectionBorder.localPosition = new Vector3(-1000.0f + (slot + 1) * 100.0f, -475, 0);
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
				StartCoroutine(HandleCastingCooldown(selectedSpell));

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

	//Handle casting cooldown (and stop casting while bringing up/putting down hand)
	IEnumerator HandleCastingCooldown(int index)
	{
		isCastingCooldown = true;

		if(index > -1)
		{
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
		}

		bool isRaisingHand = index == -1;

		while (isRaisingHand ? animator.GetCurrentAnimatorStateInfo(0).IsName("Neutral") : !animator.GetCurrentAnimatorStateInfo(0).IsName("Neutral"))
		{
			yield return new WaitForEndOfFrame();
		}
		
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
			if (!Input.GetButton("Reset") || shouldFadeRestrictControl)
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
			shouldFadeRestrictControl = true;

			yield return StartCoroutine(HandleFade(resetFadeDuration, new Color(0, 0, 0, 0), Color.black));

			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		}
	}

	//Prevent holding q after a successful reset starting another reset
	IEnumerator WaitUntilRelease()
	{
		yield return new WaitForSeconds(startFadeDuration);

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
	
	//Call quit functionality
	public void CallQuit()
	{
		StartCoroutine(QuitToMenu());
	}

    public void cycletext(int direction)
    {
        int start = currentText + direction;
        bool found = false;
        for (int i = start; i < 21 && i >= 0; i += direction)
        {
            if (unlockedPictures[i] && found == false)
            {
                currentText = i;
                found = true;
            }
        }
        if (found)
        {
            Instuction.sprite = menuPictures[currentText];

        }
        else if (direction == 1)
        {
            // cycle back to start
            Instuction.sprite = menuPictures[0];
            currentText = 0;
        }
        else if (direction == -1)
        {
            // run again from the top
            currentText = 21;
            cycletext(-1);
        }
    }

    //Return to menu from pause menu
    IEnumerator QuitToMenu()
	{
		shouldFadeRestrictControl = true;

		Info.TogglePause();

		yield return StartCoroutine(HandleFade(quitFadeDuration, new Color(0, 0, 0, 0), Color.black));
		
		SceneManager.LoadScene("PC Menu");
	}
	#endregion

	#region Fade
	//Handle fading the screen for scene transitions
	IEnumerator HandleFade(float fadeDuration, Color initialColour, Color finalColour)
	{
		float elapsedTime = 0.0f;
		float percentage;

		while(elapsedTime < fadeDuration)
		{
			elapsedTime += Time.deltaTime;

			percentage = Mathf.Min(elapsedTime / fadeDuration, 1.0f);

			fadeImage.color = Color.Lerp(initialColour, finalColour, percentage);

			yield return new WaitForEndOfFrame();
		}
	}

	//Handle the fade at the end of a level
	public Coroutine HandleLevelEndFadePC()
	{
		shouldFadeRestrictControl = true;

		return StartCoroutine(HandleFade(finishFadeDuration, new Color(0, 0, 0, 0), Color.black));
	}
	#endregion
}