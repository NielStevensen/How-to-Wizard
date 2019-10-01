using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class VRMovement : MonoBehaviour
{
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
    [Tooltip("Is the head within our designated play area?")]
    public bool isHeadInArea = true;

    [Space(10)]

    //Teleportation variables
    //Raycast info
    [Tooltip("The maximum distance players can teleport.")]
    public float teleportMaxDistance = 7.5f;
    [Tooltip("Layers the teleport raycast ignores.")]
    public LayerMask teleportLayerMask;
    [Tooltip("Teleport ray visualisation object.")]
    public GameObject visualisationPrefab;
    [Tooltip("Valid teleportation material.")]
    public Material validMaterial;
    [Tooltip("Invalid teleportation material.")]
    public Material invalidMaterial;
    private GameObject visualisationObject;
    private Renderer visualisationRenderer;
	private Vector3 teleportLocation;
    private bool isValidTeleport = false;

    [Space(10)]

    //Trackpad movement variables
    //Movement values
    [Tooltip("Movement speed of the player.")]
    public float movementSpeed = 5.0f;
    [Tooltip("Layers the moveemnt raycast ignores.")]
    public LayerMask movementLayerMask;

    private Rigidbody rigidbody;

    //Movement expressed as a vector3
    private Vector3 movement = Vector3.zero;

    //Raycast hit
    private RaycastHit hit;
    

    //Get references, set player y position and set hitbox values
    private void Start()
    {
		if (!Info.IsCurrentlyVR())
		{
			return;
		}

        isTeleportation = Info.optionsData.useTeleportation;

        visualisationObject = Instantiate(visualisationPrefab);
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

        rigidbody = GetComponent<Rigidbody>();
    }

    //Handle hitbox, movement and teleportation
    void Update()
    {
        //Handle hitbox
        if (Physics.Raycast(cameraTransform.position, transform.up * -1, out hit, 1000.0f, movementLayerMask))
        {
            currentPlayfieldSize.y = Mathf.Min(cameraTransform.localPosition.y, 2.0f);
            hitbox.size = currentPlayfieldSize;
            hitbox.center = new Vector3(0, cameraTransform.localPosition.y / 2 + 0.0125f, 0);
        }
        
        //Check if the player in in our designated play area
        isHeadInArea = Mathf.Abs(cameraTransform.localPosition.x) < currentPlayfieldSize.x / 2.0f && Mathf.Abs(cameraTransform.localPosition.z) < currentPlayfieldSize.z / 2.0f;

        if (!isHeadInArea)
        {
            isValidTeleport = false;
            visualisationObject.SetActive(false);

            rigidbody.velocity = Vector3.zero;

            return;
        }

        //Handle locomotion
        if (isTeleportation)
		{
            if (teleportAction.GetLastStateDown(hand))
            {
                visualisationObject.SetActive(true);
            }
            else if (teleportAction.GetLastState(hand))
			{
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
                    visualisationRenderer.material = invalidMaterial;
                    isValidTeleport = false;
                }
                else
                {
                    visualisationRenderer.material = validMaterial;
                    isValidTeleport = true;
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

            rigidbody.velocity = movement * Time.deltaTime;
		}
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
