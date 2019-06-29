using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellModuleList : MonoBehaviour
{
	//References
	private Spell spell;
	
	//Line renderer for charge module
	private LineRenderer lineRenderer;

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

	//Set references
	private void Start()
    {
		spell = GetComponent<Spell>();

		lineRenderer = GetComponent<LineRenderer>();
    }

	//Calls components based on a parsed list
	public IEnumerator HandleSpell(string[] modules)
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

			info.potency = dc.result.potency;
			info.collisionPoints = dc.result.collisionPoints;
			info.collisionObjects = dc.result.collisionObjects;
		}
	}

	#region Primary casting modules
	//Comment
	IEnumerator Projectile(SpellInfo info)
	{
		yield return info;

		spell.isSpellCasted = true;
	}

	//Comment
	IEnumerator Split(SpellInfo info)
	{
		yield return info;

		spell.isSpellCasted = true;
	}

	//Beam maintained while button is held
	IEnumerator Charge(SpellInfo info)
    {
        float holdTime = 0.0f;

        RaycastHit hit = new RaycastHit();

        lineRenderer.enabled = true;

        while (Input.GetButton("Fire1"))
        {
            //make it cast from hand
            if(Physics.Raycast(transform.position, transform.forward, out hit, 1000.0f))
            {
                lineRenderer.SetPosition(0, this.transform.position);
                lineRenderer.SetPosition(1, hit.point);

                lineRenderer.SetWidth(Mathf.Min(holdTime / 5, 1.0f), Mathf.Min(holdTime / 5, 1.0f));
            }

            yield return info;

            holdTime += Time.deltaTime;
        }

        lineRenderer.enabled = false;

        holdTime = Mathf.Min(holdTime, 5.0f);

        info.potency = 0.5f + 1.0f * holdTime / 5.0f;
        info.collisionPoints.Add(hit.point);
        info.collisionObjects.Add(hit.transform.gameObject);

        yield return info;

		spell.isSpellCasted = true;
	}

	//Comment
	IEnumerator Touch(SpellInfo info)
	{
		yield return info;

		spell.isSpellCasted = true;
	}
	#endregion

	#region Secondary casting modules
	//Comment
	IEnumerator AOE(SpellInfo info)
	{
		yield return info;
	}

	//Comment
	IEnumerator Proximity(SpellInfo info)
	{
		yield return info;
	}

	//Comment
	IEnumerator Timer(SpellInfo info)
	{
		yield return info;
	}
	#endregion

	#region Spell effect modules
	//Spawn a flame at point of collision
	IEnumerator Fire(SpellInfo info)
    {
        foreach (GameObject obj in info.collisionObjects)
        {
            // light on fire
            Debug.Log("am buring");
        }

        GameObject flame = Instantiate(firePrefab, info.collisionPoints[0], Quaternion.identity);
        flame.transform.localScale = new Vector3 (info.potency, info.potency, info.potency);
		
        yield return info;
    }

	//Comment
	IEnumerator Push(SpellInfo info)
	{
		yield return info;
	}

	//Comment
	IEnumerator Pull(SpellInfo info)
	{
		yield return info;
	}

	//Comment
	IEnumerator Weight(SpellInfo info)
	{
		yield return info;
	}

	//Comment
	IEnumerator Barrier(SpellInfo info)
	{
		yield return info;
	}

	//Comment
	IEnumerator Null(SpellInfo info)
	{
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