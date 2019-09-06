using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeightRepositioner : MonoBehaviour
{
	//Objects already tested for intersection. If intersected again, the space is too small for this weight
	private List<GameObject> intersectedObjects = new List<GameObject>();
	
	//Try to reposition the weight to not intersect with anything
	private void Start()
	{
		//References
		Vector3 pos = transform.position;
		Vector3 rePos = Vector3.zero;
		Collider weightCollider = GetComponent<Collider>();

		//Find everything the weight overlaps
		RaycastHit[] hits = Physics.BoxCastAll(pos, transform.localScale, Vector3.up, transform.rotation, 0.01f);
		
		//Foreach overlapping object, determine how to separate it from the weight
		if(hits.Length > 0)
		{
			Vector3 direction;
			float distance;

			foreach (RaycastHit hit in hits)
			{
				if (hit.collider.gameObject != gameObject)
				{
					if (Physics.ComputePenetration(weightCollider, transform.position, transform.rotation,
					hit.collider, hit.collider.gameObject.transform.position, hit.collider.gameObject.transform.rotation,
					out direction, out distance))
					{
						print(hit.collider.gameObject.name + ": " + direction + ", " + distance);

						transform.position += direction * distance;

						//rePos += direction * distance * 1.1f;
					}
				}
			}
		}

		//Apply repositioning value
		//transform.position += rePos;

		/*
		//Position and reposition references
		Vector3 pos = transform.position;
		Vector3 rePos = Vector3.zero;
		
		//Determine if there are any intersections
		RaycastHit[] hits = Physics.BoxCastAll(pos, transform.localScale, Vector3.up, transform.rotation, 0.01f);
		
		//If there was an intersection (there should always be one as an impact must occur to spawn a weight)
		if(hits.Length > 0)
		{
			foreach(RaycastHit hit in hits)
			{
				//If what was intersected is not the weight itself...
				if (hit.collider.gameObject != gameObject)
				{
					float distance = Vector3.Distance(pos, hit.point);
					//print("Distance between weight and point of intersection: " + distance);
					//print(hit.point);
					print(hit.normal);

		//issue: when intersecting at start of sweep, point returns zero vector
		//to fix: manually figure out what the hit point is to use the correct value
		//solution: the closest point will be from the position of the weight, in the opposite of the normal of the hit. therefore, to find the right point, raycast from the weight in the direction that is opposite the normal
		//but apparently, the normal is set to opposite the direction of the sweep if intersecting at start of sweep. so, how to find the normal?
					
					Vector3 direction = hit.point - pos;
					RaycastHit objHit;

					//Raycast from just away from the centre of the weight...
					if (Physics.Raycast(pos - Vector3.Normalize(direction) * 0.01f, direction, out objHit, 1000, ~(1 << LayerMask.NameToLayer("Ignore Raycast"))))
					{
						//
						if (objHit.collider.gameObject == hit.collider.gameObject)
						{
							print(objHit.distance);
							print("Reposition amount: " + objHit.normal * ((transform.localScale.x / 2) - objHit.distance));

							rePos += objHit.normal * ((transform.localScale.x / 1.9f) - objHit.distance);
						}
						else
						{
							print("Weight somehow spawned entirely within another object!? Raycast hit: " + objHit.collider.gameObject.name + ". Original object to check for: " + hit.collider.gameObject.name);
						}
					}
					//Somehow, the raycast didn't hit anything... ?
					else
					{
						print("Weight somehow spawned entirely within another object!? Raycast did not hit anything");
					}
				}
			}

			//Apply repositioning value
			transform.position += rePos;
		}
		*/
	}

	//weight module issues:
	//-how to spawn not intersecting with objects
	//	-need to account for rotation
	//		-the corners stick out more than the centre of a face.how to account for this?
	//		-can compare the angle to forward to determine how close where the raycast is to a corner
	//		-use an equation to increase how much the weight is displaced based on proximity to a corner
	//	-still need to account for when, even after these calculations, the weight still sits in a place where it intersects stuff
	//		-e.g.spawned in a low ceiling area or by prox(crushed between the floor and something above that triggered the prox)

	//-approach to weight repositioning
	//	-draw a triangle to determine values:
	//	-get distance between pos of object and pos of weight for hypotenuse
	//	-

	//new approach:
	//-on start, bool isIntersecting = true;
	//-while isintersecting:
	//	-boxcast weight's position
	//		-if no collision, isintersecting = false, break
	//		-if there is a collision, check if the object is already listed under intersected objects
	//			-if not, add the object to list of intersected objects
	//			-run calculations to determine how to move the object so it is no longer intersecting
	
	//repositioning calculations
	//-raycast from just away from the centre to the impacted object
	//-without complex, curved colliders, this will provide the face the weight is intersecting
	//-use the hit.normal to determine the direction to move the weight
	//-then, raycast from just away from the centre of the weight in the direction opposite the normal retrieved and move the weight based on the distance of the raycast
	//	-assuming there are no curves or diagonals, this will work perfectly fine
	//	-to make sure this works, boxcastall and see if the weight still intersects the object
	//	-if it does, do these calculations again
	//		-but what if this gives diminishing returns and it loops through an infinite number of times?
}
