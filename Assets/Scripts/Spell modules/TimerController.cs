using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerController : MonoBehaviour
{
	//Should this object persist? If other of a different set are spawned, desist
	[HideInInspector]
	public bool shouldContinue = true;

	//Reference to module list
	[HideInInspector]
	public SpellModuleList sml;

	//Spell potency
	[HideInInspector]
	public float potency = -1;

	//Timing value
	[Tooltip("The base amount of time before activation at 1 potency.")]
	public float baseTime = 10.0f;
	
	//AOE object
	[HideInInspector]
	public GameObject timerObj;

	//Whether or not the timer has depleted
	[HideInInspector]
	public bool isDepleted = false;

	//Start timer
    void Start()
    {
		StartCoroutine(TimerModule());
    }
	
	//Delay activation
	IEnumerator TimerModule()
	{
		float elapsedTime = 0.0f;
		float endTime = baseTime * potency;
		
		timerObj = Instantiate(sml.aoePrefab, transform.position, Quaternion.identity);
		timerObj.transform.localScale = transform.localScale;
		timerObj.SetActive(false);
		
		while (elapsedTime < endTime && shouldContinue)
		{
			yield return new WaitForEndOfFrame();

			elapsedTime += Time.deltaTime;
		}
		
		if (shouldContinue)
		{
			timerObj.SetActive(true);

			yield return new WaitForEndOfFrame();

			isDepleted = true;
		}
		else
		{
			isDepleted = true;

			Destroy(timerObj);
		}
	}
}
