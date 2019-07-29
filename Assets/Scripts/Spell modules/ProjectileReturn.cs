using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileReturn : MonoBehaviour
{
    public GameObject whatHit;
    public Vector3 whereHit;
    public SpellModuleList caller;

    private void OnCollisionEnter(Collision collision)
    {
        if (SpellModuleList.IsCurrentlyVR())
        {
            if(collision.gameObject.layer == 8)
            {
                GetComponent<Rigidbody>().useGravity = false;
                transform.SetParent(collision.transform);
            }
            else
            {
                //transform.SetParent(null);
                GetComponent<Rigidbody>().useGravity = true;
                caller.activeprojectile = false;
                Debug.Log("hit");
                whatHit = collision.gameObject;
                whereHit = collision.contacts[0].point;
                Debug.Log(collision.gameObject.name);
            }
        }
		else
		{
			if (collision.gameObject.layer != 8)
			{
				//transform.SetParent(null);
				GetComponent<Rigidbody>().useGravity = true;
				caller.activeprojectile = false;
				Debug.Log("hit");
				whatHit = collision.gameObject;
				whereHit = collision.contacts[0].point;
				Debug.Log(collision.gameObject.name);
			}
		}
	}

    private void Update()
    {
        //if()
    }
}
