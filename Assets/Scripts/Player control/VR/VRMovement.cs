﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Valve.VR;

public class VRMovement : MonoBehaviour
{
    #region Variables
    //Movement control scheme
    [Tooltip("Whether the player is moving via trackpad or teleportation.")]
	public bool isTeleportation = true;

    [Space(10)]

    //Controller variables
    [Tooltip("The hand that controls movement.")]
    public SteamVR_Input_Sources hand;
    [Tooltip("The object representing the controlling hand.")]
	public GameObject handObject;
    private Transform cameraTransform;
    private Transform rotationReference;
    [Tooltip("Teleportation input.")]
    public SteamVR_Action_Boolean teleportAction;
    [Tooltip("Movement input.")]
    public SteamVR_Action_Vector2 moveAction;

	[Space(10)]

	//hitbox variables
	[Tooltip("Minimum playfield size.")]
    public Vector2 minPlayfieldSize = new Vector2(0.25f, 0.25f);
    [Tooltip("Maximum playfield size.")]
    public Vector2 maxPlayfieldSize = new Vector2(2.5f, 2.5f);
    [Tooltip("Calculated designated playfield size.")]
    public Vector3 currentPlayfieldSize = new Vector3(0.25f, 2.0f, 0.25f);
    private BoxCollider hitbox;
	[Tooltip("Player area visualiser object.")]
	public Transform playAreaVisualiser;
	private Renderer pavRenderer;
	[Tooltip("Player in area visualiser colour.")]
	public Material inAreaColour;
	[Tooltip("Player out of area visualiser colour.")]
	public Material outOfAreaColour;
	[Tooltip("Is the head within our designated play area?")]
    public bool isHeadInArea = true;
	private bool wasHeadInArea = true;

    [Space(10)]

    //Teleportation variables
    //Raycast info
    [Tooltip("The maximum distance players can teleport.")]
    public float teleportMaxDistance = 7.5f;
    [Tooltip("Layers the teleport raycast ignores.")]
    public LayerMask teleportLayerMask;
    [Tooltip("Teleportation destination visualisation object.")]
    public GameObject visualisationObject;
	private Renderer visualisationRenderer;
	[Tooltip("Valid teleportation material.")]
    public Material validMaterial;
    [Tooltip("Invalid teleportation material.")]
    public Material invalidMaterial;
	private Vector3 teleportLocation;
    private bool isValidTeleport = false;

    [Space(10)]

    //Trackpad movement variables
    //Movement values
    [Tooltip("Movement speed of the player.")]
    public float movementSpeed = 5.0f;
    [Tooltip("Layers the moveemnt raycast ignores.")]
    public LayerMask movementLayerMask;

    private Rigidbody playerBody;

    //Movement expressed as a vector3
    private Vector3 movement = Vector3.zero;

    //Raycast hit
    private RaycastHit hit;

    //Fade out coroutine for out of bounds
    [Tooltip("The layer that is used to check for VR bounds.")]
    public LayerMask boundsLayermask;
    private int fadeProgress = 0;
    private Coroutine fadeCoroutine = null;
    public Image fadeBackdrop;
    public Text fadeText;
    private bool isInBounds = true;

    //menu variables
    public SteamVR_Action_Boolean menuButton;
    public SteamVR_Action_Boolean Activate;
    private bool shouldUnpause = false;
    private bool togglePause;
    public GameObject menuLayout;
    public LayerMask pausedLayers;
    public GameObject pointer;
    public Image Instuction;
    public Sprite[] menuPictures;
    private bool[] unlockedPictures = new bool[21];
    int currentText;

	//Fade transition values
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

	//Get references, set player y position and set hitbox values
	private void Start()
    { 
        
		if (!Info.IsCurrentlyVR())
		{
			return;
		}

		pointer = Instantiate(pointer);
        menuLayout.transform.SetParent(null); // seperate from vr view so raycsts work properly

        isTeleportation = Info.optionsData.useTeleportation;

		pavRenderer = playAreaVisualiser.GetComponent<Renderer>();
        visualisationRenderer = visualisationObject.GetComponent<Renderer>();

		cameraTransform = GetComponentInChildren<Camera>().transform;
        rotationReference = GetComponentInChildren<InheritYRotation>().transform;

        if (Physics.Raycast(cameraTransform.position, transform.up * -1, out hit, 1000.0f, movementLayerMask))
        {
            Vector3 temp = transform.position;
            temp.y = hit.point.y + 0.0125f;

            transform.position = temp;
        }
        else
        {
            this.enabled = false;
        }
        
        hitbox = GetComponent<BoxCollider>();
        Vector2 size = DeterminePlayAreaSize();
        currentPlayfieldSize = new Vector3(size.x, 2.0f, size.y);
        hitbox.size = currentPlayfieldSize;
		playAreaVisualiser.localScale = new Vector3(size.x, playAreaVisualiser.localScale.y, size.y);

        playerBody = GetComponent<Rigidbody>();

		StartCoroutine(HandleTransitionFade(startFadeDuration, Color.black, new Color(0, 0, 0, 0)));

        //check unlocked pictures baserd on progression
        PlayerData data = SaveSystem.LoadGame();
        unlockedPictures[20] = true;
        for (int  i = 0; i < 8;i++)
        {
            unlockedPictures[i] = true;
        }
        if(data.storyClearData[0])
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
    }

    //Handle hitbox, movement and teleportation
    void Update()
    {
		if (shouldFadeRestrictControl)
		{
			return;
		}
		
		//Handle hitbox
        if (Physics.Raycast(cameraTransform.position, transform.up * -1, out hit, 1000.0f, movementLayerMask))
        {
            currentPlayfieldSize.y = Mathf.Min(cameraTransform.localPosition.y, 2.0f);
            hitbox.size = currentPlayfieldSize;
            hitbox.center = new Vector3(0, cameraTransform.localPosition.y / 2 + 0.0125f, 0);
        }

        //Determine if the player is in bounds
        bool isCurrentlyInBounds = Physics.Raycast(cameraTransform.position + new Vector3(0, 500, 0), transform.up * -1, out hit, 1000.0f, boundsLayermask);

        if(isCurrentlyInBounds != isInBounds)
        {
            isInBounds = isCurrentlyInBounds;

            if(fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }
            
            fadeCoroutine = StartCoroutine(Fade(isCurrentlyInBounds ? -1 : 1));
        }
        
        //Check if the player is in our designated play area
        if (Mathf.Abs(cameraTransform.localPosition.x) < currentPlayfieldSize.x / 2.0f && Mathf.Abs(cameraTransform.localPosition.z) < currentPlayfieldSize.z / 2.0f)
		{
			if (!wasHeadInArea)
			{
				wasHeadInArea = true;

				pavRenderer.material = inAreaColour;
			}
		}
		else
		{
			if (wasHeadInArea)
			{
				wasHeadInArea = false;

				pavRenderer.material = outOfAreaColour;
			}

            isValidTeleport = false;
			visualisationObject.SetActive(false);

			playerBody.velocity = Vector3.zero;

			return;
		}

        //Handle locomotion
        if (!Info.isPaused)
        {
            if (isTeleportation)
            {
                if (teleportAction.GetLastState(hand))
                {
                    if (!visualisationObject.activeInHierarchy)
                    {
                        visualisationObject.SetActive(true);
                        visualisationRenderer.material = invalidMaterial;
                    }

                    Vector3 destination = transform.position;

                    if (Physics.Raycast(handObject.transform.position, handObject.transform.forward, out hit, teleportMaxDistance, teleportLayerMask))
                    {
                        destination = new Vector3(hit.point.x, transform.position.y + hitbox.center.y, hit.point.z);
                    }
                    else
                    {
                        destination = new Vector3(handObject.transform.position.x + handObject.transform.forward.x * teleportMaxDistance, transform.position.y + hitbox.center.y, handObject.transform.position.z + handObject.transform.forward.z * teleportMaxDistance);
                    }

                    visualisationObject.transform.position = destination;
                    visualisationObject.transform.localScale = hitbox.size;

                    RaycastHit[] hit2 = Physics.BoxCastAll(visualisationObject.transform.position, visualisationObject.transform.localScale / 2, Vector3.up * 0.01f, Quaternion.identity, 0.01f, teleportLayerMask);

                    if (hit2.Length > 0)
                    {
                        if (isValidTeleport)
                        {
                            visualisationRenderer.material = invalidMaterial;
                            isValidTeleport = false;
                        }
                    }
                    else
                    {
                        if (!isValidTeleport)
                        {
                            visualisationRenderer.material = validMaterial;
                            isValidTeleport = true;
                        }
                    }
                }
                else if (teleportAction.GetLastStateUp(hand))
                {
                    visualisationObject.SetActive(false);

                    if (isValidTeleport)
                    {
                        transform.position = new Vector3(visualisationObject.transform.position.x, transform.position.y, visualisationObject.transform.position.z);

                        isValidTeleport = false;
                    }
                }
            }
            else
            {
                movement = Vector3.Normalize(new Vector3(moveAction.GetAxis(hand).x, 0, moveAction.GetAxis(hand).y)) * movementSpeed;
                movement = rotationReference.TransformDirection(movement);

                playerBody.velocity = movement * Time.deltaTime;
            }
        }

        if(togglePause)
        {
            togglePause = false;
            Info.TogglePause();
            shouldUnpause = false;
            if (Info.isPaused == false)
            {              
                menuLayout.SetActive(false);
            }
            pointer.SetActive(Info.isPaused);
        }

        //Menu + pausing
        if (menuButton.GetStateDown(hand))
        {
            menuLayout.SetActive(!Info.isPaused);
            togglePause = true;

            if (!Info.isPaused)
            {
                menuLayout.transform.position = new Vector3(transform.position.x, cameraTransform.position.y, transform.position.z) + rotationReference.transform.forward * ((currentPlayfieldSize.x + currentPlayfieldSize.z) / 4.0f);
                menuLayout.transform.LookAt(cameraTransform);
            }         
        }

        //while paused
        if(Info.isPaused)
        {
            pointer.transform.position = handObject.transform.position;
            pointer.transform.rotation = handObject.transform.rotation;
            pointer.transform.localScale = Physics.Raycast(handObject.transform.position, handObject.transform.forward, out RaycastHit Output, 1000f, pausedLayers) ? new Vector3(0.1f, 0.1f, Output.distance / 2) : new Vector3(0.1f, 0.1f, 100f);

            //Pause menu interaction
            if ((Activate.GetStateDown(hand)))
            {
                if (Physics.Raycast(handObject.transform.position, handObject.transform.forward, out Output, 1000f, pausedLayers))
                {
                    if (Output.collider.gameObject.name == "Resume")
                    {
                        shouldUnpause = true;
                        togglePause = true;
                    }
                    else if (Output.collider.gameObject.name == "Exit")
                    {
                        StartCoroutine(HandleQuitFade());
                    }
                    else if(Output.collider.gameObject.name == "Next")
                    {
                        cycletext(currentText + 1, 1);
                    }
                    else if (Output.collider.gameObject.name == "Previous")
                    {
                        cycletext(currentText - 1, -1);
                    }
                }

            }
        }
    }

    public void cycletext(int start,int direction)
    {
        bool found = false;
        for(int i = start; i < 21 && i >= 0; i += direction)
        {
            if(unlockedPictures[i] && found == false)
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
            cycletext(20, -1);
        }
    }

    //Out of bounds fade out coroutine
    IEnumerator Fade(int alt)
    {
        float percent = 0;
        Color imageColour = fadeBackdrop.color;
        Color textColour = fadeText.color;
        
        while (0 <= fadeProgress && fadeProgress <= 180)
        {
            fadeProgress += alt;
            
            percent = fadeProgress / 150.0f;

            imageColour.a = Mathf.Min(percent, 1);
            textColour.a = Mathf.Min(percent * 2, 1);

            fadeBackdrop.color = imageColour;
            fadeText.color = textColour;

            yield return new WaitForEndOfFrame();
        }
        
        if (alt == 1)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else
        {
            fadeProgress = 0;
        }
    }

	//Handle fading the screen for scene transitions
	IEnumerator HandleTransitionFade(float fadeDuration, Color initialColour, Color finalColour)
	{
		float elapsedTime = 0.0f;
		float percentage;

		while (elapsedTime < fadeDuration)
		{
			elapsedTime += Time.deltaTime;

			percentage = Mathf.Min(elapsedTime / fadeDuration, 1.0f);

			fadeImage.color = Color.Lerp(initialColour, finalColour, percentage);

			yield return new WaitForEndOfFrame();
		}
	}

	//Common transition fade functionality
	void InitialiseFadeOut()
	{
		shouldFadeRestrictControl = true;

		foreach(PickupSpell hand in FindObjectsOfType<PickupSpell>())
		{
			hand.isFading = true;
		}
	}

	//Handle the fade at the end of a level
	public Coroutine HandleLevelEndFadeVR()
	{
		InitialiseFadeOut();

		return StartCoroutine(HandleTransitionFade(finishFadeDuration, new Color(0, 0, 0, 0), Color.black));
	}

	//Call reset fade functionality
	public void CallResetFade()
	{
		StartCoroutine(HandleResetFade());
	}

	//Handle the fade when resetting
	IEnumerator HandleResetFade()
	{
		InitialiseFadeOut();
		
		yield return StartCoroutine(HandleTransitionFade(resetFadeDuration, new Color(0, 0, 0, 0), Color.black));

		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}

	//Handle the fade when quitting from the pause menu
	IEnumerator HandleQuitFade()
	{
		InitialiseFadeOut();

		Info.TogglePause();

		yield return StartCoroutine(HandleTransitionFade(quitFadeDuration, new Color(0, 0, 0, 0), Color.black));

		SceneManager.LoadScene("VR Menu");
	}

	//Determine the size of the player's play area and use to calculate hitbox size
	Vector2 DeterminePlayAreaSize()
    {
        HmdQuad_t hmdRef = new HmdQuad_t();
        OpenVR.Chaperone.GetPlayAreaRect(ref hmdRef);
        
        float maxX = Mathf.Max(Mathf.Abs(hmdRef.vCorners0.v0 - hmdRef.vCorners2.v0), Mathf.Abs(hmdRef.vCorners1.v0 - hmdRef.vCorners3.v0));
        float maxZ = Mathf.Max(Mathf.Abs(hmdRef.vCorners0.v2 - hmdRef.vCorners2.v2), Mathf.Abs(hmdRef.vCorners1.v2 - hmdRef.vCorners3.v2));

        return new Vector2(Mathf.Max(Mathf.Min(maxX, maxPlayfieldSize.x), minPlayfieldSize.x), Mathf.Max(Mathf.Min(maxZ, maxPlayfieldSize.y), minPlayfieldSize.y));
    }
}
