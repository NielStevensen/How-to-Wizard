using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportPointGenerator : MonoBehaviour
{
	//Field size
	[Tooltip("How many units forwards the field extends.")]
	public int forwardField = 5;
	[Tooltip("How many units left the field extends.")]
	public int leftField = 5;
	[Tooltip("How many units backwards the field extends.")]
	public int backwardField = 5;
	[Tooltip("How many units right the field extends.")]
	public int rightField = 5;

	[Space(10)]

	//Point prefab
	[Tooltip("Prefab of the teleport point.")]
	public GameObject pointPrefab;

	//Distance between each point
	private float pointDistance = 2.5f;
	
	//Start
    void Start()
    {
		PlacePoints();
    }

	//Place points in a grid in suitable areas
	void PlacePoints()
	{
		RaycastHit hit;

		Vector3 origin = transform.position;
		Vector3 position;
		float posX;

		for (int x = -leftField; x < rightField + 1; x++)
		{
			posX = origin.x + x * pointDistance;

			for (int y = -backwardField; y < forwardField + 1; y++)
			{
				position = new Vector3(posX, origin.y, origin.z + y * pointDistance);

				if (Physics.Raycast(position, Vector3.down, out hit, 2.5f, 1 << LayerMask.NameToLayer("Teleport Point")))
				{
					Instantiate(pointPrefab, position, Quaternion.identity, transform);
				}
			}
		}
	}
}
