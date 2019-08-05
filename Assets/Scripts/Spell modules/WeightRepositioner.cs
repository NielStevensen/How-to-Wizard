using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeightRepositioner : MonoBehaviour
{
	//Try to reposition the weight to not intersect with anything
	private void Start()
	{
		//Position and reposition references
		Vector3 pos = transform.position;
		Vector3 rePos = Vector3.zero;
		
		//Determine if there are any intersections
		RaycastHit[] hits = Physics.BoxCastAll(pos, transform.localScale, Vector3.up, transform.rotation, 0.01f);
		
		//If there was an intersection (there should always be one as an impact must occur to spawn a weight)...
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
	}
}
