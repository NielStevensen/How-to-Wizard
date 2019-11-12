using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeightRepositioner : MonoBehaviour
{
	//Layermask
	[Tooltip("The layers that weight repositioning should ignore.")]
	public LayerMask weightMask;
    public AudioClip weightSound;
    public GameObject weightFX;
	//Try to reposition the weight to not intersect with anything
	private void Start()
	{
		//Intersection values
		bool isIntersecting = true;
		RaycastHit[] hits;
		List<RaycastHit> hits_ = new List<RaycastHit>();

		//Object references
		List<GameObject> testedObjects = new List<GameObject>();
		GameObject target;

		//Repositioning values
		Vector3 direction;
		float distance;
		Vector3 repos;
		Vector3 reposTotal = Vector3.zero;
		
		//Collider reference
		BoxCollider weightCollider = GetComponent<BoxCollider>();
		weightCollider.size *= transform.localScale.x;

		//Handle repositioning
		while (isIntersecting)
		{
			//Determine all objects intersecting with the weight
			hits = Physics.BoxCastAll(transform.position, transform.localScale * 0.5f - Vector3.one * 0.01f, Vector3.up, transform.rotation, 0.01f, weightMask, QueryTriggerInteraction.Ignore);

			hits_.Clear();

			foreach(RaycastHit hit in hits)
			{
				if(hit.collider.gameObject != gameObject)
				{
					hits_.Add(hit);
				}
			}
			
			//If there were no intersections, the space is clear and the weight can be spawned
			if(hits_.Count == 0)
			{
				print("no longer intersecting");

				isIntersecting = false;
			}
			//Else, try to reposition
			else
			{
				target = hits_[0].collider.gameObject;

				//Determine if the target object has been tested
				bool alreadyTested = false;

				foreach(GameObject obj in testedObjects)
				{
					if(obj == target)
					{
						alreadyTested = true;

						print("double test. object: " + target.name);

						break;
					}
				}

				//If the target object has already been tested, the weight is in too small a space
				if (alreadyTested)
				{
					break;
				}

				testedObjects.Add(target);

				//Determine repositioning amount
				Physics.ComputePenetration(weightCollider, transform.position, transform.rotation, hits_[0].collider, target.transform.position, target.transform.rotation, out direction, out distance);
				
				repos = direction * distance / transform.localScale.x;
				reposTotal += repos;

				//If the weight requires too much repositioning, it's in too small a space
				if(reposTotal.magnitude > transform.localScale.x * 1.5f)
				{
					print("moved too far. move distance: " + reposTotal.magnitude);

					break;
				}

				//Apply repositioning
				transform.position += repos;
			}
		}

		//The weight failed its repositioning
		if (isIntersecting)
		{
			//produce particle effect

			//gameObject.GetComponent<Renderer>().enabled = true;

			Destroy(gameObject);
		}
		//The weight was successfully repositioned
		else
		{
			weightCollider.isTrigger = false;
			weightCollider.size = Vector3.one;
			weightCollider.attachedRigidbody.isKinematic = false;
			weightCollider.attachedRigidbody.useGravity = true;
			gameObject.GetComponent<Renderer>().enabled = true;

            //produce particle effect
            AudioSource.PlayClipAtPoint(weightSound, transform.position, Info.optionsData.sfxLevel); // oc do not steal
            //GameObject FX = Instantiate(weightFX);
            //FX.transform.position = transform.position;
        }
    }
}
