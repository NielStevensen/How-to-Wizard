﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileReturn : MonoBehaviour
{
    public float lifeTime = 7.5f;

    [HideInInspector]
    public GameObject whatHit;
    [HideInInspector]
    public Vector3 whereHit;
    [HideInInspector]
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
				whatHit = collision.gameObject;
				whereHit = collision.contacts[0].point;
				Debug.Log(collision.gameObject.name);
			}
		}
	}

    private void Start()
    {
       StartCoroutine(destroyLifetime());
    }

    IEnumerator destroyLifetime()
    {
        yield return new WaitForSeconds(lifeTime);
        caller.activeprojectile = false;
        Destroy(gameObject);
    }
}
