﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;
using Valve.VR;

public class SpellModuleList : MonoBehaviour
{
	#region Variables
	//VR hand reference
	[HideInInspector]
    public SteamVR_Input_Sources hand;
    [HideInInspector]
    public Transform handTransform;
    [HideInInspector]
    public PickupSpell obj;

    //References
    private Spell spell;
	private SingleInstanceEnforcer sie;

	//Spell ID
	[Tooltip("Spell ID.")]
	public int spellID = -1;

	//Initial rotation of the player when the spell is cast. Used during instantiation
	private Quaternion playerRotation;
    public GameObject rotationReference;

	[Space(10)]

	//Projectile values
	public float projectileSpeedMultiplier = 10.0f;
    [HideInInspector]
    public Vector3 projectileVelocity;
    [HideInInspector]
    public Vector3 projectileAngularV;
	[HideInInspector]
	public bool activeprojectile = true;

	[Space(10)]

	//Charge values
	[Tooltip("The amount of time in seconds charge must be held to achieve maximum charge.")]
	public float maxChargeTime = 2.5f;
    private LineRenderer lineRenderer;
    public SteamVR_Action_Boolean holdAction;
	
    [Space(10)]

    //AOE values
    [Tooltip("A multiplier on how much potency affects AOE range.")]
	public float aoeSizeAmplifier = 2.5f;

    [Space(10)]

	//Push/pull values
	[Tooltip("The max distace from the first point of contact that push/pull force is applied.")]
	public float maxForceDistance = 7.5f;
	[Tooltip("The maximum amount of force applied by the push/pull modules.")]
	public float maxForce = 25.0f;

    [Space(10)]

    #region Prefabs
    //Prefabs
    [Tooltip("Projectile object spawned by projectile and split modules.")]
	public GameObject projectilePrefab;
	[Tooltip("AOE object spawned by AOE module.")]
	public GameObject aoePrefab;
	[Tooltip("Proximity object spawned by proximity module.")]
	public GameObject proxPrefab;
	[Tooltip("Timer object spawned by timer module.")]
    public GameObject timerPrefab;
	[Tooltip("Fire object spawned by fire module.")]
	public GameObject firePrefab;
	[Tooltip("Weight object spawned by weight module.")]
	public GameObject weightPrefab;
	[Tooltip("Barrier object spawned by barrier module.")]
	public GameObject barrierPrefab;
	#endregion
	#endregion

	//Set references
	private void Start()
    {
        spell = GetComponent<Spell>();
		sie = FindObjectOfType<SingleInstanceEnforcer>();

		lineRenderer = GetComponent<LineRenderer>();

        if (IsCurrentlyVR())
        {
            rotationReference = FindObjectOfType<Camera>().gameObject;
        }
        else
        {
            rotationReference = FindObjectOfType<PlayerController>().gameObject;
        }
    }

	//Calls components based on a parsed list
	public IEnumerator HandleSpell(List<string> modules)
	{
		SpellInfo info = new SpellInfo(0, new List<Vector3>(), new List<GameObject>());

		foreach (string module in modules)
		{
			DataCoroutine<SpellInfo> dc = null;

			switch (module)
			{
				case "Projectile":
					dc = new DataCoroutine<SpellInfo>(this, Projectile(info));

					break;
				case "Split":
					dc = new DataCoroutine<SpellInfo>(this, Split(info));

					break;
				case "Charge":
					dc = new DataCoroutine<SpellInfo>(this, Charge(info));

					break;
				case "Touch":
					dc = new DataCoroutine<SpellInfo>(this, Touch(info));

					break;
				case "AOE":
					dc = new DataCoroutine<SpellInfo>(this, AOE(info));

					break;
				case "Proximity":
					dc = new DataCoroutine<SpellInfo>(this, Proximity(info));

					break;
				case "Timer":
					dc = new DataCoroutine<SpellInfo>(this, Timer(info));

					break;
				case "Fire":
					dc = new DataCoroutine<SpellInfo>(this, Fire(info));

					break;
				case "Push":
					dc = new DataCoroutine<SpellInfo>(this, Push(info));

					break;
				case "Pull":
					dc = new DataCoroutine<SpellInfo>(this, Pull(info));

					break;
				case "Weight":
					dc = new DataCoroutine<SpellInfo>(this, Weight(info));

					break;
				case "Barrier":
					dc = new DataCoroutine<SpellInfo>(this, Barrier(info));

					break;
				case "Null":
					dc = new DataCoroutine<SpellInfo>(this, Null(info));

					break;
			}

			yield return dc.coroutine;

			if (!dc.result.shouldContinue)
			{
                yield break;
			}
            
			info.potency = dc.result.potency;
			info.collisionPoints = dc.result.collisionPoints;
			info.collisionObjects = dc.result.collisionObjects;
		}

		yield return new WaitForEndOfFrame();
        if (IsCurrentlyVR())
        {
            Destroy(gameObject);
        }
	}

	#region Primary casting modules
	//Throw a projectile
	IEnumerator Projectile(SpellInfo info)
	{
        activeprojectile = true;
        float holdTime = 0.0f;
        Vector3 direction = Vector3.zero;

		bool isVR = IsCurrentlyVR();

        if (!isVR)
        {
            while (Input.GetButton("Fire1"))
            {
                holdTime += Time.deltaTime;
                direction = transform.forward;
                yield return info;
            }
        }

        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);

        if (isVR)
        {
            projectile.GetComponent<Rigidbody>().velocity = projectileVelocity * projectileSpeedMultiplier;
            projectile.GetComponent<Rigidbody>().angularVelocity = projectileAngularV ;

            projectileVelocity = Vector3.zero;
            projectileAngularV = Vector3.zero;
        }
        else
        {
            projectile.GetComponent<Rigidbody>().velocity = direction * holdTime * 10;
        }
        
        projectile.GetComponent<ProjectileReturn>().caller = this;

        while (activeprojectile)
        {
            yield return info;
        }
        
        if(projectile == null) // iof the projectile destrys itself without a target
        {
            info.shouldContinue = false;

            yield return info;

            yield break;
        }
        
        info.potency = 1;
        info.collisionPoints.Add(projectile.GetComponent<ProjectileReturn>().whereHit);
        info.collisionObjects.Add(projectile.GetComponent<ProjectileReturn>().whatHit);

        Destroy(projectile);

		playerRotation = rotationReference.transform.rotation;

        yield return info; 

        spell.isSpellCasted = true;
    }

	//Comment
	IEnumerator Split(SpellInfo info)
	{
		playerRotation = rotationReference.transform.rotation;
		
		yield return info;

		spell.isSpellCasted = true;

		//could try calling handlespell 3 times with projectile in place of split
		//develop a system that only renders the projectile once
		//then yield break and tell the original coroutine to break
		//how to tell split projectiles to have randomised trajectories?
	}

	//Beam maintained while the button is held
	IEnumerator Charge(SpellInfo info)
    {
        float holdTime = 0.0f;

        RaycastHit hit = new RaycastHit();

        lineRenderer.enabled = true;

        bool hitTest = false;

		bool isVR = IsCurrentlyVR();
		
        while (IsChargeHeld(isVR))
        {
            if (isVR)
            {
                hitTest = Physics.Raycast(obj.gameObject.transform.position, handTransform.forward, out hit, 1000.0f, ~(1<<8));
                Debug.Log(hit.collider.gameObject.layer);
                lineRenderer.SetPosition(0, obj.gameObject.transform.position);
            }
            else
            {
                hitTest = Physics.Raycast(transform.position, transform.forward, out hit, 1000.0f);
                lineRenderer.SetPosition(0, this.transform.position);
            }
            
            //make it cast from hand
            if(hitTest)
            {
                //Debug.DrawLine(transform.position, hit.point, Color.green, 5.0f);

                lineRenderer.SetPosition(1, hit.point);

				float tempWidth = 0.0125f + Mathf.Min(holdTime / maxChargeTime, 1.0f) / 10.0f;
				
                lineRenderer.startWidth = tempWidth;
                lineRenderer.endWidth = tempWidth;
            }

            yield return info;

            holdTime += Time.deltaTime;
        }

        lineRenderer.enabled = false;

        holdTime = Mathf.Min(holdTime, maxChargeTime);
		
        info.potency = 0.5f + 1.0f * holdTime / maxChargeTime;
        info.collisionPoints.Add(hit.point);
        info.collisionObjects.Add(hit.transform.gameObject);

		playerRotation = rotationReference.transform.rotation;

		yield return info;

		spell.isSpellCasted = true;
	}

	//Get if the charge button is held
	bool IsChargeHeld(bool isVR)
	{
		if (isVR)
		{
			return !holdAction.GetLastStateUp(obj.hand);
		}
		else
		{
			return Input.GetButton("Fire1");
		}
	}

	//Comment
	IEnumerator Touch(SpellInfo info)
	{
		//Important note for functionality with push/pull
		//For collision objects, add a null
		//For collision points, add position of hand

		playerRotation = rotationReference.transform.rotation;

		yield return info;

		spell.isSpellCasted = true;
	}
	#endregion

	#region Secondary casting modules
	//Affect everything in the radius of the prefab
	IEnumerator AOE(SpellInfo info)
	{
		GameObject aoeObject = Instantiate(aoePrefab, info.collisionPoints[0], Quaternion.identity);
		aoeObject.transform.localScale *= info.potency * aoeSizeAmplifier;

		yield return info;

		info.collisionObjects.Clear();

		foreach (GameObject obj in aoeObject.GetComponent<SpellTriggerHandler>().containedObjects)
		{
			info.collisionObjects.Add(obj);
		}
		
		Destroy(aoeObject);

		yield return info;
	}

	//Affect everything in a small area around the point of impact after something enters the area
	IEnumerator Proximity(SpellInfo info)
	{
		GameObject obj = sie.SpawnAsSet(spellID, proxPrefab, "Prox", info.collisionPoints[0]);
		obj.transform.localScale *= info.potency;
		ProxController prox = obj.GetComponent<ProxController>();
		prox.sml = this;
		
		while (!prox.isTriggered)
		{
			yield return info;
		}
		
		if (prox.shouldContinue)
		{
			yield return info;
			
			info.collisionObjects.Clear();
			
			foreach (GameObject gameObj in prox.proxObj.GetComponent<SpellTriggerHandler>().containedObjects)
			{
				info.collisionObjects.Add(gameObj);
			}

			Destroy(prox.proxObj);
			Destroy(prox.gameObject);
		}
		else
		{
			Destroy(prox.gameObject);

			info.shouldContinue = false;
		}
		
		yield return info;
	}

	//Affect everything in a small area around the point of impact after a delay
	IEnumerator Timer(SpellInfo info)
	{
		GameObject obj = sie.SpawnAsSet(spellID, timerPrefab, "Timer", info.collisionPoints[0]);
		obj.transform.localScale *= info.potency;
		TimerController timer = obj.GetComponent<TimerController>();
		timer.sml = this;
		timer.potency = info.potency;
		
		while (!timer.isDepleted)
		{
			yield return info;
		}
		
		if (timer.shouldContinue)
		{
			info.collisionObjects.Clear();

			foreach (GameObject gameObj in timer.timerObj.GetComponent<SpellTriggerHandler>().containedObjects)
			{
				info.collisionObjects.Add(gameObj);
			}
			
			Destroy(timer.timerObj);
			Destroy(timer.gameObject);
		}
		else
		{
			Destroy(timer.gameObject);

			info.shouldContinue = false;
		}

		yield return info;
	}
	#endregion

	#region Spell effect modules
	//Spawn a flame at the point of collision
	IEnumerator Fire(SpellInfo info)
    {
        GameObject flame = Instantiate(firePrefab, info.collisionPoints[0], Quaternion.identity);
        flame.transform.localScale *= info.potency;
		
        yield return info;
    }

	//Apply a repelling force from the first point of contact
	IEnumerator Push(SpellInfo info)
	{
		ApplyForce(info, 1);

		yield return info;
	}

	//Apply an attracting force from the first point of contact
	IEnumerator Pull(SpellInfo info)
	{
		ApplyForce(info, -1);

		yield return info;
	}

	//Apply force to the parsed objects
	void ApplyForce(SpellInfo info, int forceModifier)
	{
		Vector3 origin = info.collisionPoints[0];
		Vector3 direction;

		RaycastHit hit;

		for (int i = 0; i < info.collisionObjects.Count; i++)
		{
			GameObject obj = info.collisionObjects[i];

			if(obj != null)
			{
				if (obj.GetComponent<Rigidbody>() != null)
				{
					direction = obj.transform.position - origin;

					if (Physics.Raycast(origin - Vector3.Normalize(direction) * 0.01f, direction, out hit, 1000, ~(1 << LayerMask.NameToLayer("Ignore Raycast"))))
					{
						if (hit.collider.gameObject == obj)
						{
							float pushForce = (1 - (hit.distance / maxForceDistance)) * maxForce * forceModifier;

							obj.GetComponent<Rigidbody>().AddForce(Vector3.Normalize(obj.transform.position - origin) * pushForce);
						}
					}
				}
			}
		}
	}

	//Produce a physical weight
	IEnumerator Weight(SpellInfo info)
	{
		GameObject weight = sie.SpawnAsSet(spellID, weightPrefab, "Weight", info.collisionPoints[0]);
		//weight.transform.rotation = playerRotation;
		weight.transform.localScale *= info.potency;
		weight.GetComponent<Rigidbody>().mass *= info.potency;
		
		yield return info;
	}

	//Produce a non-physical barrier with collision
	IEnumerator Barrier(SpellInfo info)
	{
		GameObject barrier = sie.SpawnAsSet(spellID, barrierPrefab, "Barrier", info.collisionPoints[0]);
		barrier.transform.rotation = playerRotation;
		barrier.transform.localScale *= info.potency;

		yield return info;
	}

	//Comment
	IEnumerator Null(SpellInfo info)
	{
		yield return info;
	}
	#endregion

	//Determine if VR is being used
	public static bool IsCurrentlyVR()
	{
		var inputDevices = new List<UnityEngine.XR.InputDevice>();
		UnityEngine.XR.InputDevices.GetDevices(inputDevices);

		return inputDevices.Count > 0;
	}
}

//Data used for spell behaviours
public class SpellInfo
{
	//Spell potency
	public float potency;
	//Points of collision
    public List<Vector3> collisionPoints;
	//Objects collided with
    public List<GameObject> collisionObjects;
	//Should the spell countinue?
	public bool shouldContinue = true;

    public SpellInfo(float _potency, List<Vector3> _points, List<GameObject> _objects)
    {
        potency = _potency;
        collisionPoints = _points;
        collisionObjects = _objects;
    }
}

//Runs a coroutine and retrieves values from it
public class DataCoroutine<T>
{
	//Data that can be retrieved
	public T result;

	//Reference to internal coroutine used to yield externally
    public Coroutine coroutine { get; private set; }
	//Target IEnumerator
    private IEnumerator target;

    public DataCoroutine(MonoBehaviour owner, IEnumerator target)
    {
        this.target = target;

        coroutine = owner.StartCoroutine(Run());
    }

	//Retrive values from the running coroutine
    private IEnumerator Run()
    {
        while (target.MoveNext())
        {
            result = (T)target.Current;

            yield return result;
        }
    }
}