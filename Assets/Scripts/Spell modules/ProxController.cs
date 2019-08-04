using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProxController : MonoBehaviour
{
	//Should this object persist? If other of a different set are spawned, desist
	[HideInInspector]
	public bool shouldContinue = true;

	//Reference to module list
	[HideInInspector]
	public SpellModuleList sml;
	
	//Timing value
	[Tooltip("The amount of time the prox field remains without triggering before disappearing.")]
	public float lifeTime = 15.0f;

	//Objects that initially intersected the prox object and will not trigger it until they exit
	private List<GameObject> initialIntersects = new List<GameObject>();

	//AOE object
	[HideInInspector]
	public GameObject proxObj;

	//Whether or not an object has triggered the prox object
	[HideInInspector]
	public bool isTriggered = false;

	//Determine what was already intersecting(and will not trigger this immediately), ready the AOE object and control the object's life
	void Start()
	{
		RaycastHit[] hits = Physics.SphereCastAll(transform.position, transform.localScale.x, Vector3.up, 0.01f);

		foreach(RaycastHit hit in hits)
		{
			if(hit.collider.tag == "Interactable")
			{
				initialIntersects.Add(hit.collider.gameObject);

				print(initialIntersects[initialIntersects.Count - 1].name);
			}
		}

		proxObj = Instantiate(sml.aoePrefab, transform.position, Quaternion.identity);
		proxObj.transform.localScale = transform.localScale;
		proxObj.SetActive(false);

		StartCoroutine(ControlLifeTime());
	}

	//Detect objects and store appropriate values
	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Interactable")
		{
			if (!initialIntersects.Contains(other.gameObject))
			{
				proxObj.SetActive(true);

				isTriggered = true;
			}
		}
	}

	//Allow objects that leave its area to be detected if they re-enter
	private void OnTriggerExit(Collider other)
	{
		initialIntersects.Remove(other.gameObject);
	}

	//Control the lifetime of hte prox object
	IEnumerator ControlLifeTime()
	{
		float elapsedTime = 0.0f;

		while(shouldContinue && elapsedTime < lifeTime)
		{
			elapsedTime += Time.deltaTime;

			yield return new WaitForEndOfFrame();
		}

		Destroy(proxObj);

		shouldContinue = false;
		isTriggered = true;
	}
}
