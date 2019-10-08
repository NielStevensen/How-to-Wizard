using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManualTiling : MonoBehaviour
{
	//Base materials
	[Tooltip("Base floor material.")]
	public Material baseFloorMaterial;
	[Tooltip("Base wall material.")]
	public Material baseWallMaterial;
	[Tooltip("Base fence material.")]
	public Material baseFenceMaterial;
	[Tooltip("Base ceiling material.")]
	public Material baseCeilingMaterial;

	[Space(10)]

	//Tiling size
	[Tooltip("Size of floor tiling.")]
	public Vector2 floorTileSize = Vector2.one;
	[Tooltip("Size of wall tiling.")]
	public Vector2 wallTileSize = Vector2.one;
	[Tooltip("Size of fence tiling.")]
	public Vector2 fenceTileSize = Vector2.one;
	[Tooltip("Size of ceiling tiling.")]
	public Vector2 ceilingTileSize = Vector2.one;

	//Tiled materials
	private List<Material> floorMaterials = new List<Material>();
	private List<Material> wallMaterials = new List<Material>();
	private List<Material> fenceMaterials = new List<Material>();
	private List<Material> ceilingMaterials = new List<Material>();

	//Generate tiled materials
	void Start()
    {
		if(baseFloorMaterial == null || baseWallMaterial == null || baseFenceMaterial == null || baseCeilingMaterial == null)
		{
			return;
		}

		int boundsLayer = LayerMask.NameToLayer("Bounds");

		List<GameObject> floorObjects = new List<GameObject>();
		List<GameObject> wallObjects = new List<GameObject>();
		List<GameObject> fenceObjects = new List<GameObject>();
		List<GameObject> ceilingObjects = new List<GameObject>();

		Material temp = null;
		
		foreach (GameObject obj in FindObjectsOfType<GameObject>())
		{
			if(obj.layer == boundsLayer)
			{
				if(obj != gameObject)
				{
					switch (obj.name.Split(' ')[0])
					{
						case ("Floor"):
							temp = new Material(baseFloorMaterial);
							temp.mainTextureScale = new Vector2(obj.transform.localScale.x * floorTileSize.x, obj.transform.localScale.z * floorTileSize.y);

							break;
						case ("Wall"):
							temp = new Material(baseWallMaterial);
							temp.mainTextureScale = new Vector2(obj.transform.localScale.x * wallTileSize.x, obj.transform.localScale.y * wallTileSize.y);

							break;
						case ("Fence"):
							temp = new Material(baseFenceMaterial);
							temp.mainTextureScale = new Vector2(obj.transform.localScale.x * fenceTileSize.x, obj.transform.localScale.y * fenceTileSize.y);

							break;
						case ("Ceiling"):
							temp = new Material(baseCeilingMaterial);
							temp.mainTextureScale = new Vector2(obj.transform.localScale.x * ceilingTileSize.x, obj.transform.localScale.z * ceilingTileSize.y);

							break;
					}

					floorMaterials.Add(temp);

					obj.GetComponent<Renderer>().material = temp;
				}
			}
		}
    }

	//Destroy mateial instances for garbage collection
	public void ClearMaterials()
	{
		foreach (Material mat in floorMaterials)
		{
			if(mat != null)
			{
				DestroyImmediate(mat);
			}
		}

		foreach (Material mat in wallMaterials)
		{
			if (mat != null)
			{
				DestroyImmediate(mat);
			}
		}

		foreach (Material mat in fenceMaterials)
		{
			if (mat != null)
			{
				DestroyImmediate(mat);
			}
		}

		foreach (Material mat in ceilingMaterials)
		{
			if (mat != null)
			{
				DestroyImmediate(mat);
			}
		}
	}
}
