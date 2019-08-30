using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NullManager : MonoBehaviour
{
    //Null state
    [Tooltip("Whether or not this object is nulled.")]
    [SerializeField]
    private bool isNulled = false;

    //Null projection object
    private GameObject nullProjection;
	[Tooltip("Null shader.")]
	public Shader nullShader;
	[HideInInspector]
	public bool isProjection = false;

    //Retrieve null state
    public bool IsNulled
    {
        get
        {
            return isNulled;
        }
    }
    
    //Create null projection
    void Start()
    {
		if (isProjection)
		{
			return;
		}

		Rigidbody rb = GetComponent<Rigidbody>();
		bool kinematicState = false;

		if (rb != null)
		{
			kinematicState = rb.isKinematic;
			rb.isKinematic = true;
		}

		StartCoroutine(HandleKinematicState(rb, kinematicState));

		nullProjection = Instantiate(gameObject, gameObject.transform.position, gameObject.transform.rotation);
		nullProjection.transform.localScale *= 1.125f;
		nullProjection.layer = LayerMask.NameToLayer("Ignore Raycast");
		nullProjection.transform.SetParent(gameObject.transform);
		nullProjection.GetComponent<NullManager>().isProjection = true;

        Component[] allComponents = nullProjection.GetComponents<Component>();
        
        foreach(Component comp in allComponents)
        {
            if(!(comp.GetType() == typeof(Transform) || comp.GetType() == typeof(MeshFilter) || comp.GetType() == typeof(MeshRenderer)))
            {
                Destroy(comp);
            }
        }
		
		foreach (Material mat in nullProjection.GetComponent<MeshRenderer>().materials)
		{
			//temp
			/*mat.shader = Shader.Find("Standard");
			Color temp = mat.color;
			temp.a = 0.625f;
			mat.color = temp;*/

			mat.shader = nullShader;
		}
		
        nullProjection.SetActive(isNulled);
    }
    
	//Stop the projection from breaking physics
	IEnumerator HandleKinematicState(Rigidbody rb, bool state)
	{
		yield return new WaitForEndOfFrame();
		
		if (rb != null)
		{
			rb.isKinematic = state;
		}
	}

    //Invert null state
    public void InvertNullState()
    {
		isNulled = !isNulled;

		nullProjection.SetActive(isNulled);
	}
}
