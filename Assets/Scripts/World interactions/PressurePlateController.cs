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
    //[Tooltip("The laser this pressure plate activates.")]
    //public List<LaserManager> targetLasers;

    [Space(10)]

	//Pressure plate values
	[Tooltip("The amount of weight required to activate this pressure plate.")]
	public float weightThreshold = 5.0f;
	[Tooltip("The current amount of weight detected.")]
	public float currentWeight = 0.0f;
	[Tooltip("The current activation state of the pressure plate.")]
	public bool isActivated = false;
    private List<WeightedObject> objectsAbove = new List<WeightedObject>();
    private List<WeightedObject> objectsDestroyed = new List<WeightedObject>();
    
    //Check that the objects above it still exist. If not, remove its weight and check activation state
    private void Update()
    {
        foreach (WeightedObject obj in objectsAbove)
        {
            if (obj.obj == null)
            {
                currentWeight -= obj.weight;

                objectsDestroyed.Add(obj);
            }
        }

        if(objectsDestroyed.Count > 0)
        {
            CheckActivationState();

            foreach (WeightedObject obj in objectsDestroyed)
            {
                if (objectsAbove.Contains(obj))
                {
                    objectsAbove.Remove(obj);
                }
            }
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
                objectsAbove.Remove(obj);

                currentWeight -= other.attachedRigidbody.mass;

                CheckActivationState();
            }
        }
	}

	//Check if the detected weight falls in the weight threshold and handle activation accordingly
	void CheckActivationState()
	{
		if(currentWeight >= weightThreshold && !isActivated)
		{
			isActivated = true;

            
		}
		else if(currentWeight < weightThreshold && isActivated)
		{
			isActivated = false;
		}

        foreach (MovingObstacleManager target in targetObstacles)
        {
            target.HandleState(isActivated);
        }

        /*foreach (LaserManager target in targetLasers)
        {
            target.HandleState(isActivated);
        }*/
    }
}
