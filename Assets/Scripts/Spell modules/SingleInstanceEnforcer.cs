using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleInstanceEnforcer : MonoBehaviour
{
	//Current universal spell ID
	[Tooltip("Current universal spell ID.")]
	public int currentSpellID = 0;
	
	//IDs of objects and references to them
	private int timerID = -1;
	private List<GameObject> timerList = new List<GameObject>();
	private int proxID = -1;
	private List<GameObject> proxList = new List<GameObject>();
	private int weightID = -1;
	private List<GameObject> weightList = new List<GameObject>();
	private int barrierID = -1;
	private List<GameObject> barrierList = new List<GameObject>();
	
	//Spawn an object as part of the same set. If another set exists, destroy it
	public GameObject SpawnAsSet(int spellID, GameObject prefab, string module, float potency, Vector3 pos)
	{
		GameObject obj = Instantiate(prefab, pos, Quaternion.identity);
		
		//Maybe look into pointers? Could seriously clean up this code (has certain compile requirements)
		//Create a method that takes pointers to the relevant id and list of gameobjects
		//This means the contents of each switch case can be put into the same method
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
							//Set a bool in each object of the old set to false
							//This bool is checked every frame
							//If false, stop functionality and fade/destroy
							
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
							//prox.GetComponent<ModuleSpecificComponent>().shouldContinue = false;
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
							//weight.GetComponent<ModuleSpecificComponent>().DestrotObject();
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
							//barrier.GetComponent<ModuleSpecificComponent>().DestrotObject();
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
