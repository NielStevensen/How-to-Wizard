using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PCCraftingZone : MonoBehaviour
{
	//Player references
	private GameObject player;
	private PlayerController controller;
	private float interactionRange;
	private Transform playerCamera;
	private OutlineEffect outlineFX;
	
	//Object reference
	private RaycastHit hit;
	private GameObject target = null;

	//Layer values
	public LayerMask outlineMask;
	private int previousLayer = -1;

	//Set reference
	private void Start()
	{
		player = FindObjectOfType<PlayerController>().gameObject;
		controller = player.GetComponent<PlayerController>();
		interactionRange = controller.interactionRange;
		playerCamera = player.transform.GetChild(0);
		outlineFX = playerCamera.GetComponent<OutlineEffect>();
	}
	
	//Enable outline
	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject == player)
		{
			outlineFX.shouldDrawOutline = true;
		}
	}

	//Disable outline
	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject == player)
		{
			outlineFX.shouldDrawOutline = false;

			if (target != null)
			{
				SetLayer(target, previousLayer);
				target = null;
			}
		}
	}

	//While inside, raycast to highlight stuff
	private void OnTriggerStay(Collider other)
	{
		if (other.gameObject == player)
		{
			GameObject newTarget = null;

			if (Physics.Raycast(playerCamera.position, playerCamera.forward, out hit, interactionRange, outlineMask))
			{
				if (!controller.isCraftCooldown)
				{
					if(hit.collider.gameObject.GetComponent<FinalizeSpell>() == null)
					{
						newTarget = hit.collider.gameObject;
					}
					else
					{
						if (controller.isSpellCollected)
						{
							newTarget = hit.collider.gameObject;
						}
					}
				}
				else
				{
					if(hit.collider.gameObject.GetComponent<Spell>() != null)
					{
						newTarget = hit.collider.gameObject;
					}
				}
				
				//might want to check for crystal info so only crystals can be higlighted
				//can get other crafting objects to do other things
			}

			if(newTarget != target)
			{
				if(target != null)
				{
					SetLayer(target, previousLayer);
				}

				target = newTarget;

				if (target != null)
				{
					previousLayer = target.layer;
					SetLayer(target, LayerMask.NameToLayer("Outline"));
				}
			}
		}
	}

	//Set layer
	void SetLayer(GameObject targetObject, int targetLayer)
	{
		targetObject.layer = targetLayer;

		if (targetObject.GetComponent<Spell>() != null)
		{
			for (int i = 0; i < targetObject.transform.GetChild(0).childCount; i++)
			{
				targetObject.transform.GetChild(0).GetChild(i).gameObject.layer = targetLayer;
			}
		}
	}
}
