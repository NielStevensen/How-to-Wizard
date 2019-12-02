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
				//Handle highlighting for crystals, slots, creation and clear. Can only be highlighted while not on crafting cooldown
				if (!controller.isCraftCooldown)
				{
					//Crystals and spell scrolls can highlight whenever while not on crafting cooldown
					if (hit.collider.gameObject.GetComponent<CrystalInfo>() != null || hit.collider.gameObject.GetComponent<Spell>() != null)
					{
						newTarget = hit.collider.gameObject;
					}
					//Slots can only highlight ifn a crystal is selected
					else if (hit.collider.gameObject.GetComponent<AttachCrystal>() != null)
					{
						newTarget = controller.selectedCrystal != null ? hit.collider.gameObject : null;
					}
					//Spell creation and clear only highlight if there is at least 1 slotted crystal
					else
					{
						bool areCrystalsSlotted = false;

						foreach (GameObject obj in controller.slottedCrystals)
						{
							if (obj != null)
							{
								areCrystalsSlotted = true;

								break;
							}
						}

						if (areCrystalsSlotted)
						{
							newTarget = hit.collider.gameObject.GetComponent<ClearTable>() != null ? hit.collider.gameObject : controller.isSpellCollected ? hit.collider.gameObject : null;
						}
					}
				}
				//Spell scrolls can be highlighted at any time
				else
				{
					if(hit.collider.gameObject.GetComponent<Spell>() != null)
					{
						newTarget = hit.collider.gameObject;
					}
				}
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
