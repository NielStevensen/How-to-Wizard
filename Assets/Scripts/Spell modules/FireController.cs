using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireController : MonoBehaviour
{
	//Lifetime
	[Tooltip("Lifetime of the flame object.")]
	public float lifetime = 5;

	//Values to handle null
	private List<NullManager> nullObjects = new List<NullManager>();
	private List<NullManager> objectsToRemove = new List<NullManager>();

    //Setup
    void Start()
    {
        if(GetComponentInChildren<AudioSource>() != null)
        {
            GetComponentInChildren<AudioSource>().volume = Info.optionsData.sfxLevel;
        }
		if(lifetime > 0.0f)
		{
			Destroy(gameObject, lifetime);
		}
	}
	
	//On entering the flame, set flammable, un-nulled objects on fire
	private void OnTriggerEnter(Collider other)
    {
		BurnController bc = other.gameObject.GetComponent<BurnController>();

		if (bc != null)
		{
			bool canBurn = true;

			NullManager nm = other.gameObject.GetComponent<NullManager>();

			if(nm != null)
			{
				if (nm.IsNulled)
				{
					nullObjects.Add(nm);

					canBurn = false;
				}
			}

			bc.enabled = canBurn;
		}
    }

	//Check the null state of objects to see if they can be set on fire
	private void OnTriggerStay(Collider other)
	{
		foreach(NullManager nm in nullObjects)
		{
			if (!nm.IsNulled)
			{
				other.gameObject.GetComponent<BurnController>().enabled = true;

				objectsToRemove.Add(nm);
			}
		}

		foreach(NullManager nm in objectsToRemove)
		{
			if (nullObjects.Contains(nm))
			{
				nullObjects.Remove(nm);
			}
		}
	}

	//Remove nulled objects from the list to check
	private void OnTriggerExit(Collider other)
	{
		if (nullObjects.Contains(other.GetComponent<NullManager>()))
		{
			nullObjects.Remove(other.GetComponent<NullManager>());
		}
	}
}
