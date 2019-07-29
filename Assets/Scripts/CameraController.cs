using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    //Determine if horizontal or vertical input should be taken.
    [Tooltip("The body should take horizontal input. The camera should take vertical input.")]
    public bool isRotatingHorizontally = false;

    [Space(10)]

    //Mouse sensitivity
    [Tooltip("Horizontal mouse sensitivity.")]
    public float mouseSensitivityX = 3.75f;
    [Tooltip("Vertical mouse sensitivity.")]
    public float mouseSensitivityY = 3.75f;
    
    [Space(10)]

    //Vertical rotation clamping values
    [Tooltip("Furthest down the player can look.")]
    public float minVerticalRotation = -45.0f;
    [Tooltip("Furthest up the player can look.")]
    public float maxVerticalRotation = 45.0f;

    [Space(10)]

    //Mouse inversion
    [Tooltip("Should vertical mouse input be inverted?")]
    public bool isMouseInverted = false;
    private int mouseInversion;

	[Space(10)]
	
	//Current rotation
	[HideInInspector]
	public float rotationHorizontal;
	[HideInInspector]
    public float rotationVertical;
	
	//Set settings values
	private void Awake()
	{
		mouseSensitivityX = PlayerPrefs.GetFloat("MouseSensitivityX", 3.75f);
		mouseSensitivityY = PlayerPrefs.GetFloat("MouseSensitivityY", 3.75f);

		isMouseInverted = PlayerPrefs.GetInt("MouseInversion", -1) == 1;
	}

	//Only render debug stuff if in editor and desired, initialise mouse inversion and set initial rotation
	void Start ()
    {
		if (isMouseInverted)
        {
            mouseInversion = -1;
        }
        else
        {
            mouseInversion = 1;
        }

        rotationHorizontal = transform.rotation.eulerAngles.y;
        rotationVertical = transform.rotation.eulerAngles.x;
    }
	
	//Handle camera
	void Update ()
    {

			if (isRotatingHorizontally)
			{
				float rotationValue = 0;

				rotationValue = Input.GetAxis("Mouse X") * mouseSensitivityX;

				rotationHorizontal += rotationValue;

				Quaternion quaternionHorizontal = Quaternion.AngleAxis(rotationHorizontal, Vector3.up);

				transform.localRotation = Quaternion.identity * quaternionHorizontal;
			}
			else
			{
				float rotationValue = 0;
				rotationValue = Input.GetAxis("Mouse Y") * mouseSensitivityY * mouseInversion;
				rotationVertical += rotationValue;
				rotationVertical = Mathf.Clamp(rotationVertical, minVerticalRotation, maxVerticalRotation);
				Quaternion quaternionVertical = Quaternion.AngleAxis(rotationVertical, Vector3.left);
				transform.localRotation = Quaternion.identity * quaternionVertical;
			}
	}
}
