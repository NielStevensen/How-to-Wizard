using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeltControll : MonoBehaviour
{
    public LayerMask ignoreRays; // layers to ignore
    GameObject relativeTo;
    RaycastHit hit;
    float floorY;
    public bool occupied;
    [Tooltip ("propotion of the head height to place spell")]
    public float waistHeight;
    public int myNumber;

    // Start is called before the first frame update
    void Start()
    {
        relativeTo = GameObject.FindObjectOfType<Camera>().gameObject;
        Physics.Raycast(relativeTo.transform.position, transform.up * -1, out hit, 1000.0f, ~ignoreRays);
        floorY = hit.point.y;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(relativeTo.transform.position.x,(relativeTo.transform.position.y - floorY) * waistHeight + floorY, relativeTo.transform.position.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Spell>() != null && !occupied)
        {
            Debug.Log("hitbelt");
            FixedJoint[] toCheck = GameObject.FindObjectsOfType<FixedJoint>();
            foreach(FixedJoint a in toCheck)
            {
                if (a.connectedBody.gameObject == other.gameObject) a.GetComponent<PickupSpell>().beltSlots[myNumber] = true;
                occupied = true;
            }
            // get hand refrence and set vallue on hand to specify location is valid
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Spell>() != null && occupied)
        {
            FixedJoint[] toCheck = GameObject.FindObjectsOfType<FixedJoint>();
            foreach (FixedJoint a in toCheck)
            {
                if (a.connectedBody.gameObject == other.gameObject) a.GetComponent<PickupSpell>().beltSlots[myNumber] = false;
                occupied = false;
            }
            // get hand refrence and set vallue on hand to specify location is no longer valid
        }
    }
}
