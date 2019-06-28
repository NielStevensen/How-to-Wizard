using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellModuleList : MonoBehaviour
{
    private LineRenderer lineRenderer;

    public GameObject fire;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    //describe
    IEnumerator Charge(SpellInfo info)
    {
        float holdTime = 0.0f;

        RaycastHit hit = new RaycastHit();

        lineRenderer.enabled = true;

        while (Input.GetButton("Fire1"))
        {
            //make it cast from hand
            if(Physics.Raycast(transform.position,transform.forward, out hit, 1000f))
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
    }

    IEnumerator Fire(SpellInfo info)
    {
        foreach (GameObject a in info.collisionObjects)
        {
            // light on fire
            Debug.Log("am buring");
        }
        GameObject flame = Instantiate(fire,info.collisionPoints[0],Quaternion.identity);
        flame.transform.localScale = new Vector3 (info.potency, info.potency, info.potency);
        SpellInfo returnInfo = new SpellInfo(0, new List<Vector3> { Vector3.zero }, new List<GameObject> { null });

        yield return returnInfo;
    }

    // runs components based on list it is given
    public IEnumerator HandleSpell(string[] componentsList)
    {
        SpellInfo info = new SpellInfo(0, new List<Vector3>(), new List<GameObject>()); 

        foreach(string module in componentsList)
        {
            DataCoroutine<SpellInfo> dc = null;

            switch (module)
            {
                case "Charge":
                    dc = new DataCoroutine<SpellInfo>(this, Charge(info));

                    break;
                case "Fire":
                    dc = new DataCoroutine<SpellInfo>(this, Fire(info));

                    break;
            }

            yield return dc.coroutine;

            info.potency = dc.result.potency;
            info.collisionPoints = dc.result.collisionPoints;

            info.collisionObjects = dc.result.collisionObjects;
        }
        yield return new WaitForEndOfFrame();
    }
}

// necesary information fpor spell beavours
public class SpellInfo
{
    public float potency;
    public List<Vector3> collisionPoints;
    public List<GameObject> collisionObjects;

    public SpellInfo(float power, List<Vector3> Points, List<GameObject> objects)
    {
        potency = power;
        collisionPoints = Points;
        collisionObjects = objects;
    }
}

//describe
public class DataCoroutine<T>
{
    public T result;

    public Coroutine coroutine { get; private set; }
    private IEnumerator target;

    public DataCoroutine(MonoBehaviour owner, IEnumerator target)
    {
        this.target = target;

        coroutine = owner.StartCoroutine(Run());
    }

    private IEnumerator Run()
    {
        while (target.MoveNext())
        {
            result = (T)target.Current;

            yield return result;
        }
    }
}