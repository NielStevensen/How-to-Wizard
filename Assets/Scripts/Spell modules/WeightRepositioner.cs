using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeightRepositioner : MonoBehaviour
{
	//Objects already tested for intersection. If intersected again, the space is too small for this weight
	[SerializeField]
	private List<GameObject> intersectedObjects = new List<GameObject>();

	[Tooltip("The layers that weight repositioning should ignore.")]
	public LayerMask collisionIgnore;
	
	//stuff to try:
		//print scale before penetration test to determine if scale is set correctly
		//print calculated position based on output distance and direction to determine if this matches where the weightmoves to
		//do penetration test 1 frame after scaling
		//start the weight with collider disabled and enable after repositioning
		//also start without gravity and enable after repositioning

	//Try to reposition the weight to not intersect with anything
	private void Start()
	{
		//print(transform.localScale);
		
		//References
		Vector3 pos = transform.position;
		Vector3 rePos = Vector3.zero;
		Collider weightCollider = GetComponent<Collider>();

		//Find everything the weight overlaps
		RaycastHit[] hits = Physics.BoxCastAll(pos, transform.localScale / 2.0f, Vector3.up, transform.rotation, 0.01f, collisionIgnore, QueryTriggerInteraction.Ignore);
		
		foreach(RaycastHit hit in hits)
		{
			intersectedObjects.Add(hit.collider.gameObject);
		}
		
		//Foreach overlapping object, determine how to separate it from the weight
		if(hits.Length > 0)
		{
			Vector3 direction;
			float distance;

			int count = 0;

			foreach (RaycastHit hit in hits)
			{
				if (hit.collider.gameObject != gameObject)
				{
					print(hit.collider.gameObject.name + ": " + count++);

					bool isCollidedPenetration = Physics.ComputePenetration(weightCollider, transform.position, transform.rotation,
					hit.collider, hit.collider.gameObject.transform.position, hit.collider.gameObject.transform.rotation,
					out direction, out distance);

					bool isCollidedBoxcast = Physics.BoxCast(transform.position, transform.localScale / 2.0f, Vector3.up, transform.rotation, 0.01f, collisionIgnore, QueryTriggerInteraction.Ignore);

					print("penetration: " + isCollidedPenetration.ToString() + ", boxcast:" + isCollidedBoxcast.ToString());

					print(direction * distance);
					
					rePos += direction * distance;
				}
			}

			transform.position += rePos;
		}

		weightCollider.attachedRigidbody.isKinematic = false;
		weightCollider.attachedRigidbody.useGravity = true;
	}
}
