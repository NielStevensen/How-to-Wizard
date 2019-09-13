using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeltHolder : MonoBehaviour
{
    GameObject relativeTo;
    public LayerMask ignoreRays; // layers to ignore
    float floorY;
    RaycastHit hit;
    [Tooltip("propotion of the head height to place spell")]
    public float waistHeight;
    public GameObject[] slots;

    void Start()
    {
        relativeTo = GameObject.FindObjectOfType<Camera>().gameObject;
        Physics.Raycast(relativeTo.transform.position, transform.up* -1, out hit, 1000.0f, ~ignoreRays);
        floorY = hit.point.y;
    }

// Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(relativeTo.transform.position.x, (relativeTo.transform.position.y - floorY) * waistHeight + floorY, relativeTo.transform.position.z);
    }
}
