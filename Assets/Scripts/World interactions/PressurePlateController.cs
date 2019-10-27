using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class to hold a reference to an object registered by the pressure plate and its mass
class WeightedObject
{
    public GameObject obj;
    public float weight = 0;

    public WeightedObject(GameObject _obj, float _weight)
    {
        obj = _obj;
        weight = _weight;
    }
}

public class PressurePlateController : MonoBehaviour
{
	//Target objects
	[Tooltip("The moving obstacles this pressure plate activates.")]
	public List<MovingObstacleManager> targetObstacles;
    [Tooltip("The laser this pressure plate activates.")]
    public List<LaserController> targetLasers;
	[Tooltip("Associated mechanism symbols.")]
	public List<SpriteRenderer> targetSymbols;
	private List<ParticleSystem> symbolPFX = new List<ParticleSystem>();

    [Space(10)]

	//Pressure plate values
	[Tooltip("The amount of weight required to activate this pressure plate.")]
	public float weightThreshold = 5.0f;
	[Tooltip("The current amount of weight detected.")]
	public float currentWeight = 0.0f;
	[Tooltip("The current activation state of the pressure plate.")]
	public bool isActivated = false;
    private List<WeightedObject> objectsAbove = new List<WeightedObject>();
    private List<WeightedObject> objectsRemoved = new List<WeightedObject>();

	//Set references
	private void Start()
	{
		foreach(SpriteRenderer obj in targetSymbols)
		{
			foreach(ParticleSystem pfx in obj.GetComponentsInChildren<ParticleSystem>())
			{
				symbolPFX.Add(pfx);
			}
		}
	}

	//Check that the objects above it still exist. If not, remove its weight and check activation state
	private void Update()
    {
        foreach (WeightedObject obj in objectsAbove)
        {
            if (obj.obj == null)
            {
                currentWeight -= obj.weight;

                objectsRemoved.Add(obj);
            }
        }

        if(objectsRemoved.Count > 0)
        {
            CheckActivationState();

            foreach (WeightedObject obj in objectsRemoved)
            {
                if (objectsAbove.Contains(obj))
                {
                    objectsAbove.Remove(obj);
                }
            }

			objectsRemoved.Clear();
        }
    }

    //On moving above the pressure plate, note its presence and weight
    private void OnTriggerEnter(Collider other)
	{
		if(other.tag == "Interactable")
		{
            objectsAbove.Add(new WeightedObject(other.gameObject, other.attachedRigidbody.mass));

			currentWeight += other.attachedRigidbody.mass;

			CheckActivationState();
		}
	}

	//On moving off the pressure plate, note its absence and reduction in weight
	private void OnTriggerExit(Collider other)
	{
        foreach(WeightedObject obj in objectsAbove)
        {
            if(obj.obj == other.gameObject)
            {
				currentWeight -= obj.weight;
				
				objectsRemoved.Add(obj);
				
                CheckActivationState();
            }
        }

		if (objectsRemoved.Count > 0)
		{
			CheckActivationState();

			foreach (WeightedObject obj in objectsRemoved)
			{
				if (objectsAbove.Contains(obj))
				{
					objectsAbove.Remove(obj);
				}
			}

			objectsRemoved.Clear();
		}
	}

	//Check if the detected weight falls in the weight threshold and handle activation accordingly
	void CheckActivationState()
	{
		if(currentWeight >= weightThreshold && !isActivated)
		{
			isActivated = true;

			UpdateSymbols(true);
		}
		else if(currentWeight < weightThreshold && isActivated)
		{
			isActivated = false;

			UpdateSymbols(false);
		}

        foreach (MovingObstacleManager target in targetObstacles)
        {
            target.HandleState(isActivated);
        }

        foreach (LaserController target in targetLasers)
        {
            target.HandleState(isActivated);
        }
    }

	//Update mechanism symbols
	void UpdateSymbols(bool state)
	{
		for(int i = 0; i < targetSymbols.Count; i++)
		{
			targetSymbols[i].color = state ? Color.red : Color.white;

			if (state)
			{
				symbolPFX[i].Play();
			}
			else
			{
				symbolPFX[i].Stop();
			}
		}
	}
}
