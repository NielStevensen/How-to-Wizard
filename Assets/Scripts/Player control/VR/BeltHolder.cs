using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeltHolder : MonoBehaviour
{
    GameObject relativeTo;
    public LayerMask ignoreRays; // layers to ignore
    float floorY;
    RaycastHit[] hit;
    [Tooltip("propotion of the head height to place spell")]
    public float waistHeight;
    public GameObject[] slots;
    public bool[] states;

    [Tooltip("Displace the belt during play.")]
    public Vector3 displacement = Vector3.zero;

    void Start()
    {
        relativeTo = GameObject.FindObjectOfType<VRMovement>().gameObject.GetComponentInChildren<CameraController>().gameObject;
        hit = Physics.RaycastAll(relativeTo.transform.position, transform.up* -1, 1000.0f, ignoreRays);
        
        foreach(RaycastHit hit_ in hit)
        {
            if (hit_.collider.gameObject.name.Split(' ')[0] == "Floor")
            {
                floorY = hit_.point.y;

                break;
            }
        }
    }

// Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(relativeTo.transform.position.x, (relativeTo.transform.position.y - floorY) * waistHeight + floorY, relativeTo.transform.position.z) + transform.TransformDirection(displacement);
    }
}
