using System.Collections;
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
    public int modifier;
    public AudioClip hitSound;
	
    private void OnCollisionEnter(Collision collision)
    {
        if (Info.IsCurrentlyVR())
        {
            if(collision.gameObject.layer == 8)
            {
                GetComponent<Rigidbody>().useGravity = false;
                transform.SetParent(collision.transform);
            }
            else
            {
                GetComponent<Rigidbody>().useGravity = true;
                if(modifier == 10)
                {
                    caller.activeprojectile = false;
                }
                else
                {
                    caller.activeSplits[modifier + 1] = false;
                }                
                whatHit = collision.gameObject;
                whereHit = collision.contacts[0].point;
            }
        }
		else
		{
			if (collision.gameObject.layer != 8)
			{
				GetComponent<Rigidbody>().useGravity = true;
                if (modifier == 10)
                {
                    caller.activeprojectile = false;
                }
                else
                {
                    caller.activeSplits[modifier + 1] = false;
				}

                whatHit = collision.gameObject;
				whereHit = collision.contacts[0].point;
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
        if (modifier == 10)
        {
            caller.activeprojectile = false;
        }
        else
        {
            caller.activeSplits[modifier + 1] = false;
        }
        Destroy(gameObject);
    }
}
