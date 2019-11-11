using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    public float projectilePredictionDistance;
    [Tooltip("the distance the line will travel before a new line is created in projectile predictions. Highernumbers will be more accurte to the real result")]
    public int maxSimulationSegments;
    [Tooltip("how many segments to attempt to simulate")]
    public float maxThrow;
    [HideInInspector]
    public Vector3 projectileVelocity;
    [HideInInspector]
    public Vector3 projectileAngularV;
	[HideInInspector]
	public bool activeprojectile = true;
    [HideInInspector]
    public bool[] activeSplits = new bool[3] { true, true, true };
	private List<GameObject> currentSplits = new List<GameObject>();
	[Tooltip("How much split projectiles vary.")]
	public float splitVariance = 1.25f;

    [Space(10)]

	//Charge values
	[Tooltip("The amount of time in seconds charge must be held to achieve maximum charge.")]
	public float maxChargeTime = 2.5f;
	public LayerMask chargeIgnoreRays;
	private LineRenderer lineRenderer;
    public SteamVR_Action_Boolean holdAction;

    [Space(10)]

    //Touch values
    public float touchDistance;
    public float touchSize;

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
    [Tooltip("segment for charge and pc throw simulations")]
    public GameObject segment;
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

        if (Info.IsCurrentlyVR())
        {
            rotationReference = FindObjectOfType<InheritYRotation>().gameObject;
        }
        else
        {
            rotationReference = FindObjectOfType<PlayerController>().gameObject;
        }
    }

	//Calls components based on a parsed list
	public IEnumerator HandleSpell(List<string> modules , int modifier)
	{
		if(modifier == 10)
		{
			FindObjectOfType<ExitManager>().spellsCasted++;
		}

		SpellInfo info = new SpellInfo(0, new List<Vector3>(), new List<GameObject>());

		float moduleID = 0;

		foreach (string module in modules)
		{
			DataCoroutine<SpellInfo> dc = null;

			switch (module)
			{
				case "Projectile":
					dc = new DataCoroutine<SpellInfo>(this, Projectile(info, modifier));

					break;
				case "Split":
					dc = new DataCoroutine<SpellInfo>(this, Split(info, modules));

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
					dc = new DataCoroutine<SpellInfo>(this, Weight(info, moduleID));

					break;
				case "Barrier":
					dc = new DataCoroutine<SpellInfo>(this, Barrier(info, moduleID));

					break;
				case "Null":
					dc = new DataCoroutine<SpellInfo>(this, Null(info));

					break;
			}

			yield return dc.coroutine;

			if (!dc.result.shouldContinue)
			{
                if(module == "Split")
                {
                    yield break;
                }

                break;
			}
            
			info.potency = dc.result.potency;
			info.collisionPoints = dc.result.collisionPoints;
			info.collisionObjects = dc.result.collisionObjects;

			moduleID += 0.2f;
		}

		yield return new WaitForEndOfFrame();

		bool shouldDestroy = true;

		if(currentSplits.Count > 0)
		{
			foreach(GameObject obj in currentSplits)
			{
				if(obj != null)
				{
					shouldDestroy = false;

					break;
				}
			}
		}

		if (shouldDestroy)
		{
			Destroy(gameObject);
		}
	}

	#region Primary casting modules
	//Throw a projectile
	IEnumerator Projectile(SpellInfo info, int modifier)
	{
        #region localVariables
        RaycastHit hit = new RaycastHit();
        Vector3 direction = Vector3.zero; // the direction the projectile will be rleased in
        float holdTime = 0.0f; // how long has the player held throw

        //Projectilespawning + releasing
        activeprojectile = true; // is there a projectile in the scene
        float power = 0.0f; // the velocity trhe projectile will br relased with

        // arc simulation
        float time = 0.0f; //virtual time used for simulating arc in PC
        bool simulationComplete = false; // has the throw simulation hit something (or to many segments)
        int simulatedSegments = 0;
        float speed = 0.0f; // the speed opf the projectile simulation

        List<GameObject> lineSegments = new List<GameObject>();
        GameObject targetOrb = null;
        #endregion

        Vector3 previousPoint = transform.position;
        Vector3 nextPoint = transform.position;

        bool isVR = Info.IsCurrentlyVR();

        if (!isVR)
        {
			while (lineSegments.Count < maxSimulationSegments && modifier % 10 == 0)
			{
				lineSegments.Add(Instantiate(segment));
				lineSegments[lineSegments.Count - 1].name = lineSegments.Count.ToString();
			}

			while (Input.GetButton("Fire2"))
			{
				holdTime += Time.deltaTime;
				power = Mathf.Min(holdTime * 2, maxThrow) * 10;
				direction = transform.forward;

				if (modifier % 10 == 0)
				{
					time = 0;
					simulatedSegments = 0;
					simulationComplete = false;
					speed = (direction * power).magnitude; //velocity if simulated throw

					previousPoint = transform.position; // rest positions to prevent end to start relooping
					nextPoint = transform.position;
					//simulate arc for up to maximum number of segments
					while (simulatedSegments < maxSimulationSegments && !simulationComplete)
					{
                        if (targetOrb != null) Destroy(targetOrb);
                        previousPoint = nextPoint;
						time += (projectilePredictionDistance / 100) / speed; // simulated time for prediction based on length of rendered line
						nextPoint.x = transform.position.x + ((direction * power).x * time);
						nextPoint.y = transform.position.y + ((direction * power).y * time) + (0.5f * Physics.gravity.y * (time * time));
						nextPoint.z = transform.position.z + ((direction * power).z * time);

						lineSegments[simulatedSegments].transform.position = previousPoint; // place segments
						lineSegments[simulatedSegments].transform.LookAt(nextPoint);
						lineSegments[simulatedSegments].transform.localScale = new Vector3(0.1f, 0.1f, (nextPoint - previousPoint).magnitude / 2);
						simulatedSegments += 1;
						if (Physics.Raycast(previousPoint, nextPoint - previousPoint, out hit, (nextPoint - previousPoint).magnitude))
						{
							nextPoint = hit.point;
							simulationComplete = true;
                            targetOrb = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                            Destroy(targetOrb.GetComponent<SphereCollider>());
                            targetOrb.transform.position = hit.point;
                            targetOrb.transform.localScale *= 0.6f;
                            targetOrb.GetComponent<Renderer>().material = segment.GetComponentInChildren<Renderer>().sharedMaterial;
                        }

					}

					while (simulatedSegments < maxSimulationSegments && simulationComplete)
					{
						lineSegments[simulatedSegments].transform.position = previousPoint;
						lineSegments[simulatedSegments].transform.LookAt(nextPoint);
						lineSegments[simulatedSegments].transform.localScale = new Vector3(0.1f, 0.1f, 0);
						simulatedSegments += 1;
					}
				}
				
				yield return info;
			}
        }

        foreach (GameObject a in lineSegments)
        {
            Destroy(a);
        }
        if (targetOrb != null) Destroy(targetOrb);

        if (modifier % 10 == 0)
		{
            NotifySpellCasted();

            if (!isVR)
            {
                FindObjectOfType<PlayerController>().animator.SetTrigger(PlayerController.throwHash);
            }
		}
		
		GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        projectile.GetComponent<ProjectileReturn>().modifier = modifier;
        projectile.GetComponent<ProjectileReturn>().caller = this;

		Vector3 trajectory = Vector3.zero;

		if(modifier != 10)
		{
			currentSplits.Add(projectile);
		}

        if (isVR)
        {
            trajectory = projectileVelocity * projectileSpeedMultiplier;
			projectile.GetComponent<Rigidbody>().angularVelocity = projectileAngularV ;
        }
        else
        {
			trajectory = direction * power;
		}
        
		if(modifier % 10 != 0)
		{
            if (isVR)
            {
                trajectory += FindObjectOfType<InheritYRotation>().transform.right * modifier * splitVariance;
            }
            else
            {
                trajectory += transform.right * modifier * splitVariance;
            }
		}

        projectile.GetComponent<Rigidbody>().velocity = trajectory;

		if(modifier == 10)
		{
			while (activeprojectile)
			{
				yield return info;
			}
		}
		else
		{
			while(activeSplits[modifier + 1])
			{
				yield return info;
			}
		}
		
        if(projectile == null) // iof the projectile destrys itself without a target
        {
            info.shouldContinue = false;

            yield return info;
			
			yield break;
        }

		info.potency = modifier == 10 ? 1 : 0.3f;
        info.collisionPoints.Add(projectile.GetComponent<ProjectileReturn>().whereHit);
        info.collisionObjects.Add(projectile.GetComponent<ProjectileReturn>().whatHit);

        Destroy(projectile);

		if(modifier == 10)
		{
			playerRotation = rotationReference.transform.rotation;
		}

        yield return info;
	}

	//Call projectile 3 times with variance in trajectory
	IEnumerator Split(SpellInfo info, List<string> modules)
	{
		playerRotation = rotationReference.transform.rotation;

		List<string> newModules = new List<string>();

		for(int i = 0; i < modules.Count; i++)
		{
			if(i == 0)
			{
				newModules.Add("Projectile");
			}
			else
			{
				newModules.Add(modules[i]);
			}
		}
		
        for(int i = -1; i < 2; i ++)
        {
            StartCoroutine(HandleSpell(newModules, i));
        }

		info.shouldContinue = false;

		yield return info;
	}

	//Beam maintained while the button is held
	IEnumerator Charge(SpellInfo info)
    {
        float holdTime = 0.0f;
        GameObject line = Instantiate(segment);

        float length; // length of segment
        float width; // width of segment

		RaycastHit hit = new RaycastHit();
        bool hitTest = false;

		bool isVR = Info.IsCurrentlyVR();

		Vector3 origin;
		Vector3 direction;

        if (!isVR)
        {
            FindObjectOfType<PlayerController>().animator.SetTrigger(PlayerController.chargeHash);
        }

		while (IsChargeHeld(isVR))
        {
            if (isVR)
            {
				origin = handTransform.position;
				direction = handTransform.forward;
            }
            else
            {
				origin = transform.position;
				direction = transform.forward;
            }

			hitTest = Physics.Raycast(origin, direction, out hit, 1000.0f, ~chargeIgnoreRays);
			
			if (hitTest)
            {
                length = (origin - hit.point).magnitude / 2;
            }
			else
			{
                length = 1000f;
			}
			
            width = 0.0125f + Mathf.Min(holdTime / maxChargeTime, 1.0f) / 10.0f;
            line.transform.localScale = new Vector3(width, width, length);
            line.transform.position = origin;
            line.transform.LookAt(hit.point);

			yield return info;

            holdTime += Time.deltaTime;
        }

        if (!isVR)
        {
            FindObjectOfType<PlayerController>().animator.SetTrigger(PlayerController.endChargeHash);
        }

		holdTime = Mathf.Min(holdTime, maxChargeTime);

		if (hitTest)
		{
			info.potency = 0.5f + 1.0f * holdTime / maxChargeTime;
			info.collisionPoints.Add(hit.point);
			info.collisionObjects.Add(hit.transform.gameObject);

			playerRotation = rotationReference.transform.rotation;
		}
		else
		{
			info.shouldContinue = false;
		}
		
		yield return info;

        Destroy(line);

		NotifySpellCasted();
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
			return Input.GetButton("Fire2");
		}
	}

	//Produce spell efects at the player's hand
	IEnumerator Touch(SpellInfo info)
	{
        RaycastHit hit = new RaycastHit();
        GameObject aoeObject = null;
        Vector3 touchPoint = Vector3.zero;

		if (Info.IsCurrentlyVR())
		{
			touchPoint = handTransform.position;
		}
		else
		{
			if (Physics.Raycast(transform.position, transform.forward, out hit, touchDistance))
			{
				touchPoint = hit.point;
			}
			else
			{
				touchPoint = transform.position + transform.forward * touchDistance;
			}
            
			FindObjectOfType<PlayerController>().animator.SetTrigger(PlayerController.touchHash);
		}
		
        info.collisionPoints.Add(touchPoint);

        aoeObject = Instantiate(aoePrefab, touchPoint, Quaternion.identity);
        aoeObject.transform.localScale *= touchSize;

        yield return info;

        foreach (GameObject obj in aoeObject.GetComponent<SpellTriggerHandler>().containedObjects)
        {
            info.collisionObjects.Add(obj);
        }

        Destroy(aoeObject);

        info.potency = 2;

		playerRotation = rotationReference.transform.rotation;

		yield return info;

		NotifySpellCasted();
	}

	//Notify player control scripts that the spell has been cast and that the cooldown should start depleting
	void NotifySpellCasted()
	{
		if (!Info.IsCurrentlyVR())
		{
			FindObjectOfType<PlayerController>().isSpellCasted = true;
		}
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

			if (obj != null)
			{
				if (obj.GetComponent<Rigidbody>() != null)
				{
					bool canBeForced = true;

					NullManager nullManager = obj.GetComponent<NullManager>();

					if (nullManager != null)
					{
						if (nullManager.IsNulled)
						{
							canBeForced = false;
						}
					}

					if (canBeForced)
					{
						direction = obj.transform.position - origin;

						if (Physics.Raycast(origin - Vector3.Normalize(direction) * 0.01f, direction, out hit, 1000, ~(1 << LayerMask.NameToLayer("Ignore Raycast"))))
						{
							if (hit.collider.gameObject == obj)
							{
								float pushForce = (1 - (hit.distance / maxForceDistance)) * maxForce * forceModifier;

								Vector3 pushVector = Vector3.Normalize(obj.transform.position - origin) * pushForce;
								pushVector.y = 0;
								
								obj.GetComponent<Rigidbody>().AddForce(pushVector);
							}
						}
					}
				}
			}
		}
	}

	//Produce a physical weight
	IEnumerator Weight(SpellInfo info, float moduleID)
	{
		GameObject weight = sie.SpawnAsSet(spellID + moduleID, weightPrefab, "Weight", info.collisionPoints[0]);
		weight.transform.localScale *= info.potency;
		weight.GetComponent<Rigidbody>().mass *= info.potency;
		
		yield return info;
	}

	//Produce a non-physical barrier with collision
	IEnumerator Barrier(SpellInfo info, float moduleID)
	{
		GameObject barrier = sie.SpawnAsSet(spellID + moduleID, barrierPrefab, "Barrier", info.collisionPoints[0]);
		barrier.transform.rotation = playerRotation;
		barrier.transform.localScale = new Vector3(info.potency, 10000, 0.1f);

		yield return info;
	}

	//Invert the null stated of collided objects
	IEnumerator Null(SpellInfo info)
	{
		for (int i = 0; i < info.collisionObjects.Count; i++)
		{
			GameObject obj = info.collisionObjects[i];

			if (obj != null)
			{
				NullManager nullManager = obj.GetComponent<NullManager>();

				if (nullManager != null)
				{
					nullManager.HandleNullEvent();
				}
			}
		}

		yield return info;
	}
	#endregion
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