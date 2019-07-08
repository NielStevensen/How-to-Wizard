using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;

public class PlayerController : MonoBehaviour {

    //Movement speed of the player
    [Tooltip("Movement speed of the player.")]
    public float movementSpeed = 5.0f;

    //Movement expressed as a vector3
    private Vector3 movement = Vector3.zero;
	
    //Reference to SocialMediaManager
    private SocialMediaManager smm;
	
    //Used for player response logic
    [HideInInspector]
    public bool wasNegativeCommentPosted = false;
	private float negativeCommentDelay;

    //Current index of player responses
    [HideInInspector]
    public int responseIndex = 1;
    
    //UI objects
    private GameObject phone;
    private GameObject responseArea;
    private GameObject responseCursor;
	private GameObject turnOffArea;
	private GameObject turnOffCursor;

    //Has the player started/finished?
    private bool hasGameStarted = false;
	[HideInInspector]
    public bool hasGameFinished = false;

    [Space(10)]

    //How long you have to hold the button to turn off the phone
    [Tooltip("The length of time in seconds the player must hold 'E' to turn off the phone.")]
    public float turnOffTime = 1.0f;

	//Is the player trying to turn their phone off
	private bool isTryingToTurnOff = false;
	private int turnOffIndex = 0;

	[Space(10)]

	//Fade stuff
	[Tooltip("The time in seconds taken to fade out of black when starting the game.")]
	public float fadeOutTime = 1.0f;
	[Tooltip("The time in seconds taken to fade into white for the good ending.")]
	public float fadeInTime = 1.0f;
	private Image fadeImage;

	//Prompt text stuff
	private Text promptText;
	private Coroutine fadeCoroutine;

	[Space(10)]

	//Saturation stuff
	[Tooltip("The final saturation reached when turning off the phone.")]
	[Range(-100, 100)]
	public float turnOffTargetSaturation = 100.0f;
	[Tooltip("The time in seconds taken to shift to turnOffTargetSaturation.")]
	public float turnOffSaturationTime = 3.75f;
	[Tooltip("The final saturation reached when all AI reach max negativity.")]
	[Range(-100, 100)]
	public float negativeTargetSaturation = -100.0f;
	[Tooltip("The time in seconds taken to shift to negativeTargetSaturation.")]
	public float negativeSaturationTime = 3.75f;

	[Space(10)]
	
	//Max negativity purgatory
	[Tooltip("The time in seconds before the max negativity end actually occurs.")]
	public float maxNegativityEndDelay = 30.0f;

	//Is the player allowed to turn the phone off? Not allowed when the negative or neutral endings are triggered
	[HideInInspector]
	public bool isAllowedToTurnOff = true;
	private AudioSource audioSource;

	//InteractionsManager
	private InteractionsManager interactionsManager;
	
    //Set references and values
    private void Start()
    {
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		
        smm = FindObjectOfType<SocialMediaManager>();

		negativeCommentDelay = smm.negativeCommentDelay;
        
        phone = GameObject.Find("image base");
        responseArea = GameObject.Find("image container");
        responseCursor = GameObject.Find("image cursor");
        turnOffArea = GameObject.Find("image turn off");
        turnOffCursor = GameObject.Find("image cursorX");

		smm.playerProfilePicture = GameObject.Find("Image Player profile pic").GetComponent<RectTransform>();

		phone.SetActive(false);
        responseArea.SetActive(false);
		turnOffArea.SetActive(false);

		fadeImage = GameObject.FindGameObjectWithTag("FadeImage").GetComponent<Image>();

		FadeTransition.FadeEffect(fadeImage, fadeInTime, 1, 0);

		promptText = GameObject.Find("Instruction text").GetComponent<Text>();

		fadeCoroutine = StartCoroutine(FadePromptText(1));

		audioSource = GetComponent<AudioSource>();

		interactionsManager = gameObject.GetComponent<InteractionsManager>();
    }

    //Handle movement and phone
    void Update ()
    {
		if (!hasGameStarted)
		{
			if (Input.GetButtonDown("Turn Off"))
			{
				StartCoroutine(ConfirmTurnOff());
			}
		}
		else if(!hasGameFinished)
		{
			movement = Vector3.Normalize(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"))) * movementSpeed;
			movement = transform.TransformDirection(movement);
			
			if (CheckConfirmKeys())
			{
				if (!isTryingToTurnOff)
				{
					if (wasNegativeCommentPosted)
					{
						DisableReplies();
							
						smm.CalculateReputation();

						smm.replyEndTime = -1;
					}
				}
				else
				{
					if (turnOffIndex == 0)
					{
						isTryingToTurnOff = false;
						
						turnOffArea.SetActive(false);
					}
					else
					{
						if (isAllowedToTurnOff && (!interactionsManager.isSitting || (interactionsManager.isSitting && interactionsManager.isSeated)))
						{
							hasGameFinished = true;

							smm.shouldNotProduceComments = true;

							phone.SetActive(false);
							turnOffArea.SetActive(false);

							movement = Vector3.zero;

							CameraController[] cameras = GameObject.FindObjectsOfType<CameraController>();

							foreach (CameraController camera in cameras)
							{
								camera.hasGameStarted = false;
							}

							StartCoroutine(FadeOutGame());
						}
						else
						{
							audioSource.PlayOneShot(smm.commentNotificationSounds[1]);
						}
					}
				}
			}

			if (Input.GetAxisRaw("Mouse ScrollWheel") != 0)
			{
				if (!isTryingToTurnOff)
				{
					if (wasNegativeCommentPosted)
					{
						float newEndTime = Time.time + negativeCommentDelay;

						if(newEndTime > smm.replyEndTime)
						{
							smm.replyEndTime = newEndTime;
						}

						responseIndex = Mathf.Clamp(responseIndex - Mathf.Clamp((int)Input.GetAxisRaw("Mouse ScrollWheel"), -1, 1), 0, 2);

						responseCursor.transform.localPosition = new Vector3(-187.5f, (responseIndex - 1) * -62.5f, 0);
					}
				}
				else
				{
					turnOffIndex = Mathf.Clamp(turnOffIndex + Mathf.Clamp((int)Input.GetAxisRaw("Mouse ScrollWheel"), -1, 1), 0, 1);

					turnOffCursor.transform.localPosition = new Vector3(-87.5f, -50 + turnOffIndex * 37.5f, 0);
				}
			}

			if (isAllowedToTurnOff)
			{
				if (Input.GetButtonDown("Turn Off"))
				{
					StartCoroutine(ConfirmTurnOff());
				}
			}
		}

		CharacterController characterController = GetComponent<CharacterController>();
		
		movement.y = -9.8f;

		if (characterController.enabled)
		{
			characterController.Move(movement * Time.deltaTime);
		}
	}

	//Get if scroll wheel or space is pressed
	bool CheckConfirmKeys()
	{
		return Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2) || Input.GetButtonDown("Jump");
	}

	//Enable reply area and reset the cursor
	public void EnableReplies()
	{
		wasNegativeCommentPosted = true;

		responseArea.SetActive(true);

		responseIndex = 1;

		responseCursor.transform.localPosition = new Vector3(-187.5f, (responseIndex - 1) * 62.5f, 0);
	}

	//Disable reply area
	public void DisableReplies()
	{
		wasNegativeCommentPosted = false;
		
		responseArea.SetActive(false);
	}

	//Check if the turn off button is held. If so, enter a turn off 'state'
	IEnumerator ConfirmTurnOff()
    {
        float endTime = Time.time + turnOffTime;
        bool stillHeld = true;

        while(Time.time < endTime && stillHeld)
        {
            if(Input.GetButtonUp("Turn Off"))
            {
                stillHeld = false;
            }

            yield return new WaitForEndOfFrame();
        }

        if (stillHeld)
        {
			if (hasGameStarted)
			{
				isTryingToTurnOff = true;

				turnOffArea.SetActive(true);
			}
			else
			{
				hasGameStarted = true;

				FindObjectOfType<AIManager>().CallBellTimer();

				CameraController[] cameras = GameObject.FindObjectsOfType<CameraController>();

				foreach(CameraController camera in cameras)
				{
					camera.hasGameStarted = true;
				}

				smm.shouldStart = true;
				
				phone.SetActive(true);

				gameObject.GetComponent<FootstepHandler>().enabled = true;
				
				if(fadeCoroutine != null)
				{
					StopCoroutine(fadeCoroutine);
				}

				StartCoroutine(FadePromptText(-1));

				gameObject.GetComponent<InteractionsManager>().enabled = true;
			}
        }
    }

	//'Turn off the phone' ending. Increase saturation and increase ambient sound volume, then fade to white and close then game
	IEnumerator FadeOutGame()
	{
		FindObjectOfType<MusicHandler>().CallFadeMusic(turnOffSaturationTime);

		ColorGrading colourGrading;

		FindObjectOfType<PostProcessVolume>().profile.TryGetSettings(out colourGrading);

		SaturationHandler.HandleSaturation(colourGrading, 0.0f, turnOffTargetSaturation, turnOffSaturationTime);

		FindObjectOfType<SoundFadeHandler>().CallHandleSound();

		yield return new WaitForSeconds(turnOffSaturationTime + 1.25f);

		if (fadeImage == null)
		{
			Debug.Assert(true, "There is no image tagged \"FadeImage\" in the scene!");
		}
		else
		{
			FindObjectOfType<MusicHandler>().CallFadeAudio(fadeOutTime);

			FadeTransition.FadeEffect(fadeImage, Color.white, fadeOutTime, 0, 1);

			yield return new WaitForSeconds(fadeOutTime);

			Application.Quit();
		}
	}

	//Call ForceRestart. Called by AIManager
	public void CallForceEnd()
	{
		StartCoroutine(ForceEnd());
	}

	//'Max negativity' ending. Wait a while, then decrease saturation and then fade to black
	IEnumerator ForceEnd()
	{
		yield return new WaitForSeconds(maxNegativityEndDelay);

		ColorGrading colourGrading;

		FindObjectOfType<PostProcessVolume>().profile.TryGetSettings(out colourGrading);

		SaturationHandler.HandleSaturation(colourGrading, 0.0f, negativeTargetSaturation, negativeSaturationTime);
		
		yield return new WaitForSeconds(negativeSaturationTime);

		StartCoroutine(FadeEnd(fadeOutTime));
	}

	//Call FadeEnd
	public void CallFadeEnd(float fadeTime)
	{
		StartCoroutine(FadeEnd(fadeTime));
	}

	//Fade to black and close the game
	IEnumerator FadeEnd(float fadeTime)
	{
		if (fadeImage == null)
		{
			Debug.Assert(true, "There is no image tagged \"FadeImage\" in the scene!");
		}
		else
		{
			FindObjectOfType<MusicHandler>().CallFadeAudio(fadeTime);

			FadeTransition.FadeEffect(fadeImage, fadeTime, 0, 1);

			yield return new WaitForSeconds(fadeTime);

			Application.Quit();
		}
	}

	//Fade prompt text
	IEnumerator FadePromptText(int alt)
	{
		float targetAlpha = 0.5f + 0.5f * alt;

		Outline promptOutline = promptText.GetComponent<Outline>();

		Color textColour = promptText.color;
		Color outlineColour = promptOutline.effectColor;

		while (promptText.color.a != targetAlpha)
		{
			float increment = 0.025f * alt;
			
			textColour.a = Mathf.Clamp(textColour.a + increment, 0, 1);
			outlineColour.a = Mathf.Clamp(outlineColour.a + increment, 0, 1); ;

			promptText.color = textColour;
			promptOutline.effectColor = outlineColour;

			yield return new WaitForEndOfFrame();
		}
	}
}