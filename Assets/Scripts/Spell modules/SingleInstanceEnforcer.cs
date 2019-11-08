using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleInstanceEnforcer : MonoBehaviour
{
	//Current universal spell ID
	[Tooltip("Current universal spell ID.")]
	public int currentSpellID = 0;
	
	//IDs of objects and references to them
	private float timerID = -1;
	private List<GameObject> timerList = new List<GameObject>();
	private float proxID = -1;
	private List<GameObject> proxList = new List<GameObject>();
	private float weightID = -1;
	private List<GameObject> weightList = new List<GameObject>();
	private float barrierID = -1;
	private List<GameObject> barrierList = new List<GameObject>();
	
	//Spawn an object as part of the same set. If another set exists, destroy it
	public GameObject SpawnAsSet(float spellID, GameObject prefab, string module, Vector3 pos)
	{
		GameObject obj = Instantiate(prefab, pos, Quaternion.identity);
		
		switch (module)
		{
			case "Timer":
				if (spellID != timerID)
				{
					timerID = spellID;
					
					foreach (GameObject timer in timerList)
					{
						if(timer != null)
						{
							timer.GetComponent<TimerController>().shouldContinue = false;
						}
					}

					timerList.Clear();
				}
				
				timerList.Add(obj);
				
				break;
			case "Prox":
				if (spellID != proxID)
				{
					proxID = spellID;
					
					foreach (GameObject prox in proxList)
					{
						if (prox != null)
						{
							prox.GetComponent<ProxController>().shouldContinue = false;
						}
					}

					proxList.Clear();
				}
				
				proxList.Add(obj);
				
				break;
			case "Weight":
				if (spellID != weightID)
				{
					weightID = spellID;
					
					foreach (GameObject weight in weightList)
					{
						if (weight != null)
						{
                            Destroy(weight);
						}
					}

					weightList.Clear();
				}
				
				weightList.Add(obj);
				
				break;
			case "Barrier":
				if (spellID != barrierID)
				{
					barrierID = spellID;
					
					foreach (GameObject barrier in barrierList)
					{
						if (barrier != null)
						{
							Destroy(barrier);
						}
					}

					barrierList.Clear();
				}
				
				barrierList.Add(obj);
				
				break;
		}
		
		return obj;
	}
}
