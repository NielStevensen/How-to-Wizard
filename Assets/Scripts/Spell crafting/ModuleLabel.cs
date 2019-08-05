using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ModuleLabel : MonoBehaviour
{
    Transform player;
    public Text myText;
    // Start is called before the first frame update
    void Start()
    {
        myText.text = GetComponent<CrystalInfo>().module;
        player = FindObjectOfType<Camera>().transform;
    }
    // Update is called once per frame
    void Update()
    {
        Vector3 relativePos = player.position - transform.position;

        // the second argument, upwards, defaults to Vector3.up
        Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
        myText.transform.rotation = rotation;
    }
}
