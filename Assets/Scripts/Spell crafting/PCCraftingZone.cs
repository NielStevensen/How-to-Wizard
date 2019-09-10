using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PCCraftingZone : MonoBehaviour
{
	//Player references
	[SerializeField]
	private GameObject player;
	private PlayerController controller;
	private float interactionRange;
	private Transform playerCamera;

	//Shaders
	[Tooltip("Default shader.")]
	public Shader defaultShader;
	[Tooltip("Outline shader.")]
	public Shader outlineShader;

	[Space(10)]

	//Outline colours
	[Tooltip("Hover outline colour.")]
	public Color hoverColour;
	[Tooltip("Selected outline colour.")]
	public Color selectedColour;

	//Object reference
	private RaycastHit hit;
	private GameObject target = null;
	private Renderer targetRenderer = null;
	private CrystalInfo targetInfo = null;
	private bool targetSelectState = false;

	//Set reference
	private void Start()
	{
		player = FindObjectOfType<PlayerController>().gameObject;
		controller = player.GetComponent<PlayerController>();
		interactionRange = controller.interactionRange;
		playerCamera = player.transform.GetChild(0);
	}
	
	private void OnTriggerEnter(Collider other)
	{
		print(other.gameObject.name + ", " + other.gameObject.layer);
	}

	//While inside, raycast to highlight stuff
	private void OnTriggerStay(Collider other)
	{
		if (other.gameObject == player)
		{
			print("player in");

			GameObject newTarget = null;

			if (Physics.Raycast(playerCamera.position, playerCamera.forward, out hit, interactionRange, 1 << LayerMask.NameToLayer("Crafting")))
			{
				newTarget = hit.collider.gameObject;
			}

			if (target != null && newTarget == target)
			{
				if (targetSelectState != targetInfo.isSelected)
				{
					targetSelectState = targetInfo.isSelected;

					ApplyOutline();
				}
			}
			else
			{
				if (target != null)
				{
					RemoveOutline();
				}

				target = newTarget;

				if(target != null)
				{
					targetRenderer = target.GetComponent<Renderer>();
					targetInfo = target.GetComponent<CrystalInfo>();
					targetSelectState = targetInfo == null ? false : targetInfo.isSelected;

					ApplyOutline();
				}	
			}
		}
	}

	//Apply outline
	void ApplyOutline()
	{
		Color temp;

		foreach (Material mat in targetRenderer.materials)
		{
			temp = mat.GetColor("_Color");

			mat.shader = outlineShader;
			
			mat.SetColor("Colour", temp);
			mat.SetColor("_OutlineColour", targetSelectState ? selectedColour : hoverColour);
		}
	}

	//Remove outline
	void RemoveOutline()
	{
		if (targetRenderer != null)
		{
			Color temp;

			foreach (Material mat in targetRenderer.materials)
			{
				temp = mat.GetColor("_Color");

				mat.shader = defaultShader;

				mat.SetColor("Colour", temp);
			}
		}
	}

	//When exiting the crafting zone, deselect everything
	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject == player)
		{
			RemoveOutline();
			
			controller.DeselectCrystals();
		}
	}
}
