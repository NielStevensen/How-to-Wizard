using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    //Movement speed of the player
    [Tooltip("Movement speed of the player.")]
    public float movementSpeed = 5.0f;

    //Movement expressed as a vector3
    private Vector3 movement = Vector3.zero;

	public GameObject handObject;

	//Raycast info
	public float teleportMaxDistance = 7.5f;
	RaycastHit initialHit;
	RaycastHit secondHit;
	public LineRenderer lineRenderer;

	//Find the teleport point closest to the sampled raycast hit point
	GameObject FindTeleportPoint(Vector2 input)
	{
		Vector2 boundSize = new Vector2(1.25f, 1.25f);
		Vector2 objPos;
		
		foreach (GameObject point in FindObjectOfType<TeleportPointGenerator>().allPoints)
		{
			objPos = new Vector2(point.transform.position.x, point.transform.position.z);

			if (IsInBounds(input, objPos - boundSize, objPos + boundSize))
			{
				return point;
			}
		}

		return null;
	}

	//Determine if the position is close to any teleport points
	bool IsInBounds(Vector2 input, Vector2 areaBottomLeft, Vector2 areaTopRight)
	{
		return areaBottomLeft.x < input.x && input.x < areaTopRight.x && areaBottomLeft.y < input.y && input.y < areaTopRight.y;
	}

	//Set cursor state
	private void Start()
    {
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
    }

    //Handle movement and phone
    void Update ()
    {
		if (Input.GetButton("Fire1"))
		{
			if (Physics.Raycast(handObject.transform.position, handObject.transform.forward, out initialHit, teleportMaxDistance))
			{
				lineRenderer.enabled = true;
				lineRenderer.SetPosition(0, handObject.transform.position);
				lineRenderer.SetPosition(1, initialHit.point);

				Vector3 heightCheckOrigin = initialHit.point - Vector3.Normalize(handObject.transform.forward) * 0.01f + Vector3.up * 0.01f;

				if (Physics.Raycast(heightCheckOrigin, Vector3.down, out secondHit, 1000.0f, ~LayerMask.NameToLayer("Bounds")))
				{
					GameObject targetPoint = FindTeleportPoint(new Vector2(secondHit.point.x, secondHit.point.z));

					if (targetPoint != null)
					{
						lineRenderer.SetPosition(2, heightCheckOrigin);
						lineRenderer.SetPosition(3, secondHit.point);
					}
					else
					{
						lineRenderer.SetPosition(2, initialHit.point);
						lineRenderer.SetPosition(3, initialHit.point);
					}
				}
				else
				{
					lineRenderer.SetPosition(2, initialHit.point);
					lineRenderer.SetPosition(3, initialHit.point);
				}
			}
			else
			{
				lineRenderer.enabled = false;
			}
		}
		else if (Input.GetButtonUp("Fire1"))
		{
			GameObject targetPoint = FindTeleportPoint(new Vector2(secondHit.point.x, secondHit.point.z));
			
			if (targetPoint != null)
			{
				gameObject.GetComponent<CharacterController>().enabled = false;
				gameObject.transform.position = new Vector3(targetPoint.transform.position.x, transform.position.y, targetPoint.transform.position.z);
				gameObject.GetComponent<CharacterController>().enabled = true;
			}
		}
		else
		{
			lineRenderer.enabled = false;
		}

		movement = Vector3.Normalize(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"))) * movementSpeed;
		movement = transform.TransformDirection(movement);
		CharacterController characterController = GetComponent<CharacterController>();

		if (characterController.enabled)
		{
			characterController.Move(movement * Time.deltaTime);
		}
	}
}