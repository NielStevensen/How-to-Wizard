using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserController : MonoBehaviour
{
	//Activation values
	[Tooltip("The initial activation state of this laser.")]
	public bool startsActivated = true;
	[Tooltip("Is this laser active?")]
	public bool isActivated = true;

    [Space(10)]

    //Laser values
	[Tooltip("Rotation speed of the laser.")]
	public float rotationSpeed = 0.0f;
	private Vector3 currentRotation = Vector3.zero;
	[Tooltip("Origin of the laser.")]
	public Transform laserOrigin;
    [Tooltip("The layers that the laser should ignore.")]
    public LayerMask laserIgnore;

	//Impact object
	private GameObject impactObject;
	private NullManager nullManager;

	[Space(10)]

	//Laser visualisation
	[Tooltip("Laser visualisation object.")]
	public GameObject laserPrefab;
	private GameObject laserObject;

    public GameObject DestroyFX;

	//Log an error if there is no origin
	void Start()
	{
		Debug.Assert(laserOrigin != null, gameObject.name + " does not have a laser origin!");
		
		isActivated = startsActivated;

		laserObject = Instantiate(laserPrefab, laserOrigin.position, laserOrigin.rotation, laserOrigin);
		laserObject.SetActive(isActivated);
	}
	
	//Handle laser and impact
	void Update()
	{
		laserObject.SetActive(isActivated);

		if (isActivated)
		{
			RaycastHit hit;

			//Debug.DrawLine(laserOrigin.position, laserOrigin.position + Vector3.Normalize(laserOrigin.forward) * 1000.0f, Color.red);

			float laserLength = 1000.0f;

			if (Physics.Raycast(laserOrigin.position, laserOrigin.forward, out hit, 1000.0f, ~laserIgnore))
			{
				laserLength = hit.distance / 2;

				if (hit.collider.gameObject != impactObject)
				{
					impactObject = hit.collider.gameObject;
					nullManager = impactObject.GetComponent<NullManager>();
				}
				
				if (impactObject.tag == "Interactable")
				{
					bool shouldDestroy = true;
					
					if (nullManager != null)
					{
						if (nullManager.IsNulled)
						{
							shouldDestroy = false;
						}
					}
					
					if (shouldDestroy)
					{
						Destroy(impactObject);
                        GameObject FX = Instantiate(DestroyFX);
                        FX.transform.position = impactObject.transform.position;
                        FX.transform.localScale = impactObject.transform.localScale *0.5f;
                        impactObject = null;
					}
				}
			}

			laserObject.transform.localScale = new Vector3(0.0625f, 0.0625f, laserLength);
			laserObject.transform.LookAt(hit.point);

			if (rotationSpeed != 0)
			{
				currentRotation.y += rotationSpeed;
				
				transform.Rotate(new Vector3(0, rotationSpeed, 0));
			}
		}
	}
	
	//Set activation state
	public void HandleState(bool state)
	{
		isActivated = startsActivated != state;
	}
}
